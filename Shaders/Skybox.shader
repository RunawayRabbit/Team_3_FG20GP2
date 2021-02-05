Shader "ejtb/Skybox" {

    Properties {
        [Header(Sky)]
        [Space(7)]
        _SkyTopColor ("Sky Color", Color) = (.5, .5, .5, 1)
        _SkyBotColor ("Horizon Color", Color) = (.5, .5, .5, 1)
        _SkyGradientHeight ("Sky Gradient Height", Range(0,1)) = 0.2
        _SkyGradientSoftness ("Sky Hardness", Range(0,1)) = 0.2

        _StarColor("Star color", Color) = (1.0, 1.0, 1.0, 1.0)
        _StarSizes("Star size range", Vector) = (0.6, 0.9, 1.1, 1.5)
        _StarBrightness("Star Brightness", float) = 2

        [Space(7)]
        [Header(Moon)]
        [Space(7)]

        [NoScaleOffset] _MoonTex ("Moon", 2D) = "black" {}
        _MoonColor ("Tint", Color) = (.5, .5, .5, 1)
        _MoonShadow ("Shadow Strength", Range(0.00,0.05)) = 0.01
        _MoonShadowRot ("Moon Shadow Rotation", Range(-3.14159, 3.14159)) = 0
        _MoonSize ("Size", Range(0,1)) = 0.04
        _MoonGlow ("Glow Strength", Range(0.1, 2)) = 0.8
        _MoonRadius ("Glow Radius", Range(0.1, 2)) = 0.8
        _MoonPhase ("MoonPhase", Range(0.0, 3)) = 0.5

        [Space(7)]
        [Header(Noise)]
        [Space(7)]
        [NoScaleOffset] _NoiseMap ("Noise", 2D) = "grey" {}
        _ShimmerRate ("Shimmer Rate", float) = 0.2
        _StarTwinkleness ("Star Twinkleness", float) = 2

        [Space(7)]
        [Header(Zodiac)]
        [Space(7)]

        [NoScaleOffset] _ZodiacTex ("Zodiac Texture", 2D) = "black" {}
        _ZodiacSize ("Size", Range(0,1)) = 0.04
        [Space(7)]

        _Zodiac0 ("Zodiac 0 Position", Vector) = (1,0,0,1)
        _Zodiac1 ("Zodiac 1 Position", Vector) = (1,0,0,1)
        _Zodiac2 ("Zodiac 2 Position", Vector) = (1,0,0,1)
        _Zodiac3 ("Zodiac 3 Position", Vector) = (1,0,0,1)
        _Zodiac4 ("Zodiac 4 Position", Vector) = (1,0,0,1)
        _Zodiac5 ("Zodiac 5 Position", Vector) = (1,0,0,1)
        _Zodiac6 ("Zodiac 6 Position", Vector) = (1,0,0,1)
        _Zodiac7 ("Zodiac 7 Position", Vector) = (1,0,0,1)
        _Zodiac8 ("Zodiac 8 Position", Vector) = (1,0,0,1)
        _Zodiac9 ("Zodiac 9 Position", Vector) = (1,0,0,1)
        _Zodiac10 ("Zodiac 10 Position", Vector) = (1,0,0,1)
        _Zodiac11 ("Zodiac 11 Position", Vector) = (1,0,0,1)
        _Zodiac12 ("Zodiac 12 Position", Vector) = (1,0,0,1)
        _Zodiac13 ("Zodiac 13 Position", Vector) = (1,0,0,1)
        _Zodiac14 ("Zodiac 14 Position", Vector) = (1,0,0,1)
        _Zodiac15 ("Zodiac 15 Position", Vector) = (1,0,0,1)

    }

    SubShader {
        Tags {
            "Queue"="Background"
            "RenderType"="Background"
            "PreviewType"="Skybox"
        }
        Cull Off ZTest On

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define TAU 6.28318530718

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            uniform half3 _SkyTopColor;
            uniform half3 _SkyBotColor;
            uniform float _SkyGradientSoftness;
            uniform float _SkyGradientHeight;

            sampler2D _NoiseMap;
            uniform float _ShimmerRate;
            uniform float _StarTwinkleness;

            //moon
            sampler2D _MoonTex;
            uniform half3 _MoonColor;
            uniform half _MoonShadow;
            uniform half _MoonSize;
            uniform half _MoonGlow;
            uniform half _MoonRadius;
            uniform half _MoonPhase;
            uniform half _MoonShadowRot;

            //stars
            uniform half4 _StarSizes;
            uniform half4 _StarColor;
            uniform float _StarBrightness;
            uniform float _Star1;
            uniform float _Star2;
            uniform float _Star3;

            //zodiac
            sampler2D _ZodiacTex;
            uniform half _ZodiacSize;

            // Apparently Unity doesn't serialize vector arrays..?
            uniform float4 _Zodiac0;
            uniform float4 _Zodiac1;
            uniform float4 _Zodiac2;
            uniform float4 _Zodiac3;
            uniform float4 _Zodiac4;
            uniform float4 _Zodiac5;
            uniform float4 _Zodiac6;
            uniform float4 _Zodiac7;
            uniform float4 _Zodiac8;
            uniform float4 _Zodiac9;
            uniform float4 _Zodiac10;
            uniform float4 _Zodiac11;
            uniform float4 _Zodiac12;
            uniform float4 _Zodiac13;
            uniform float4 _Zodiac14;
            uniform float4 _Zodiac15;


            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 rayDir : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };


            v2f vert(appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.rayDir = mul(unity_ObjectToWorld, v.vertex);

                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }


            float4 blend(float4 src, float4 dest)
            {
                return src.aaaa * src + (float4(1.0, 1.0, 1.0, 1.0) - src.aaaa) * dest;
            }

            float4 MoonColor(half3 ray)
            {
                float3 moonZ = _WorldSpaceLightPos0.xyz;
                float3 moonX = -normalize(cross(moonZ, float3(0.0, 1.0, 0.0)));
                float3 moonY = -normalize(cross(moonX, moonZ));

                float3x3 moonMatrix = float3x3(moonX, moonY, moonZ) / _MoonSize;

                float3 transformedRay = mul(moonMatrix, ray);
                float3 moonUV = transformedRay + float3(0.5, 0.5, 0.0);

                float moonCos = cos(_MoonShadowRot);
                float moonSin = sin(_MoonShadowRot);
                float2 moonPhaseUV = float2(
                    transformedRay.x * moonCos - transformedRay.y * moonSin,
                    transformedRay.x * moonSin + transformedRay.y * moonCos);

                float4 moonCol = float4(0, 0, 0, 0);
                if (moonUV.z > 0.0)
                {
                    float moonDist = length(moonUV.xy - 0.5);
                    float moonGlow = smoothstep(2.2 * _MoonRadius, 0, moonDist) * _MoonGlow;
                    moonCol += float4(moonGlow * _MoonColor.xyz, moonGlow);

                    if (moonUV.x > 0.0 && moonUV.x < 1.0 &&
                        moonUV.y > 0.0 && moonUV.y < 1.0)
                    {
                        float phaseDot = dot(moonPhaseUV, moonPhaseUV);
                        float circle = (2.0 - sqrt(1.0 - (3.0 * phaseDot))) / (1.0 + phaseDot);

                        float3 fakeNormal = float3((circle * moonPhaseUV), circle - 2.0f);
                        float3 fakeLight = float3(cos(_MoonPhase * TAU), 0, sin(_MoonPhase * TAU));

                        float lambert = saturate(1 - dot(fakeNormal, fakeLight));

                        float t = 1 - pow(1 - lambert, 1.5);
                        //float t = lambert;
                        float moonShadow = _MoonShadow * 0.03;

                        float3 tint = lerp(_MoonColor.xyz, moonShadow.xxx, t);
                        float4 moonSample = tex2D(_MoonTex, moonUV.xy) * float4(tint, 1);
                        moonCol = blend(saturate(pow(moonSample, 0.45)), moonCol);
                    }
                }
                return moonCol;
            }

            float3x3 GetZodiacMatrix(half3 dir)
            {
                float3 zodiacZ = normalize(dir);
                float3 zodiacX = -normalize(cross(zodiacZ, float3(0.0, 1.0, 0.0)));
                float3 zodiacY = -normalize(cross(zodiacX, zodiacZ));

                return float3x3(zodiacX, zodiacY, zodiacZ) / _ZodiacSize;
            }

            float4 DrawZodiac(half3 ray, half4 zodiacDir, int i)
            {
                if (zodiacDir.w == 0.0 || dot(-zodiacDir.xyz, ray) < 0.9) return float4(0, 0, 0, 0);

                float3x3 zodiacMatrix = GetZodiacMatrix(-zodiacDir);
                float3 zodiacUV = mul(zodiacMatrix, ray) + float3(0.5, 0.5, 0.0);

                if (zodiacUV.x < 0 || zodiacUV.x > 1 ||
                    zodiacUV.y < 0 || zodiacUV.y > 1)
                {
                    return float4(0, 0, 0, 0);
                }

                int col = i % 4;
                int row = i / 4;

                zodiacUV.x = (saturate(zodiacUV.x) * 0.25) + (0.25 * col);
                zodiacUV.y = (saturate(zodiacUV.y) * 0.25) + (0.25 * row);

                float4 tex = tex2D(_ZodiacTex, zodiacUV.xy);

                return tex * tex.a * zodiacDir.a;
            }

            float2 hash(float2 x)
            {
                return frac((sin(mul(x, float2x2(1.543, 0.692, 1.603, 74.24))) * 2.673) * 5.512);
            }

            float StarPass(float3 ray, float sphereRadius, float size)
            {
                float3 spherePoint = ray * sphereRadius;

                float upAtan = atan2(spherePoint.y, length(spherePoint.zx)) + 2 * TAU;

                float oneOverSphereRadius = 1.0 / sphereRadius;

                //Uniform size regardless of resolution
                float starSize = (sphereRadius * 0.0018) * 1000.0 * size * fwidth(upAtan);
                upAtan -= fmod(upAtan, oneOverSphereRadius) - oneOverSphereRadius * 0.5;

                float numberOfStars = floor(sqrt(pow(sphereRadius, 1.4) * (1.0 - pow(sin(upAtan), 2.0))) * 3.0);

                float planeAngle = atan2(spherePoint.z, spherePoint.x) + 2.0 * TAU;
                planeAngle = planeAngle - fmod(planeAngle, TAU / numberOfStars * 0.5);

                float2 randomPosition = hash(float2(planeAngle, upAtan));

                float starAngle = planeAngle + (TAU * (randomPosition.x * (1.0 - starSize)) / numberOfStars);
                float starLevel = sin(upAtan + oneOverSphereRadius * (randomPosition.y - 0.5) * (1.0 - starSize)) * sphereRadius;

                float starDistanceToYAxis = sqrt(sphereRadius * sphereRadius - starLevel * starLevel);
                float3 starCenter = float3(cos(starAngle) * starDistanceToYAxis, starLevel, sin(starAngle) * starDistanceToYAxis);

                float star = smoothstep(starSize, 0.0, distance(starCenter, spherePoint));

                return star;
            }


            float3x3 GetRotMatrix(float angle)
            {
                float3 axis = _WorldSpaceLightPos0.xyz;
                angle *= TAU;
                float s = sin(angle);
                float c = cos(angle);
                float oneMinusC = 1 - c;
                return
                    float3x3(oneMinusC * axis.x * axis.x + c, oneMinusC * axis.x * axis.y - axis.z * s,
                             oneMinusC * axis.z * axis.x + axis.y * s,
                             oneMinusC * axis.x * axis.y + axis.z * s, oneMinusC * axis.y * axis.y + c,
                             oneMinusC * axis.y * axis.z - axis.x * s,
                             oneMinusC * axis.z * axis.x - axis.y * s, oneMinusC * axis.y * axis.z + axis.x * s,
                             oneMinusC * axis.z * axis.z + c
                    );
            }


            float StarColor(float3 rayDir)
            {
                float starColor = 0.0;

                starColor += StarPass(rayDir, 2 * pow(4.3, 1), _StarSizes.x) * (1.0 / pow(3.4, 1));
                starColor += StarPass(rayDir, 2 * pow(3.3, 2), _StarSizes.y) * (1.0 / pow(3.3, 2));
                starColor += StarPass(rayDir, 2 * pow(2.3, 3), _StarSizes.z) * (1.0 / pow(3.2, 3));
                starColor += StarPass(rayDir, 2 * pow(1.3, 4), _StarSizes.w) * (1.0 / pow(3.1, 4));
                starColor += StarPass(rayDir, 2 * pow(1.3, 5), _StarSizes.x) * (1.0 / pow(3.1, 5));

                return starColor * _StarBrightness;
            }

            float4 RenderAllZodiacs(float3 rayDir)
            {
                float4 zodiacColor = float4(0, 0, 0, 0);

                #if 0
                return zodiacColor;
                #endif
                zodiacColor += DrawZodiac(rayDir, _Zodiac0, 0);
                zodiacColor += DrawZodiac(rayDir, _Zodiac1, 1);
                zodiacColor += DrawZodiac(rayDir, _Zodiac2, 2);
                zodiacColor += DrawZodiac(rayDir, _Zodiac3, 3);
                zodiacColor += DrawZodiac(rayDir, _Zodiac4, 4);
                zodiacColor += DrawZodiac(rayDir, _Zodiac5, 5);
                zodiacColor += DrawZodiac(rayDir, _Zodiac6, 6);
                zodiacColor += DrawZodiac(rayDir, _Zodiac7, 7);
                zodiacColor += DrawZodiac(rayDir, _Zodiac8, 8);
                zodiacColor += DrawZodiac(rayDir, _Zodiac9, 9);
                zodiacColor += DrawZodiac(rayDir, _Zodiac10, 10);
                zodiacColor += DrawZodiac(rayDir, _Zodiac11, 11);
                zodiacColor += DrawZodiac(rayDir, _Zodiac12, 12);
                zodiacColor += DrawZodiac(rayDir, _Zodiac13, 13);
                zodiacColor += DrawZodiac(rayDir, _Zodiac14, 14);
                zodiacColor += DrawZodiac(rayDir, _Zodiac15, 15);

                return zodiacColor;
            }

            float2 Shimmer(float4 screenPos)
            {
                float2 screenP = screenPos.xy/ screenPos.w;

                float shimmer1 = tex2D(_NoiseMap, screenP + _CosTime.x * _ShimmerRate * 0.05) - 0.5;
                float shimmer2 = tex2D(_NoiseMap, screenP*2 + _SinTime.x * _ShimmerRate) - 0.5;

                return float2(shimmer1, shimmer2);
            }


            float4 frag(v2f i) : SV_Target
            {
                float2 shimmers =  Shimmer(i.screenPos);

                float3 rayDir = normalize(i.rayDir - _WorldSpaceCameraPos);

                float4 moonColor = MoonColor(rayDir);
                float4 zodiacColor = RenderAllZodiacs(rayDir) * (0.8 + (shimmers.y * 0.4));
                float starShimmer = (1 + _StarTwinkleness * shimmers.y);
                float4 starColor = StarColor(rayDir) * starShimmer * _StarColor;

                float3 skyColor = _SkyTopColor;
                float3 lowerSkyColor = _SkyBotColor * (1 + 0.7 * shimmers.x);

                skyColor += lowerSkyColor * saturate((-rayDir.y * _SkyGradientSoftness) + _SkyGradientHeight);

                float4 background = float4(skyColor + starColor, 1);

                float4 col = blend(zodiacColor,background );
                col = saturate(blend(moonColor, col));
                return col;
            }
            ENDCG
        }
    }

    Fallback Off
}
