Shader "Custom/GridWaveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Rows ("Rows", Int) = 12
        _Columns ("Columns", Int) = 8
        _CustomTime ("Custom Time", Float) = 0
        _Amplitude ("Amplitude", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _Rows;
            int _Columns;
            float _CustomTime; // Renamed from _Time to avoid conflict
            float _Amplitude;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate grid position
                float2 gridPos = floor(i.uv * float2(_Columns, _Rows));
                // Calculate wave effect
                float wave = sin(_CustomTime + (gridPos.x + gridPos.y) * _Amplitude) * 0.5 + 0.5;
                
                fixed4 col = tex2D(_MainTex, i.uv); // Sample the texture
                col.a *= wave; // Modify alpha based on wave effect
                
                return col; // Return the final color
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
}