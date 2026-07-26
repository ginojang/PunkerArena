#ifndef GRASS_FUNCTIONS_CG_INCLUDED
#define GRASS_FUNCTIONS_CG_INCLUDED

float3 _WindAxis;
float4 _WindParameter;

float4 GetGrassVertexWithUV(in float4 vertex, in float2 uv)
{
	float t = (_Time.y + dot(vertex.xz, _WindAxis.xz)) * _WindParameter.y;
	float wave = cos(t) * _WindParameter.z * uv.y;
	vertex.xyz += _WindAxis*wave;
	return vertex;
}

float4 GetGrassVertexWithColor(in float4 vertex, in fixed4 color)
{
	float t = (_Time.y + dot(vertex.xz, _WindAxis.xz)) * _WindParameter.y;
	float wave = cos(t) * _WindParameter.z * color.x;
	vertex.xyz += _WindAxis*wave;
	return vertex;
}

#endif