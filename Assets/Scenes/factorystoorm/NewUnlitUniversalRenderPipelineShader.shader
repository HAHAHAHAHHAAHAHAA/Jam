Shader "Custom/TriplanarDirtMulti"
{
    Properties
    {
        [Header(Textures)]
        _MainTex1 ("Dirt Texture 1", 2D) = "white" {}
        _MainTex2 ("Dirt Texture 2", 2D) = "white" {}
        _Color ("Dirt Color", Color) = (1,1,1,1)
        
        [Header(Tiling)]
        _Tiling ("Tiling", Float) = 1.0
        _NoiseScale ("Noise Scale", Float) = 2.0
        
        [Header(Effect)]
        _Intensity ("Effect Intensity", Range(0, 1)) = 0.5
        _BlendSharpness ("Blend Sharpness", Float) = 4.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry+100"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Name "TriplanarDirtMultiPass"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex1);
            SAMPLER(sampler_MainTex1);
            TEXTURE2D(_MainTex2);
            SAMPLER(sampler_MainTex2);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Tiling;
                float _Intensity;
                float _BlendSharpness;
                float _NoiseScale;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                return OUT;
            }

            // Хеш-функция
            float Hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            // Плавная интерполяция
            float SmoothNoise(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f * f * (3.0 - 2.0 * f); // smoothstep
                
                // Сэмплируем 8 углов куба и интерполируем
                return lerp(
                    lerp(lerp(Hash(i), Hash(i + float3(1,0,0)), f.x),
                         lerp(Hash(i + float3(0,1,0)), Hash(i + float3(1,1,0)), f.x), f.y),
                    lerp(lerp(Hash(i + float3(0,0,1)), Hash(i + float3(1,0,1)), f.x),
                         lerp(Hash(i + float3(0,1,1)), Hash(i + float3(1,1,1)), f.x), f.y), f.z
                );
            }

            // Трипланарное сэмплирование
            float4 TriplanarSample(TEXTURE2D(tex), SAMPLER(samplerState), float3 position, float3 normal, float blendSharpness)
            {
                float4 xProj = SAMPLE_TEXTURE2D(tex, samplerState, position.zy * _Tiling);
                float4 yProj = SAMPLE_TEXTURE2D(tex, samplerState, position.xz * _Tiling);
                float4 zProj = SAMPLE_TEXTURE2D(tex, samplerState, position.xy * _Tiling);

                float3 weights = pow(abs(normal), blendSharpness);
                weights = weights / (weights.x + weights.y + weights.z);

                return xProj * weights.x + yProj * weights.y + zProj * weights.z;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                
                // Сэмплируем обе текстуры
                float4 dirt1 = TriplanarSample(_MainTex1, sampler_MainTex1, IN.positionWS, normalWS, _BlendSharpness);
                float4 dirt2 = TriplanarSample(_MainTex2, sampler_MainTex2, IN.positionWS, normalWS, _BlendSharpness);
                
                // Получаем плавный шум (0-1)
                float noise = SmoothNoise(IN.positionWS * _NoiseScale);
                
                // Смешиваем текстуры на основе шума
                float4 dirtSample = lerp(dirt1, dirt2, noise);
                
                float3 finalColor = dirtSample.rgb * _Color.rgb;
                float alpha = dirtSample.a * _Intensity;
                
                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}
