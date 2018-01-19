Shader "Tools/DataMesh/DataMeshEditorVisualisation"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		[Toggle] _IsSelected ("IsSelected", float) = 1
	}
	SubShader
	{

		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		ZWrite Off 
		ZTest Always

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 _Color;
			float _IsSelected;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color;

				float3 intensity = dot(col.rgb, float3(0.299, 0.587, 0.114));
				col.rgb = lerp(intensity, col.rgb, lerp(0.3, 1,_IsSelected));

				col.a = lerp(0.5, 0.7,_IsSelected);

				return col;
			}
			ENDCG
		}
	}
}
