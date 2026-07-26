#ifndef LIGHT_FUNCTION_INCLUDED
#define LIGHT_FUNCTION_INCLUDED

#include "UnityShaderVariables.cginc"

#define MANY_PIXEL_LIGHT_COUNT 3

inline fixed3 VsDiffuseLighting(in float3 viewpos, 
								in half3 viewnormal, 
								out float4 atten4,
								out float3 toLight[4],
								uniform int pixelLightCount)
{
	int i=0;
	for(i=0; i<4; ++i) toLight[i] = unity_LightPosition[i].xyz - viewpos * unity_LightPosition[i].w;

	half4 lengthSq4 = LengthSq4(toLight[0], toLight[1], toLight[2], toLight[3]);
	Normalize4(toLight[0], toLight[1], toLight[2], toLight[3], lengthSq4);
	
	//customzied attenuation tern
	/*
	float4 rangeSq4 = float4(unity_LightAtten[0].w, unity_LightAtten[1].w, unity_LightAtten[2].w, unity_LightAtten[3].w);
	float4 invRangeSq = 1.0/rangeSq4;
	atten4 = saturate(1 - lengthSq4 * invRangeSq);
	atten4 *= atten4;
	*/

	float4 attenSq4 = float4(unity_LightAtten[0].z, unity_LightAtten[1].z, unity_LightAtten[2].z, unity_LightAtten[3].z);
	atten4 = 1.0 / (1.0 + lengthSq4 * attenSq4);
	
	fixed3 diffuse=0;
	half4 ndotL4;
	for(i=pixelLightCount; i<4; ++i)
	{
		ndotL4[i] = dot(toLight[i], viewnormal);
	}

	ndotL4 = saturate(ndotL4) * atten4;
	
	for(i=pixelLightCount; i<4; ++i)
	{
		diffuse += ndotL4[i] * unity_LightColor[i].rgb;
	}
	
	return diffuse;	
}

inline fixed3 VsDiffuseLighting(in float3 viewpos, in half3 viewnormal)
{
	float4 atten4;
	float3 toLight[4];

	return VsDiffuseLighting(viewpos, viewnormal, atten4, toLight, 0);
}

inline void VsDiffuseSpecularLighting(	in float3 viewpos,
										in half3 viewnormal, 
										in half3 sight,
										in half shininess, 
										in half specIntensity,
										out fixed3 diffuse, 
										out fixed3 specular,
										out float3 toLight[4],
										out float4 atten4,
										uniform int pixelLightCount)
{
	diffuse = VsDiffuseLighting(viewpos, 
								viewnormal, 
								atten4, 
								toLight,
								pixelLightCount);
				
	specular=0;		
	half4 ndotH4;	
	int i=0;					
	for(i=pixelLightCount; i<4; ++i)
	{
		half3 lightReflect = (toLight[i] + sight) * 0.5f;
		ndotH4[i] = dot(lightReflect, viewnormal);
	}
	
	ndotH4 = pow(saturate(ndotH4), shininess.xxxx) * atten4;
	for(i=pixelLightCount; i<4; ++i)
	{
		specular += ndotH4[i] * unity_LightColor[i].rgb;
	}
	
	specular *= specIntensity;
}

inline void VsDiffuseSpecularLighting(	in float3 viewpos,
										in half3 viewnormal, 
										in half3 toVert,
										in half shininess, 
										in half specIntensity,
										out fixed3 diffuse, 
										out fixed3 specular)
{
	float3 toLight[4];
	float4 atten4;

	VsDiffuseSpecularLighting(	viewpos, 
								viewnormal, 
								toVert,
								shininess, 
								specIntensity, 
								diffuse, 
								specular, 
								toLight,
								atten4,
								0);
}

inline fixed3 PsDiffuseLighting(in half3 lightDir, in half atten, in half3 normal)
{
	half ndotL = dot(lightDir, normal);
	ndotL = saturate(ndotL) * atten;

	return ndotL * unity_LightColor[0].rgb;
}

inline fixed3 PsSpecularLighting(	in half3 lightDir, 
									in half3 sight,
									in half atten,
									in half3 normal, 
									in float shininess, 
									in float specIntensity)
{
	half3 lightReflect = (lightDir + sight) * 0.5f;
	half ndotH = dot(lightReflect, normal);
	ndotH = pow(saturate(ndotH), shininess.xxxx) * specIntensity * atten;
	
	return ndotH * unity_LightColor[0].rgb;
}

inline fixed3 PsDiffuseManyLighting(in half3 lightDir[MANY_PIXEL_LIGHT_COUNT], in half4 atten4, in half3 normal)
{
	fixed3 diffuse=0;
	half4 ndotL4;
	int i=0;
	for(i=0; i<MANY_PIXEL_LIGHT_COUNT; ++i)
	{
		ndotL4[i] = dot(lightDir[i], normal);
	}
	
	ndotL4 = saturate(ndotL4) * atten4;
	
	for(i=0; i<MANY_PIXEL_LIGHT_COUNT; ++i)
	{
		diffuse += ndotL4[i] * unity_LightColor[i].rgb;
	}
	
	return diffuse;
}

inline fixed3 PsSpecularManyLighting(	in half3 ligtDir[MANY_PIXEL_LIGHT_COUNT],
										in half3 sight,
										in half4 atten4,
										in half3 normal,
										in float shininess, 
										in float specIntensity)
{
	fixed3 spec=0;
	half4 ndotH4; 
	int i=0;
	
	for(i=0; i<MANY_PIXEL_LIGHT_COUNT; ++i)
	{
		half3 lightReflect = (ligtDir[i] + sight) * 0.5f;
		ndotH4[i] = dot(lightReflect, normal);
	}
	
	ndotH4 = pow(saturate(ndotH4), shininess.xxxx) * atten4;
	
	for(i=0; i<MANY_PIXEL_LIGHT_COUNT; ++i)
	{
		spec += ndotH4[i] * unity_LightColor[i].rgb;
	}
	
	return spec * specIntensity;
}

#endif