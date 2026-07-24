Shader "Custom/Echolocation2D"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (0, 0, 0, 1)
        _ObjectColor ("Wave Color", Color) = (1, 1, 1, 1)
        _WaveThickness ("Thickness", Range(0.01, 0.5)) = 0.08
        _FadePower ("Softness", Range(0.1, 5.0)) = 2.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "LightMode" = "UniversalForward"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "Unlit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _ObjectColor;
                float _WaveThickness;
                float _FadePower;
            CBUFFER_END
            
            #define MAX_WAVES 8
            uniform float4 _GlobalWavePositions[MAX_WAVES]; 
            uniform float _GlobalWaveAlphas[MAX_WAVES];     
            uniform int _GlobalActiveWavesCount;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                OUT.positionWS = positionInputs.positionWS;
                
                OUT.uv = IN.uv; 
                OUT.color = IN.color;
                
                return OUT;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                if (texColor.a < 0.01)
                    discard;

                float totalWaveIntensity = 0.0;

                for (int i = 0; i < _GlobalActiveWavesCount; i++)
                {
                    float3 waveCenter = _GlobalWavePositions[i].xyz;
                    float currentRadius = _GlobalWavePositions[i].w;
                    float waveAlpha = _GlobalWaveAlphas[i];

                    float dist = distance(input.positionWS.xy, waveCenter.xy);
                    float ringDist = abs(dist - currentRadius);

                    if (ringDist < _WaveThickness)
                    {
                        float ringIntensity = 1.0 - (ringDist / _WaveThickness);
                        ringIntensity = pow(ringIntensity, _FadePower);
                        totalWaveIntensity += ringIntensity * waveAlpha;
                    }
                }

                totalWaveIntensity = saturate(totalWaveIntensity);

                half4 color = lerp(_BaseColor, _ObjectColor * texColor, totalWaveIntensity);
                color.a *= texColor.a * input.color.a;

                return color;
            }
            ENDHLSL
        }
    }
}