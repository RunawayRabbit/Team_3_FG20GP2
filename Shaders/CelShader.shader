// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Toon/CelShader" {
    Properties {
        _Tint("Color/Tint", color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]_NormalMap("Normals", 2D) = "bump" {}
        _NormalIntensity("Normal Intensity", Range(0,1)) = 1
        _AmbientColorOverride("Ambient Light Override", color) = (0,0,0,0) //black, no ambient light
        [Header(For lower values consider removing shadow casting on the mesh renderer)][Space(10)]
        _ShadowIntensity("Shadow Intensity", Range(0,1)) = 1
        _ShadowStart("Shadow start fade", Range(-1,1)) = 0
        _ShadowEnd("Shadow end fade", Range(-1,1)) = 0.02
        _Glossiness("Glossiness", Range(0,1)) = 0
        _SpecularIntensity("Specular Intensity", Range(0,1)) = 1
        _SpecularStart("Specular start fade", Range(0,1)) = 0.005
        _SpecularEnd("Specular End fade", Range(0,1)) = 0.01
        _RimStart("Rim start fade", Range(0,1)) = 0.7
        _RimEnd("Rim End fade", Range(0,1)) = 0.8
        _RimIntensity("Rim Intensity",Range(0,1)) = 0.8
        [Header(Consider turning to 0 for objects with flat surfaces)][Space(5)]_RimSensitivity("Rim Sensitivity", Range(0,1)) = 0.1
        _OutlineWidth("Outline Width", Range(0,20)) = 0.01
        _OutlineColor("Outline Color", color) = (0,0,0,0)
        [Header(Outlines may look bad on hard edges)]
        [Header(This can be countered by baking smooth normals into the vertex colors)]
        [Space(10)]
        [Toggle]_BakedNormals("Model has smooth normals baked in vertex colours", Range(0,1)) = 0
        [Toggle]_EqualDistance("Lines are equally thick at different distances", Range(0,1)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }

        //Directional Light Pass

        Pass {
            Tags {
                "LightMode" = "ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 tangent : TEXCOORD3;
                float3 bitangent : TEXCOORD4;

                SHADOW_COORDS(5)
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            float4 _MainTex_ST;
            float4 _AmbientColor;
            float4 _AmbientColorOverride;
            float4 _Tint;
            float _NormalIntensity;
            float _Glossiness;
            float _SpecularIntensity;
            float _RimStart;
            float _RimEnd;
            float _RimIntensity;
            float _RimSensitivity;
            float _ShadowStart;
            float _ShadowEnd;
            float _SpecularStart;
            float _SpecularEnd;
            float _ShadowIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewDir = WorldSpaceViewDir(v.vertex); //direction towards camera
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.bitangent = cross(o.normal, o.tangent);
                o.bitangent *= v.tangent.w * unity_WorldTransformParams.w;

                TRANSFER_VERTEX_TO_FRAGMENT(o);
                TRANSFER_SHADOW(o)
                return o;
            }

            float InverseLerp(float a, float b, float v)
            {
                return (v - a) / (b - a);
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalMap, i.uv));
                tangentSpaceNormal = lerp(float3(0, 0, 1), tangentSpaceNormal, _NormalIntensity);

                float3x3 mtxTangToWorld = {
                    i.tangent.x, i.bitangent.x, i.normal.x,
                    i.tangent.y, i.bitangent.y, i.normal.y,
                    i.tangent.z, i.bitangent.z, i.normal.z
                };

                float4 ambient = _AmbientColorOverride.a > 0 ? _AmbientColorOverride : _AmbientColor;

                float3 V = normalize(i.viewDir);
                float3 H = normalize(V + _WorldSpaceLightPos0);
                float3 N = normalize(mul(mtxTangToWorld, tangentSpaceNormal)); //normal may not be normalized when interpolated

                float attenuation = LIGHT_ATTENUATION(i);

                //diffuse lighting
                float lambert = dot(normalize(_WorldSpaceLightPos0), N) * attenuation; //determine where shadow begins

                //float shadow = SHADOW_ATTENUATION(i);
                float t = InverseLerp(_ShadowStart, _ShadowEnd, lambert); //make less sharp shadow edges for round objects

                float lightIntensity = smoothstep(0, 1, t) + (1 - _ShadowIntensity); // * shadow;
                float4 diffuseLight = saturate(lightIntensity * _LightColor0);

                //specular light
                float gloss = exp2(_Glossiness * 11) + 2; //makes the slider more impactful
                float blinnPhongSpecular = saturate(dot(H, N));
                float3 specularLight = pow(blinnPhongSpecular, gloss) * (lambert > 0);
                specularLight = saturate(smoothstep(_SpecularStart, _SpecularEnd, specularLight) * _SpecularIntensity);
                specularLight *= _LightColor0 * attenuation;

                //Rim highlight
                float3 rim = 1 - dot(V, N); //fresnel to get rim effect
                rim *= pow(lambert, _RimSensitivity);
                rim = smoothstep(_RimStart, _RimEnd, rim); //set edges of rim
                rim *= _RimIntensity * _LightColor0; //apply color to rim

                // sample the texture
                float3 col = tex2D(_MainTex, i.uv);

                return saturate(float4(saturate(col * _Tint) * (diffuseLight + rim + specularLight + (ambient * (1.0f - _ShadowIntensity))), 1));
            }
            ENDCG
        }

        //Point & spotlight Pass
        Pass {
            Blend One One
            Tags {
                "LightMode" = "ForwardAdd"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 viewDir : TEXCOORD1;
                float3 wPos : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float3 tangent : TEXCOORD4;
                float3 bitangent : TEXCOORD5;

                LIGHTING_COORDS(6, 7)
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            float _NormalIntensity;
            float4 _MainTex_ST;
            float4 _Tint;
            float _Glossiness;
            float _SpecularIntensity;
            float _ShadowStart;
            float _ShadowEnd;
            float _SpecularStart;
            float _SpecularEnd;
            float4 _AmbientColor;
            float4 _AmbientColorOverride;
            float _ShadowIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewDir = WorldSpaceViewDir(v.vertex); //direction towards camera
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.bitangent = cross(o.normal, o.tangent);
                o.bitangent *= v.tangent.w * unity_WorldTransformParams.w;

                TRANSFER_VERTEX_TO_FRAGMENT(o); //multiple lighting coords
                TRANSFER_SHADOW(o)
                return o;
            }

            float InverseLerp(float a, float b, float v)
            {
                return (v - a) / (b - a);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 tangentSpaceNormal = UnpackNormal(tex2D(_NormalMap, i.uv));
                tangentSpaceNormal = lerp(float3(0, 0, 1), tangentSpaceNormal, _NormalIntensity);

                float3x3 mtxTangToWorld = {
                    i.tangent.x, i.bitangent.x, i.normal.x,
                    i.tangent.y, i.bitangent.y, i.normal.y,
                    i.tangent.z, i.bitangent.z, i.normal.z
                };

                float3 N = normalize(mul(mtxTangToWorld, tangentSpaceNormal));

                float3 L = normalize(UnityWorldSpaceLightDir(i.wPos));
                float3 V = normalize(i.viewDir);
                float3 H = normalize(V + L);

                float attenuation = LIGHT_ATTENUATION(i);

                //diffuse lighting
                float lambert = dot(L, N) * attenuation; //determine where shadow begins

                float t = InverseLerp(_ShadowStart, _ShadowEnd, lambert); //make less sharp shadow edges for round objects
                t = saturate(t);


                float lightIntensity = saturate((smoothstep(0, 1, t) * attenuation)); // + 1-(_ShadowIntensity)); // * shadow;
                //return float4(lightIntensity.xxx, 1.0f);

                float4 diffuseLight = lightIntensity * _LightColor0;

                //specular light
                float gloss = exp2(_Glossiness * 11) + 2; //makes the slider more impactful
                float blinnPhongSpecular = saturate(dot(H, N));
                float3 specularLight = pow(blinnPhongSpecular, gloss) * (lambert > 0); // * attenuation;
                specularLight = saturate(smoothstep(_SpecularStart, _SpecularEnd, specularLight) * _SpecularIntensity);
                specularLight *= _LightColor0;

                // sample the texture
                float3 col = tex2D(_MainTex, i.uv);

                return float4(saturate(col * _Tint * diffuseLight) + saturate(specularLight), 1);
            }
            ENDCG
        }

        //Outline pass
        Pass {
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 pos : POSITION;
                float3 normal : NORMAL;
                float3 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;
            float _BakedNormals;
            float _EqualDistance;

            v2f vert(appdata v)
            {
                bool useBaked = _BakedNormals > 0.5;
                bool equallyThick = _EqualDistance > 0.5;

                v2f o;

                v.color.xyz = v.color.zyx;
                v.color = (v.color) * 2 - 1; //remap from bake
                //v.color = Rotate(float3(0,1,0), -90, v.color);//rotate into proper coordinate system
                v.color = float3(-v.color.z, v.color.y, v.color.x);
                normalize(v.color);

                float3 normal = v.normal * (1 - useBaked) + v.color * useBaked;

                if (equallyThick)
                {
                    float4 clipPos = UnityObjectToClipPos(v.pos);
                    float3 clipNormal = mul(unity_MatrixMVP, normal);
                    float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _OutlineWidth * clipPos.w * 2;
                    clipPos.xy += offset;

                    o.vertex = clipPos;
                }
                else
                {
                    v.pos.xyz += normal * _OutlineWidth;
                    float4 clipPos = UnityObjectToClipPos(v.pos);
                    o.vertex = clipPos;
                }

                return o;
            }

            fixed4 frag(v2f o) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

    }
    CustomEditor "ToonShaderGUI"
}
