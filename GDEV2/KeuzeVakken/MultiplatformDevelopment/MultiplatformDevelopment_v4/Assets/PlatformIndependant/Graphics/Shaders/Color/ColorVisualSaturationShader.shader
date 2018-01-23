Shader "Custom/ColorVisualSaturationShader"
{
	Properties
	{
		_Hue("Hue", Range(0,1)) = 0
		_Saturation("Saturation", Range(0,1)) = 0
		_Value("Value", Range(0,1)) = 1
	}
		SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always

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

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	//sampler2D _MainTex;
	float _Hue;
	float _Saturation;
	float _Value;

	float3 Hue(float H)
	{
		float R = abs(H * 6 - 3) - 1;
		float G = 2 - abs(H * 6 - 2);
		float B = 2 - abs(H * 6 - 4);
		return saturate(float3(R, G, B));
	}

	float4 HSVtoRGB(in float3 HSV)
	{
		return float4(((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z, 1);
	}

	fixed4 frag(v2f i) : SV_Target
	{
		//fixed4 col = tex2D(_MainTex, i.uv);
		// just invert the colors
		//col.rgb = 1 - col.rgb;

		float4 col;
	col.rgb = HSVtoRGB(float3(_Hue, i.uv.x, _Value));

	return col;
	}
		ENDCG
	}
	}
}
