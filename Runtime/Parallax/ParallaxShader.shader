Shader "AurecasLib/Parallax/ParallaxShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Factor ("Factor", Float) = 0
        _Direction("Direction", Vector) = (1, 1, 0, 0)
        _Offset("Offset", Vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Factor;
            float2 _Direction;
            float2 _Offset;

            fixed2 WorldToScreenPos(fixed3 pos) {
                pos = normalize(pos - _WorldSpaceCameraPos) * (_ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y)) + _WorldSpaceCameraPos;
                fixed2 uv = 0;
                fixed3 toCam = mul(unity_WorldToCamera, pos);
                fixed camPosZ = toCam.z;
                fixed height = 2 * camPosZ / unity_CameraProjection._m11;
                fixed width = _ScreenParams.x / _ScreenParams.y * height;
                uv.x = (toCam.x + width / 2) / width;
                uv.y = (toCam.y + height / 2) / height;
                return uv;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 scp = WorldToScreenPos(float3(0, 0, 0)) *10;

                fixed4 col = tex2D(_MainTex, (i.uv + _Offset) - scp * _Factor * _Direction);
                return col * i.color;
                //return float4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
}
