Shader "Custom/URP_MOS_Optimized"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [NoScaleOffset] _MOSMap("MOS Map (R: Metallic, G: AO, B: Smoothness)", 2D) = "default_MOS" {}
        [Normal][MainNormal] _BumpMap("Normal Map", 2D) = "bump" {}
    }

        SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        // Textures
        TEXTURE2D(_BaseMap);
        TEXTURE2D(_MOSMap);
        TEXTURE2D(_BumpMap);

        // OPTIMIZATION: Single shared sampler state for all 3 textures
        SAMPLER(sampler_BaseMap);

        struct Attributes
        {
            float4 positionOS   : POSITION;
            float3 normalOS     : NORMAL;
            float4 tangentOS    : TANGENT;
            float2 uv           : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionCS   : SV_POSITION;
            float2 uv           : TEXCOORD0;
            float3 positionWS   : TEXCOORD1;
            half3 normalWS      : TEXCOORD3;
            half3 tangentWS     : TEXCOORD4;
            half3 bitangentWS   : TEXCOORD5;
        };

        Varyings Vert(Attributes input)
        {
            Varyings output = (Varyings)0;

            VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
            VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

            output.positionCS = vertexInput.positionCS;
            output.positionWS = vertexInput.positionWS;
            output.uv = input.uv;
            output.normalWS = normalInput.normalWS;
            output.tangentWS = normalInput.tangentWS;
            output.bitangentWS = normalInput.bitangentWS;

            return output;
        }

        half4 Frag(Varyings input) : SV_Target
        {
            // 1. Fetch textures using the shared sampler
            half4 albedoSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
            half3 mos = SAMPLE_TEXTURE2D(_MOSMap,  sampler_BaseMap, input.uv).rgb;
            half4 normalSample = SAMPLE_TEXTURE2D(_BumpMap, sampler_BaseMap, input.uv);

            // 2. Map MOS Channels (R: Metallic, G: AO, B: Smoothness)
            half metallic = mos.r;
            half occlusion = mos.g;
            half smoothness = mos.b;

            // 3. Unpack Tangent Space Normal
            half3 normalTS = UnpackNormal(normalSample);
            half3 N = TransformTangentToWorld(normalTS, half3x3(input.tangentWS, input.bitangentWS, input.normalWS));
            N = NormalizeNormalPerPixel(N);

            // 4. Construct Lighting Inputs
            InputData inputData = (InputData)0;
            inputData.positionWS = input.positionWS;
            inputData.normalWS = N;
            inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
            inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
            inputData.bakedGI = SampleSH(N);

            SurfaceData surfaceData = (SurfaceData)0;
            surfaceData.albedo = albedoSample.rgb;
            surfaceData.metallic = metallic;
            surfaceData.smoothness = smoothness;
            surfaceData.occlusion = occlusion;
            surfaceData.normalTS = normalTS;
            surfaceData.alpha = albedoSample.a;

            return UniversalFragmentPBR(inputData, surfaceData);
        }
        ENDHLSL
    }
    }
}