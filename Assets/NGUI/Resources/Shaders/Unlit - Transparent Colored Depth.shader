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
			ZWrite Off
			ZTest Less
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
	        struct fragOut
			{
				half4 color : SV_Target ;
				float depth : SV_Depth ;
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
				// o.color.rgb = o.vertex.zzz;
				// o.color.a = 1.0f;
				
				return o;
			}
				
			fragOut frag (v2f IN)
			{//
				fragOut o;
				fixed4 texColor = tex2D(_MainTex, IN.texcoord);
				clip(texColor.a-0.9);
				o.color = texColor * IN.color;
				
				o.depth =  IN.vertex.z;
				//o.color.rgb = IN.vertex.zzz;
				return o;
			}
			ENDCG
		}
	}

}
