Shader "Metaball/RayMarching"
{    
    SubShader
    {
        Cull Off
        ZWrite Off
        Ztest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#include Unity.cginc
            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;

            float4x4 _InvProjectionM;
            float4x4 _InvViewM;

            float4 _Blobs[100];
            float _REffect[100];
            float _Density[100];
            float4 _Color[100];
            float _Threshold = 0.5;
            float _Accuracy = 0.1;
            float _Ambient =0.2;
            float _Roughness[100];
            int _Number = 0;
            int _Steps = 32;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            struct marchHit//when hit, return surface parameters at pos.
            {
                float3 position;
                float3 normal;
                float4 color;//color.w is used to check whether this ray hit something or not
                float roughness;
            };

            float SDF(float3 pos)//get signed distance field at pos
            {
                float minDis = 1000;
                for(int i = 0; i < _Number; i++)
                {
                    minDis = min(minDis, (length(pos - _Blobs[i].xyz) - _REffect[i]));
                }
                return minDis;
            } 

            float Wyvill(float r, float R)//use Wyvill's polynomial for ray marching
            {
                if(r >= R)return 0;
                float t = r / R;
                return ((-4.0f*t*t + 17.0f)*t*t - 22.0f)*t*t / 9.0f + 1.0f;         
            }

            float WyvillDeri(float r, float R)
            {
                if(r >= R)return 0;
                float t = r / R;
                return ((-24.0f*t*t + 68.0f)*t*t - 44.0f)*t / (9.0f * R);   
            }

            float SPF(float3 pos)//get potential field at pos
            {
                float sum = 0;
                for(int i =0; i< _Number; i++)
                {
                    float r = length(pos - _Blobs[i].xyz);//get the distance from pos to the center of ith blob
                    sum += Wyvill(r, _REffect[i]) * _Density[i];
                }
                return sum;
            }

            float4 GetWorldPosition(float2 uv, float depth)//using texcoord and depth to calculate world position.
            {
                float4 viewPos = mul(_InvProjectionM, float4(uv*2-1, depth, 1.0));
                viewPos.xyz /= viewPos.w;

                float4 worldPos = mul(_InvViewM, float4(viewPos.xyz, 1));

                return worldPos;
            }

            float3 GetGradient(float3 pos)//get gradient of field at some point. Gradient is used as normal.
            {
                float3 grad;
                for(int i = 0; i< _Number; i++)
                {
                    float r = length(pos - _Blobs[i].xyz);
                    grad.x += WyvillDeri(r, _REffect[i]) * _Density[i] * (pos.x - _Blobs[i].x) /r;
                    grad.y += WyvillDeri(r, _REffect[i]) * _Density[i] * (pos.y - _Blobs[i].y) /r;
                    grad.z += WyvillDeri(r, _REffect[i]) * _Density[i] * (pos.z - _Blobs[i].z) /r;
                }
                return grad;
            }

            marchHit GetBlendingSurface(float3 pos)//blend color at pos
            {
                marchHit surface;
                surface.color = float4(0,0,0,0);
                surface.roughness = 0;
                surface.position = pos;
                surface.normal = normalize(-GetGradient(pos));
                float sum =0;
                for(int i = 0; i< _Number; i++)
                {
                    float r = length(pos - _Blobs[i].xyz);
                    sum += Wyvill(r, _REffect[i]) * _Density[i];
                    surface.color += _Color[i] * Wyvill(r, _REffect[i]) * _Density[i];
                    surface.roughness += _Roughness[i] * Wyvill(r, _REffect[i]) * _Density[i];
                }
                surface.color /= sum;
                surface.roughness /= sum;
                return surface;
            }

            marchHit RayMarching(float3 start, float3 dir)//dir should be normalized!
            {
                float3 current = start;
                float sd;
                float sp;
                marchHit hit;
                hit.color = float4(1,1,1,1);
                for(int i = 0; i < 64; i++)//if not inside the effective range, march quickly. Otherwise, go slowly.
                {
                    sd = SDF(current);
                    if(sd <= 0){
                        break;
                    }else{
                        current += max(sd, _Accuracy) * dir;
                    }
                }
                if(i >= 64){//if not hit a range: this ray will not hit anything!
                    hit.color.w = 0;
                    return hit;
                };

                for(int i = 0; i <= _Steps; i++)
                {
                    sp = SPF(current);
                    if(sp >= _Threshold){//hit the equipotential surface
                        hit = GetBlendingSurface(current);
                        return hit;
                    }else{
                        current += _Accuracy * dir;
                    }
                }
                //if not hit the equipotential surface 
                hit.color.w = 0;
                return hit;
            }

            float BlinnPhong(float3 normal, float3 view, float roughness)//surface parameters and view vector for spec
            {
                float diffuse;
                float specular;
                diffuse = saturate(dot(normal, _WorldSpaceLightPos0)) * roughness;
                float3 mid = normalize(_WorldSpaceLightPos0 + view);
                specular = pow(saturate(dot(mid, normal)), 2) * (1 - roughness);
                return _Ambient + diffuse + specular;
            }

            v2f vert (appdata i) 
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                return o;
            }

            float4 frag (v2f i ) : SV_Target
            {
                float depth = tex2D(_CameraDepthTexture, i.uv);

                float3 worldPos = GetWorldPosition(i.uv, depth).xyz;
                float3 rayOrigin = _WorldSpaceCameraPos;

                float3 dir = normalize(worldPos - rayOrigin);//get a ray direction.
                
                marchHit hit = RayMarching(rayOrigin, dir);

                if(hit.color.w){//if hitted
                    return BlinnPhong( hit.normal, -dir, hit.roughness) * hit.color * hit.color.w + tex2D(_MainTex, i.uv) * (1 - hit.color.w);
                }else{
                    return tex2D(_MainTex, i.uv);
                }
            }
            ENDCG
        }
    }
}
