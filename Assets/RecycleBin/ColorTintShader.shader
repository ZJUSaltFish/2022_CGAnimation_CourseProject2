Shader "PostProcess/ColorTint"
{
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault;
            #pragma fragment frag;

            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            TEXTURE2D_SAMPLER2D (_MainTex, sampler_MainTex);
            float _Blend;
            float4 _Color;

            float4 frag (VaryingsDefault i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                col = lerp(col, col * _Color, _Blend);
                return col;
            }
            ENDHLSL
        }
    }
}
