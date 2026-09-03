Shader "Custom/URP_Hologram_Animated"
{
    Properties
    {
        [HDR] _Color("Hologram Color", Color) = (0, 0.5, 1, 1)
        _FresnelPower("Fresnel Power", Range(0.1, 10)) = 3.0
        _ScanSpeed("Scanline Speed", Float) = 2.0
        _ScanTiling("Scanline Density", Float) = 30.0
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        Blend One One
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 worldNormal  : TEXCOORD0;
                float3 viewDirWS    : TEXCOORD1;
                float3 worldPos     : TEXCOORD2; // Added to track height for lines
            };

            float4 _Color;
            float _FresnelPower;
            float _ScanSpeed;
            float _ScanTiling;

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                output.positionCS = vertexInput.positionCS;
                output.worldNormal = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                output.worldPos = vertexInput.positionWS; // Pass world position to fragment shader

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.worldNormal);
                float3 viewDir = normalize(input.viewDirWS);

                // 1. Core Fresnel Glow
                float fresnel = 1.0 - saturate(dot(normal, viewDir));
                fresnel = pow(fresnel, _FresnelPower);

                // 2. Moving Scanlines Math
                // We use input.worldPos.y (height) and add _Time.y (Unity's built-in time variable)
                float lines = sin((input.worldPos.y * _ScanTiling) - (_Time.y * _ScanSpeed));
                lines = saturate(lines * 0.5 + 0.5); // Remap sin (-1 to 1) into a clean (0 to 1) range

                // 3. Random Glitch Flicker Math
                // A rapid sine wave based on time creates a chaotic, broken projection look
                float flicker = saturate(sin(_Time.y * 40.0) * 0.2 + 0.8);

                // Combine everything together
                return _Color * fresnel * lines * flicker;
            }
            ENDHLSL
        }
    }
}
