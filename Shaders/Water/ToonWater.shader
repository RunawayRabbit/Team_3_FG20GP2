Shader "Unlit/ToonWater" {
    Properties {
        _DepthGradientShallow("Depth Gradient Shallow", Color) = (0.3, 0.8, 0.9, 0.75)
        _DepthGradientDeep("Depth Gradient Deep", Color) = (0.08, 0.4, 1, 0.75)
        _FoamColor("Foam Color", Color) = (1, 1, 1, 0.9)
        _DepthMaxDistance("Depth Maximum Distance", float) = 1
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _NoiseAmount("Noise Amount", Range(0,1)) = 0.75
        _SurfaceDistortion("Surface Distortion", 2D) = "white" {}
        _DistortionAmount("Distortion Amount", Range(0,1)) = 0.27
        _FoamDistance("Foam Distance", Float) = 0.4
        _EdgeDistance("Edge Distance", Float) = 0.4
        _SurfaceNoiseScroll("Surface Noise Scroll Amount", Vector) = (0.03, 0.03, 0, 0)
        [noscaleoffset]_VoronoiTex ("Voronoi Texture", 2D) = "white" {}
        _VoronoiAmount ("Voronoi Amount", Range(0,1)) = 1
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
        }

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPosition : TEXCOORD0;
                float2 noiseUV : TEXCOORD1;
                float2 distortUV : TEXCOORD2;
            };

            float4 _DepthGradientShallow;
            float4 _DepthGradientDeep;
            float4 _FoamColor;
            float _DepthMaxDistance;
            sampler2D _CameraDepthTexture;
            sampler2D _VoronoiTex;

            sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;
            float _NoiseAmount;
            float2 _SurfaceNoiseScroll;

            sampler2D _SurfaceDistortion;
            float4 _SurfaceDistortion_ST;
            float _DistortionAmount;

            float _FoamDistance;
            float _EdgeDistance;
            float _VoronoiAmount;

            v2f vert(appdata v)
            {
                v.vertex.xyz += cos(_Time.y*v.uv.x) * float3(0,1,0) * 0.2;
                
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
                o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);

                return o;
            }

            float4 blend(float4 src, float4 dest)
            {
                return src.aaaa * src + (float4(1.0, 1.0, 1.0, 1.0) - src.aaaa) * dest;
            }

            float4 frag(v2f i) : SV_Target
            {
                float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
                float depthLinear = LinearEyeDepth(depth);
                float depthFromSurface = depthLinear - i.screenPosition.w;

                float t = saturate(depthFromSurface / _DepthMaxDistance);
                t = 1 - pow(1 - t, 15);
                float4 waterColour = lerp(_DepthGradientShallow, _DepthGradientDeep, t);

                float foamDepth = saturate(depthFromSurface / _FoamDistance);

                float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _DistortionAmount;

                float2 noiseUV = float2((i.noiseUV.x + _Time.y * 0.5f * _SurfaceNoiseScroll.x) + distortSample.x,
                                        (i.noiseUV.y + _Time.y * _SurfaceNoiseScroll.y) + distortSample.y);
                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV);

                fixed sdf = length(i.noiseUV - fixed2(0.5, 0.5)) + _EdgeDistance;
                float noiseAmount = foamDepth * _NoiseAmount;

                float edgeMask = smoothstep(0.4, 0.5, sdf);
                float surfaceNoise = smoothstep(noiseAmount - 0.1, noiseAmount + 0.1, surfaceNoiseSample + edgeMask);

                float voronoi = tex2D(_VoronoiTex, (i.noiseUV+_Time.y * 0.5f * _SurfaceNoiseScroll.x)+distortSample.x/2) * _VoronoiAmount;
                voronoi = smoothstep(0.1, 0.5, voronoi);
                return lerp(waterColour, _FoamColor, voronoi + surfaceNoise);
                //return blend(waterColour, _FoamColor * surfaceNoise);

                //return lerp(waterColour, _FoamColor, surfaceNoise);
            }
            ENDCG
        }
    }
}
