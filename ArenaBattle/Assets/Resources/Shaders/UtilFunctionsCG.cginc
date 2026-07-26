// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef UTIL_FUNCTIONS_CG_INCLUDED
#define UTIL_FUNCTIONS_CG_INCLUDED

#ifdef SHADER_API_D3D11 
	#pragma exclude_renderers d3d9 gles xbox360
#elif SHADER_API_D3D9
	#pragma exclude_renderers d3d11 gles xbox360
#endif 

#include "UnityShaderVariables.cginc"

inline float FastInvSqrtAlmostOne(in float3 v)
{
	float lenSq = dot(v, v);
	const float a0 = 15.0f / 8.0f;
	const float a1 = -5.0f / 4.0f;
	const float a2 = 3.0f / 8.0f;
	
	return a0 + (a1 * lenSq) + (a2 * lenSq * lenSq);
}

inline float3 FastReNormalize(in float3 v)
{
	return v * FastInvSqrtAlmostOne(v);
}

inline float3 ViewPos(in float4 p)
{
	return mul(UNITY_MATRIX_MV, p).xyz;
}

inline float3 WorldPos(in float4 p)
{
	return mul(unity_ObjectToWorld, p).xyz;
}

inline half3 ViewNormal(in float3 normal)
{
	half3 viewnormal = mul((float3x3)UNITY_MATRIX_MV, normal);
	return (viewnormal);
}

inline half3 NormalizeViewNormal(in float3 normal)
{
	half3 viewnormal = mul((float3x3)UNITY_MATRIX_MV, normal);
	return normalize(viewnormal);
}

inline half3 WorldNormal(in float3 normal)
{
	half3 viewnormal = mul((float3x3)unity_ObjectToWorld, normal);
	return (viewnormal);
}

inline half3 NormalizeWorldNormal(in float3 normal)
{
	half3 viewnormal = mul((float3x3)unity_ObjectToWorld, normal);
	return normalize(viewnormal);
}

float RimFactor(in half3 viewnormal)
{
	return saturate(1-viewnormal.z);
}

float RimFactor(in half3 viewnormal, in float ignoreDepth)
{
	return saturate(1-viewnormal.z-ignoreDepth);
} 

float RimFactor(in half3 viewnormal, in half3 sight)
{
	return saturate(1-dot(viewnormal, sight));
}

float RimFactor(in half3 viewnormal, in half3 sight, in float ignoreDepth)
{
	return saturate(1-dot(viewnormal, sight)-ignoreDepth);
}

inline half GetFogFactor(in float4 clipVert, in half4 fogStart, in half4 fogEnd)
{
	return (fogEnd.x - clipVert.w) / (fogEnd.x - fogStart.x);	
}

inline void ApplyFog(inout fixed4 c, in half fogFactor, in fixed4 fogColor)
{
	c.rgb = lerp(fogColor.rgb, c.rgb, saturate(fogFactor));
}

inline void ApplyFogForTransparent(inout fixed4 c, in half fogFactor)
{
	c *= saturate(fogFactor);
}

inline float3 UnpackNormal(float3 packNormal)
{
	return packNormal*2-1;
}


inline half4 LengthSq4(in float3 v0, in float3 v1, in float3 v2, in float3 v3)
{
	return half4(	dot(v0, v0),
					dot(v1, v1),
					dot(v2, v2),
					dot(v3, v3));
}

inline void Normalize4(inout float3 v0, inout float3 v1, inout float3 v2, inout float3 v3, in half4 lengthSq4)
{
	half4 rsqrtLengthSq4 = rsqrt(lengthSq4);

	v0 *= rsqrtLengthSq4.x;
	v1 *= rsqrtLengthSq4.y;
	v2 *= rsqrtLengthSq4.z;
	v3 *= rsqrtLengthSq4.w;
}										

half3 ToTangentDir(in half3 dir, in half3 tangent, in half3 binormal, in half3 normal)
{
	 half3 result = half3(	dot(dir, tangent),
							dot(dir, binormal),
							dot(dir, normal));
	//result = normalize(result);
	return result;
}

half3 FromTangentDir(in half3 dir, in half3 tangent, in half3 binormal, in half3 normal)
{
	half3 top = dir.xxx * tangent;
	half3 middle = dir.yyy 	* binormal;
	half3 bottom = dir.zzz * normal;

	half3 result = top + middle + bottom;
	//result = normalize(result);
	return result;
}

#endif