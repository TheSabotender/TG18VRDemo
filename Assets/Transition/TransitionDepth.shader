Shader "Unlit/TransitionDepth"
{
	Properties
	{
		_TransTex("Transition", 2D) = "white" {}
		_WhiteTex("White", 2D) = "white" {}
		_BlackTex("Black", 2D) = "white" {}
		_Amount("Amount", Range(0,1)) = 0
		_Edge("Edge", Range(0.01,0.5)) = 0
		[Toggle(INVERT)]_Invert("Invert", Float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			sampler2D _WhiteTex;
			sampler2D _BlackTex;
			sampler2D _CameraDepthTexture;
			float4 _TransTex_ST;
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
				fixed4 col1 = tex2D(_WhiteTex, i.uv);
				fixed4 col2 = tex2D(_BlackTex, i.uv);
				fixed4 dpth = tex2D(_CameraDepthTexture, i.uv);

				float g = (trn.r + trn.g + trn.b) / 3;

				//Depth
				g = dpth.r * 10;

				if (_Invert >= 0.5) {
					g = 1 - g;
				}					

				fixed edge = lerp(g + _Edge, g - _Edge, _Amount);
				fixed alpha = smoothstep(_Amount + _Edge, _Amount - _Edge, edge);

				return lerp(col1, col2, alpha);
			}
			ENDCG
		}
	}
}
