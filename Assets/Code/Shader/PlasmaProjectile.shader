Shader "Unlit/PlasmaProjectile"
{
    Properties
    {
        _Color0("Front Color", Color) = (1, 1, 1, 1)
        _Color1("Back Color", Color) = (1, 1, 1, 1)
        _Value("Brightness", float) = 1

        _WAmp("Wobble Amplitude", float) = 0.1
        _WFreq("Wobble Frequency", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 pos : TEXCOORD1;
            };

            float _WAmp;
            float _WFreq;

            v2f vert (appdata v)
            {
                v2f o;
                float s1 = 1 + sin(_Time[1] * _WFreq) * _WAmp;
                float s2 = sqrt(1.0f / s1);
                
                float4 tPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));

                v.vertex = mul(unity_ObjectToWorld, v.vertex);
                v.vertex -= tPos;
                v.vertex *= float4(s2, s1, s2, 1);
                v.vertex += tPos;
                v.vertex = mul(unity_WorldToObject, v.vertex);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos = v.vertex;
                o.uv = v.uv;
                return o;
            }

            float4 _Color0;
            float4 _Color1;
            float _Value;

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = length(i.pos.xyz) > 0.375 ? _Color0 : _Color1;
                
                col *= 1 + _Value * exp(_Value);
                
                return col;
            }
            ENDCG
        }
    }
}
