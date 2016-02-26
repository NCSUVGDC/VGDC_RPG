Shader "Unlit/tilemapShader"
{
	Properties
	{
		_MainTex ("Tiles", 2D) = "black" {}
		_AtlasTex ("Atlas", 2D) = "white" {}
		_TilesWidth ("Tiles Width", Float) = 64
		_TilesHeight ("Tiles Height", Float) = 64
		_AtlasSize("Atlas Size", Float) = 4
		_TestSlide("Test Slider", Range (0.1, 4.0)) = 1
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
			};

			sampler2D _MainTex;
			sampler2D _AtlasTex;
			float4 _MainTex_ST;

			float _TilesWidth;
			float _TilesHeight;
			float _AtlasSize;

			float _TestSlide;
			uint _Frame;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				
				uint fa = floor(col.z * _AtlasSize);
				col.y += (_Frame % fa) / _AtlasSize;

				col.y = (_AtlasSize - 1.0) / _AtlasSize - col.y;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);				
				float2 nuv = float2(frac(i.uv.x * _TilesWidth), frac(i.uv.y * _TilesHeight));
				col = tex2D(_AtlasTex, col.xy + nuv / _AtlasSize);
				return col;
			}
			ENDCG
		}
	}
}
