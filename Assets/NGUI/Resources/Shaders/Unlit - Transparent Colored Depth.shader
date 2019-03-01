// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Transparent Colored Depth"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite On
			ZTest Off
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			v2f o;

			v2f vert (appdata_t v)
			{
				float4x4 mvp = UNITY_MATRIX_MVP;
				mvp[2][0] = 0.0;
				mvp[2][1] = 0.0;
				mvp[2][2] = 1.0;
				mvp[2][3] = 0.0;
				
				o.vertex = mul(mvp, v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				o.color.rgb = o.vertex.zzz;
				o.color.a = 1.0f;
				
				return o;
			}
				
			fixed4 frag (v2f IN) : SV_Target
			{//tex2D(_MainTex, IN.texcoord) *
				return  IN.color;
			}
			ENDCG
		}
	}

}
