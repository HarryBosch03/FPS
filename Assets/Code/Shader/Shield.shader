Shader "Unlit/Shield"
{
    Properties
    {
        _NScale ("Noise Scale", Vector) = (10, 10, 0, 0)
        _DAmp ("Distortion Amplitude", float) = 0
        _FPow ("Fresnel Power", float) = 2
        _FStren ("Fresnel Strength", float) = 0
        _NPow ("Noise Power", float) = 2
        _NVis ("Noise Visibility", float) = 0
        _NDist ("Noise Distortion", Range(0, 1)) = 0
        _SSpeed ("Scroll Speed", float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
       
        Pass
        {
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off
            
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #define REQUIRE_OPAQUE_TEXTURE
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 pos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float4 normal : NORMAL;
                float4 viewDir : TEXCOORD3;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                o.pos = mul(unity_ObjectToWorld, v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.normal = normalize(mul(unity_ObjectToWorld, float4(v.normal.xyz, 0)));
                o.viewDir = float4(normalize(_WorldSpaceCameraPos.xyz - o.pos), 0);
                return o;
            }

            uint rand (const int x, const uint seed = 0)
            {
                uint m = x;
                m *= 0xB5297A4D;
                m += seed;
                m ^= (m >> 8);
                m += 0x68E31DA4;
                m ^= (m << 8);
                m *= 0x1B56C4E9;
                m ^= (m >> 8);
                return m;
            }
            
            uint rand (const int3 p, const uint seed = 0)
            {
                return rand(p.x + p.y * 198491317 + p.z * 6542989, seed);
            }
            
            float rand_f (const int x, const uint seed = 0)
            {
                return rand(x, seed) / 4294967295.0f;
            }

            float rand_f (const int3 p, const uint seed = 0)
            {
                return rand(p, seed) / 4294967295.0f;
            }

            float3 random_direction (const int3 p0, const uint seed = 0)
            {
                float2 r = float2(rand_f(p0, seed), rand_f(p0, ~seed)) * 2.0f * PI;
                return float3(cos(r.x), sin(r.x), 0.0f) * cos(r.y) + float3(0.0f, 0.0f, sin(r.y));
            }
            
            float dot_grid_gradient (const int3 p0, const float3 p, const uint seed = 0)
            {
                const float3 direction = random_direction(p0, seed);
                const float3 dp = p - (float3)p0;
                return dot(dp, direction) * 0.5f + 0.5f;
            }

            float interpolate (const float a, const float b, const float t)
            {
                const float s = t * t * t * (t * (t * 6 - 15) + 10);
                return lerp(a, b, s);
            }
            
            float noise (const float3 p, const uint seed = 0)
            {
                const int3 p0 = floor(p);
                float3 pd = p - (float3)p0;

                float2 i = 0;
                float2x4 o = 0;

                i[0] = dot_grid_gradient(p0 + int3(0, 0, 0), p, seed);
                i[1] = dot_grid_gradient(p0 + int3(1, 0, 0), p, seed);
                o[0][0] = interpolate(i[0], i[1], pd.x);
                
                i[0] = dot_grid_gradient(p0 + int3(0, 1, 0), p, seed);
                i[1] = dot_grid_gradient(p0 + int3(1, 1, 0), p, seed);
                o[0][1] = interpolate(i[0], i[1], pd.x);
                
                i[0] = dot_grid_gradient(p0 + int3(0, 0, 1), p, seed);
                i[1] = dot_grid_gradient(p0 + int3(1, 0, 1), p, seed);
                o[0][2] = interpolate(i[0], i[1], pd.x);
                
                i[0] = dot_grid_gradient(p0 + int3(0, 1, 1), p, seed);
                i[1] = dot_grid_gradient(p0 + int3(1, 1, 1), p, seed);
                o[0][3] = interpolate(i[0], i[1], pd.x);

                o[1][0] = interpolate(o[0][0], o[0][1], pd.y);
                o[1][1] = interpolate(o[0][2], o[0][3], pd.y);

                return clamp(interpolate(o[1][0], o[1][1], pd.z), 0, 1);
            }

            float3 scene_color(float2 uv)
            {
                float3 res = SHADERGRAPH_SAMPLE_SCENE_COLOR(uv);
                return res;
            }

            float2 _NScale;
            float _DAmp;
            float _FPow;
            float _FStren;
            float _NPow;
            float _NVis;
            float _NDist;
            float _SSpeed;

            float bigNoise (const float3 p, const uint seed = 0)
            {
                float n = 0.0;
                float m = 0.0f;
                for (int i = 0; i < 4; i++)
                {
                    float f = pow(2.0, i);
                    float a = pow(0.5, i);

                    n += noise(p * f, seed + i) * a;
                    m += a;
                }
                return n / m;
            }
            
            float4 frag (v2f i) : SV_Target
            {

                float3 n0 = float3(bigNoise(i.pos * _NScale.x), bigNoise(i.pos * _NScale.x, 1), bigNoise(i.pos * _NScale.x, 2)) * 2.0f - 1.0f;
                float3 nCoords = lerp(i.pos, n0, _NDist);
                float2 n1 = float2(bigNoise(nCoords * _NScale.y, 0), bigNoise(nCoords * _NScale.y, 1));
                n1 = sin(n1 * 100 + _Time.x * _SSpeed) * 0.5f + 0.5f;

                float3 col = scene_color((i.screenPos.xy / i.screenPos.w) + float4(n1, 0, 0) * _DAmp).rgb;

                float fresnel = pow(max(1 - dot(i.normal, i.viewDir), 0), _FPow) * _FStren;
                col += fresnel;
                
                col += max(pow(n1.x * 0.5f + 0.5f, _NPow) * _NVis, 0) * fresnel;
                
                return float4(col, 1.0f);
            }
            ENDHLSL
        }
    }
}
