Shader "Custom/GlitchRing"
{
    Properties
    {
        [HideInInspector] _MainTex("Sprite Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _Center("Glitch Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius("Glitch Radius", Float) = 0.0
        _Intensity("Glitch Intensity", Float) = 0.0
        _Aspect("Screen Aspect Ratio", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Cull Off ZWrite Off ZTest [ALW] Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _Center;
                float _Radius;
                float _Intensity;
                float _Aspect;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float2 aspectCorrectedUV = uv * float2(_Aspect, 1.0);
                float2 aspectCorrectedCenter = _Center.xy * float2(_Aspect, 1.0);

                float dist = distance(aspectCorrectedUV, aspectCorrectedCenter);
                float wave = smoothstep(_Radius - 0.08, _Radius, dist) * smoothstep(_Radius + 0.08, _Radius, dist);
                
                uv.x += sin(uv.y * 60.0 + _Time.y * 10.0) * wave * _Intensity * 0.04;
                uv.y += cos(uv.x * 60.0 + _Time.y * 10.0) * wave * _Intensity * 0.04;

                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _BaseColor;
            }
            ENDHLSL
        }
    }
}
