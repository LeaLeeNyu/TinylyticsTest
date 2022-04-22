Shader "Sprites/Custom"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

		_MultiplyColor ("Multiply Color", Color) = (1,1,1,1)
		_AdditiveColor ("Additive Color", Color) = (0,0,0,1)
		[MaterialToggle] _GrayScale ("GrayScale", Float) = 0
		_Colorize ("Colorize", Color) = (0, 0, 0, 0)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
				if (_AlphaSplitEnabled)
					color.a = tex2D (_AlphaTex, uv).r;
#endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

				return color;
			}

			fixed4 _MultiplyColor;
			fixed4 _AdditiveColor;
			float _GrayScale;
			fixed4 _Colorize;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;

				if (_GrayScale) {
					float gray = 0.2126 * c.r + 0.7152 * c.g + 0.0722 * c.b;
					c.r = gray;
					c.g = gray;
					c.b = gray;
				}

				if (_Colorize.a > 0) {
					float gray = 0.33 * c.r + 0.34 * c.g + 0.33 * c.b;
					float3 base = fixed3(gray, gray, gray);
					float3 overlayed;
					if (gray < 0.5) {
						overlayed = base * _Colorize.rgb * 2;
					} else {
						overlayed = fixed3(1, 1, 1) - ((1, 1, 1) - base) * (fixed3(1, 1, 1) - _Colorize.rgb) * 2;
					}
					c.rgb = c.rgb * (1 - _Colorize.a) + overlayed.rgb * _Colorize.a;
				}

				c.rgb *= c.a;

				c.rgb *= _MultiplyColor.rgb;
				c.rgb = (c.rgb + _AdditiveColor.rgb * _AdditiveColor.a) * c.a;
				return c;
			}
		ENDCG
		}
	}
}
