Shader "Unlit/LoadTexture"
{
	Properties
	{
		_TransTex("Transition", 2D) = "white" {}
		_Color("Main Color", COLOR) = (1,1,1,1)		
		_Amount("Amount", Range(0,1)) = 0
		_Edge("Edge", Range(0.01,0.5)) = 0
		[Toggle(INVERT)]_Invert("Invert", Float) = 0.0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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

			sampler2D _TransTex;
			float4 _TransTex_ST;
			float4 _Color;
			float _Amount;
			float _Edge;
			float _Invert;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _TransTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 trn = tex2D(_TransTex, i.uv);

				float g = (trn.r + trn.g + trn.b) / 3;
				float4 alphaColor = _Color;
				alphaColor.a = 0;

				if (_Invert >= 0.5) {
					g = 1 - g;
				}					

				fixed edge = lerp(g + _Edge, g - _Edge, _Amount);
				fixed alpha = smoothstep(_Amount + _Edge, _Amount - _Edge, edge);

				clip(alpha - 0.1);

				return lerp(alphaColor, _Color, alpha);
			}
			ENDCG
		}
	}
}
