// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Anna/FastBloom"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Bloom("Bloom (RGB)", 2D) = "black" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _Bloom;

	uniform half4 _MainTex_TexelSize;

	uniform half4 _Parameter;

	#define ONE_MINUS_THRESHHOLD_TIMES_INTENSITY _Parameter.w
	#define POWER _Parameter.y
	#define THRESHHOLD _Parameter.z

	struct v2f_simple
	{
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;

	#if UNITY_UV_STARTS_AT_TOP
		half2 uv2 : TEXCOORD1;
	#endif
	};

	v2f_simple vertBloom(appdata_img v)
	{
		v2f_simple o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;

	#if UNITY_UV_STARTS_AT_TOP
		o.uv2 = v.texcoord;
		if (_MainTex_TexelSize.y < 0.0)
			o.uv.y = 1.0 - o.uv.y;
	#endif

		return o;
	}

	struct v2f_tap
	{
		float4 pos : SV_POSITION;
		half2 uv20 : TEXCOORD0;
		half2 uv21 : TEXCOORD1;
		half2 uv22 : TEXCOORD2;
		half2 uv23 : TEXCOORD3;
	};

	v2f_tap vert4Tap(appdata_img v)
	{
		v2f_tap o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv20 = v.texcoord + _MainTex_TexelSize.xy * half2(0.5h, 0.5h); ;
		o.uv21 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, -0.5h);
		o.uv22 = v.texcoord + _MainTex_TexelSize.xy * half2(0.5h, -0.5h);
		o.uv23 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, 0.5h);

		return o;
	}

	fixed4 fragBloom(v2f_simple i) : SV_Target
	{
	#if UNITY_UV_STARTS_AT_TOP
		fixed4 color = tex2D(_MainTex, i.uv2);
		return color + tex2D(_Bloom, i.uv);
	#else
		fixed4 color = tex2D(_MainTex, i.uv);
		return color + tex2D(_Bloom, i.uv);
	#endif
	}

	fixed4 fragDownsample(v2f_tap i) : SV_Target
	{
		fixed4 color = tex2D(_MainTex, i.uv20);
		color += tex2D(_MainTex, i.uv21);
		color += tex2D(_MainTex, i.uv22);
		color += tex2D(_MainTex, i.uv23);

		color = max(color * 0.25 - THRESHHOLD, 0) * ONE_MINUS_THRESHHOLD_TIMES_INTENSITY;
		color = pow(color, POWER);

		return color;
	}

	// weight curves
	static const half4 curve4[7] =
	{
		half4(0.0205, 0.0205, 0.0205, 0),
		half4(0.0855, 0.0855, 0.0855, 0),
		half4(0.232, 0.232, 0.232, 0),
		half4(0.324, 0.324, 0.324, 1),
		half4(0.232, 0.232, 0.232, 0),
		half4(0.0855, 0.0855, 0.0855, 0),
		half4(0.0205, 0.0205, 0.0205, 0)
	};

	struct v2f_withBlurCoords8
	{
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half2 offs : TEXCOORD1;
	};

	v2f_withBlurCoords8 vertBlurHorizontal(appdata_img v)
	{
		v2f_withBlurCoords8 o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv = v.texcoord;
		o.offs = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _Parameter.x;

		return o;
	}

	v2f_withBlurCoords8 vertBlurVertical(appdata_img v)
	{
		v2f_withBlurCoords8 o;
		o.pos = UnityObjectToClipPos(v.vertex);

		o.uv = v.texcoord.xy;
		o.offs = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _Parameter.x;

		return o;
	}

	half4 fragBlur8(v2f_withBlurCoords8 i) : SV_Target
	{
		half2 netFilterWidth = i.offs;
		half2 coords = i.uv - netFilterWidth * 3.0;

		half4 color = half4(0, 0, 0, 0);

		color += tex2D(_MainTex, coords) * curve4[0];
		coords += netFilterWidth;

		color += tex2D(_MainTex, coords) * curve4[1];
		coords += netFilterWidth;

		color += tex2D(_MainTex, coords) * curve4[2];
		coords += netFilterWidth;

		color += tex2D(_MainTex, coords) * curve4[3];
		coords += netFilterWidth;

		color += tex2D(_MainTex, coords) * curve4[4];
		coords += netFilterWidth;

		color += tex2D(_MainTex, coords) * curve4[5];
		coords += netFilterWidth;

		color += tex2D(_MainTex, coords) * curve4[6];

		return color;
	}
	
	ENDCG

	SubShader
	{
		ZTest Off Cull Off ZWrite Off Blend Off

		// 0	
		Pass
		{
			CGPROGRAM
			#pragma vertex vertBloom
			#pragma fragment fragBloom
			ENDCG
		}

		// 1
		Pass
		{
			CGPROGRAM
			#pragma vertex vert4Tap
			#pragma fragment fragDownsample
			ENDCG

		}

		// 2
		Pass
		{
			ZTest Always
			Cull Off

			CGPROGRAM
			#pragma vertex vertBlurVertical
			#pragma fragment fragBlur8
			ENDCG
		}

		// 3	
		Pass
		{
			ZTest Always
			Cull Off

			CGPROGRAM
			#pragma vertex vertBlurHorizontal
			#pragma fragment fragBlur8
			ENDCG
		}
	}

	FallBack Off
}
