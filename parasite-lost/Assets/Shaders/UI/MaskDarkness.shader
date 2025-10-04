Shader "UI/MaskDarkness"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,0.95)
        _Center ("Center", Vector) = (0.5,0.5,0,0)
        _RadiusX ("RadiusX", Float) = 0.2
        _RadiusY ("RadiusY", Float) = 0.2
        _Softness ("Softness", Float) = 0.02
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            fixed4 _Color;
            float4 _Center;
            float _RadiusX;
            float _RadiusY;
            float _Softness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float rx = max(0.00001, _RadiusX);
                float ry = max(0.00001, _RadiusY);
                float dx = (uv.x - _Center.x) / rx;
                float dy = (uv.y - _Center.y) / ry;
                float d = sqrt(dx * dx + dy * dy);

                float s = saturate(_Softness);
                float edgeStart = 1.0 - s;
                float t = smoothstep(edgeStart, 1.0, d);
                float outA = _Color.a * t;

                return fixed4(_Color.rgb, outA);
            }
            ENDCG
        }
    }
}
