Shader "Unlit/lightShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ShadowColor("Shadow Color", Color) = (0, 0, 0, 1)
		_TilesWidth("Tiles Width", Float) = 64
		_TilesHeight("Tiles Height", Float) = 64
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		//Blend SrcAlpha OneMinusSrcAlpha
		Blend DstColor Zero

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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float2 uv2 : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _ShadowColor;
			float _TilesWidth;
			float _TilesHeight;
			
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
				float4 av = tex2D(_MainTex, float2(i.uv2.x / _TilesWidth, i.uv2.y / _TilesHeight));
				//av *= av * 32 * 16;
				fixed4 col = lerp(_ShadowColor, av, _ShadowColor.a);//float4(av.rgb, _ShadowColor.a);//lerp(_ShadowColor, av, av.a);
				return col;
			}
			ENDCG
		}
	}
}
