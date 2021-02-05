Shader "Unlit/Transitioner" {
    Properties {
        _CenterX ("CenterX / CenterY / Radius / Alpha", Float) = 0
        _CenterY ("CenterX / CenterY / Radius / Alpha", Float) = 0
        _Radius ("CenterX / CenterY / Radius / Alpha", Float) = 50
        _Alpha ("CenterX / CenterY / Radius / Alpha", Float) = 0
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed _CenterX;
            fixed _CenterY;
            fixed _Radius;
            fixed _Alpha;

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


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = fixed2(v.uv.x * _ScreenParams.x, v.uv.y * _ScreenParams.y);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed sdf = length(i.uv - fixed2(_CenterX, _CenterY)) - _Radius;
                float ddxy = fwidth(sdf);
                float alpha = smoothstep(-ddxy, ddxy, sdf);
                return fixed4(0, 0, 0, alpha * _Alpha);
            }
            ENDCG
        }
    }
}
