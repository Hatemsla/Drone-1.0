Shader "Custom/UIBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
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
            float _BlurAmount;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float4 texColor = tex2D(_MainTex, i.uv);
                float2 uvOffset = float2(_BlurAmount, _BlurAmount) * i.uv;
                float4 blurredColor = (tex2D(_MainTex, i.uv + uvOffset) +
                                       tex2D(_MainTex, i.uv - uvOffset) +
                                       tex2D(_MainTex, i.uv + float2(uvOffset.x, -uvOffset.y)) +
                                       tex2D(_MainTex, i.uv + float2(-uvOffset.x, uvOffset.y))) * 0.25;
                return texColor * lerp(1, blurredColor, _BlurAmount);
            }
            
            ENDCG
        }
    }
}
