Shader "Custom/Unlit/GoalEffect"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (1, 1, 1, 1)
		
		_LinePattern("Line Pattern", 2D) = "white" {}
		_LinePatternColor("Line Pattern Color", Color) = (1, 1, 1, 1)
		_LineVSpeed("Line Vertical Speed Multiplier", Float) = 0
		_LineHSpeed("Line Horizontal Speed Multiplier", Float) = 0
		
		_BasePattern("Base Pattern", 2D) = "white" {}
		_BasePatternColor("Base Pattern Color", Color) = (1, 1, 1, 1)
		
		_BlinkAlphaFrom("Blink Alpha From", FLoat) = 0
		_BlinkAlphaTo("Blink Alpha To", Float) = 1
		_BlinkSpeed("Blink Speed Multiplier", Float) = 1
	}
		SubShader
	{
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
		Pass
		{
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
         
			// vertex shader
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = v.uv;
				return o;
			}

			uniform float4 _BaseColor;
			
			sampler2D _LinePattern;
			uniform float4 _LinePattern_ST;
			uniform float4 _LinePatternColor;
			uniform float _LineVSpeed;
			uniform float _LineHSpeed;

			sampler2D _BasePattern;
			uniform float4 _BasePattern_ST;
			uniform float4 _BasePatternColor;

			uniform float _BlinkAlphaFrom;
			uniform float _BlinkAlphaTo;
			uniform float _BlinkSpeed;
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = _BaseColor;
				
				float4 linePatternAlpha = tex2D(_LinePattern, ((i.uv * _LinePattern_ST.xy + float2( _Time.y * _LineHSpeed, _Time.y * _LineVSpeed)) % 1 + 1) % 1);
				color = _LinePatternColor * linePatternAlpha.x + color * (1 - linePatternAlpha.x);

				float4 basePatternAlpha = tex2D(_BasePattern, i.uv * _BasePattern_ST.xy);
				color = _BasePatternColor * basePatternAlpha.x + color * (1 - basePatternAlpha.x);
				
				color.a *= _BlinkAlphaFrom + (_BlinkAlphaTo - _BlinkAlphaFrom) * (sin(_Time.y * 3.1416 * _BlinkSpeed) + 1) / 2;
				
                return color;
			}
			ENDCG
		}
	}
}