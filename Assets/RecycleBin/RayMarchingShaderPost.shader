Shader "PostProcess/RayMarching"
{    
    SubShader
    {
        Cull Off
        ZWrite Off
        Ztest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment frag
            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

            TEXTURE2D_SAMPLER2D( _MainTex, sampler_MainTex);
            TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);
            float4x4 _InvProjectionM;
            float4x4 _InvViewM;

            float4 _Blobs[100];
            float _REffect[100];
            int _Number = 0;
            float _Threshold = 0.5;

            


            float4 GetWorldPosition(float2 uv, float depth)
            {
                float4 viewPos = mul(_InvProjectionM, float4(uv*2-1, depth, 1.0));
                viewPos.xyz /= viewPos.w;

                float4 worldPos = mul(_InvViewM, float4(viewPos.xyz, 1));

                return worldPos;
            }

            float RayMarching(float3 start, float3 dir)
            {
                float3 current = start;
                float sum = 0;
                dir *= 0.5;
                for(int i = 0; i < 256; i++)
                {
                    current += dir;
                    if(current.x > -10 && current.x <10 &&
                        current.y > -10 && current.y <10 &&
                        current.z > -10 && current.z <10)
                        {
                            sum += 0.01;
                        }
                }

                //float sum = 0;
                return sum;
            }

            float Wyvill(float r, float R)
            {
                
                if(r >= R)return 0;
                float t = r / R;
                return ((-4.0f*t*t + 17.0f)*t*t - 22.0f)*t*t / 9.0f + 1.0f;         
            }

            float SPF(float3 pos)
            {
                float sum = 0;
                for(int i =0; i< _Number; i++)
                {
                    float r = length(pos - _Blobs[i].xyz);
                    sum += Wyvill(r, _REffect[i]);
                }
                return sum;
            }

            float SDF(float3 pos)
            {
                float minDis = 1000;
                for(int i = 0; i < _Number; i++)
                {
                    minDis = min(minDis, (length(pos - _Blobs[i].xyz) - _REffect[i]));
                }
                return minDis;
            } 

            float4 frag (VaryingsDefault i ) : SV_Target
            {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);

                float3 worldPos = GetWorldPosition(i.texcoord, depth).xyz;
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 ray = worldPos - rayOrigin;
                float3 dir = normalize(ray);
                
                float marched = RayMarching(rayOrigin, dir);

                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                return color + marched;
            }
            ENDHLSL
        }
    }
}
