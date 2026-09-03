Shader "Custom/URP_Hologram"
{
    Properties
    {
        [HDR] _Color("Hologram Color", Color) = (0, 0.5, 1, 1)
        _FresnelPower("Fresnel Power", Range(0.1, 10)) = 3.0
    }

        SubShader
    {
        // 1. Setup Tags for Transparent/Additive rendering
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend One One // Additive Blending
        ZWrite Off    // Don't write to depth buffer (allows transparency overlapping)

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Includes Unity's core math and coordinate transformation functions
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 2. Data structure coming from the 3D Model Mesh
            struct Attributes
            {
                float4 positionOS   : POSITION;  // Object Space position
                float3 normalOS     : NORMAL;    // Object Space normal
            };

    // 3. Data structure passed from Vertex Shader to Fragment Shader
    struct Varyings
    {
        float4 positionCS   : SV_POSITION; // Clip Space position
        float3 worldNormal  : TEXCOORD0;   // World Space normal
        float3 viewDirWS    : TEXCOORD1;   // World Space view direction
    };

    // Uniform variables exposed to the Material Inspector
    float4 _Color;
    float _FresnelPower;

    // 4. VERTEX SHADER: Handles positions and directions
    Varyings vert(Attributes input)
    {
        Varyings output;

        // Convert object position to screen/clip space
        VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
        output.positionCS = vertexInput.positionCS;

        // Convert normal to world space and calculate the camera view direction
        output.worldNormal = TransformObjectToWorldNormal(input.normalOS);
        output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

        return output;
    }

    // 5. FRAGMENT SHADER: Computes the pixel color and glow
    float4 frag(Varyings input) : SV_Target
    {
        // Normalize vectors to make sure lengths are exactly 1
        float3 normal = normalize(input.worldNormal);
        float3 viewDir = normalize(input.viewDirWS);

        // Fresnel Math: 1.0 minus dot product of Normal and View Direction
        // This outputs 0 at the center (facing camera) and 1 at the glancing edges
        float fresnel = 1.0 - saturate(dot(normal, viewDir));

        // Sharpen the edge glow using the power function
        fresnel = pow(fresnel, _FresnelPower);

        // Multiply the glowing edge factor by our chosen color
        return _Color * fresnel;
    }
    ENDHLSL
}
    }
}
