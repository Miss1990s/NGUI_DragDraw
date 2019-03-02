// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Transparent DrawLine"
{
	// Properties
	// {
	// 	_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
	// }

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
			Offset -1, -1
			Fog { Mode Off }
			ZTest Off
			ZClip Off
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);
			// float2 _ClipArgs0 = float2(1000.0, 1000.0);

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
			};

			v2f o;

			v2f vert (appdata_t v)
			{
				o.vertex = v.vertex;
				o.color = v.color;
				return o;
			}
			
			half4 frag (v2f IN) : SV_Target
			{
				float2 factor = _ClipRange0.zw - abs(IN.vertex.xy - _ClipRange0.xy);
				clip(factor.x);
				clip(factor.y);
				half4 col = IN.color;
				
				//col.rgb = IN.vertex.xxx;
				return col;
			}
			ENDCG
		}
	}
}
