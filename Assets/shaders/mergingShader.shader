// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Fullscreen/Merging Shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SunColor("Sun Color", Color) = (0, 0, 0, 1)
		_AmbientColor("Ambient Color", Color) = (0.1, 0.1, 0.1, 1)
		_Brightness("Brightness", Float) = 1
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float2 screenPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _WarpTex;
			sampler2D _LightTex;
			float4 _MainTex_ST;
			float _Brightness;

			float4 _SunColor;
			float4 _AmbientColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = o.vertex.xy * 0.5 + float2(0.5, 0.5);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//return float4(tex2D(_WarpTex, i.screenPos) * float2(0.5, 0.5) + float2(0.5, 0.5), 0, 1);
				float2 uv = i.screenPos + tex2D(_WarpTex, i.screenPos).xy;
				fixed4 col = tex2D(_MainTex, uv) * _Brightness;
				col.xyz *= lerp(tex2D(_LightTex, uv), _SunColor, _SunColor.a) + _AmbientColor;
				return col;
			}
			ENDCG
		}
	}
}
