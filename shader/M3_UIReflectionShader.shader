Shader "M3/UI/ReflectionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ReflectStrength ("Reflect Strength", Range(0, 1)) = 0.5
        _GradientStart ("Gradient Start", Range(0, 1)) = 0.3
        _GradientStrength ("Gradient Strength", Range(0, 2)) = 0.5
        _ReverseX ("Reverse X", Float) = 0
        _ReverseY ("Reverse Y", Float) = 0
        // required for UI.Mask
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            // required for UI.Mask
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            ColorMask[_ColorMask]
            ZWrite Off
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

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
            float _ReflectStrength;
            float _GradientStart;
            float _GradientStrength;
            float _ReverseX;
            float _ReverseY;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Reverse X and Y handling
                float2 reflectUv = i.uv;
                reflectUv.x = _ReverseX > 0.5 ? 1.0 - reflectUv.x : reflectUv.x; // Flip X if _ReverseX is enabled
                reflectUv.y = _ReverseY > 0.5 ? 1.0 - reflectUv.y : reflectUv.y; // Flip Y if _ReverseY is enabled

                fixed4 reflectColor = tex2D(_MainTex, reflectUv);

                // Gradient effect
                float gradient = smoothstep(_GradientStart, _GradientStart + _GradientStrength, i.uv.y);
                reflectColor *= gradient;

                return reflectColor * _ReflectStrength;
            }
            ENDCG
        }
    }
}
