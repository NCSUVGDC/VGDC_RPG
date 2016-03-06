Shader "Unlit/tilemapShader"
{
	Properties
	{
		_MainTex ("Tiles", 2D) = "black" {}
		_AtlasTex ("Atlas", 2D) = "white" {}
		_TilesWidth("Tiles Width", Float) = 64
		_TilesHeight("Tiles Height", Float) = 64
		_AtlasSize("Atlas Size", Float) = 4
		_AtlasResolution("Atlas Resolution", Float) = 512
		_HighlightColor("Highlight Color", Color) = (0, 0, 1, 0)
		_Frame("Frame", Int) = 0
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float2 uv2 : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _AtlasTex;
			float4 _MainTex_ST;
			float4 _HighlightColor;

			float _TilesWidth;
			float _TilesHeight;
			float _AtlasSize;
			float _AtlasResolution;

			float _TestSlide;
			uint _Frame;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = v.vertex.xz;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col = tex2D(_MainTex, float2(i.uv2.x / _TilesWidth - 0.001 / _TilesWidth, i.uv2.y / _TilesHeight - 0.001 / _TilesHeight));
				
				int fa = floor(col.z * _AtlasSize);
				col.y += (_Frame % fa) / _AtlasSize;

				col.y = (_AtlasSize - 1.0) / _AtlasSize - col.y;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);				
				//float2 nuv = float2(clamp(frac(i.uv.x * _TilesWidth), 0.01, 0.99) - 0.0 / _AtlasResolution, frac(i.uv.y * _TilesHeight) + 0.5 / _AtlasResolution);
				float2 nuv = float2(clamp(frac(i.uv2.x), 0.01, 0.99), frac(i.uv2.y));
				return lerp(tex2D(_AtlasTex, col.xy + nuv / _AtlasSize), _HighlightColor, col.w);
			}
			ENDCG
		}
	}
}
