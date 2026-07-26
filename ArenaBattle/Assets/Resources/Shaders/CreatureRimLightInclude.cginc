#ifndef CREATURE_RIMLIGHT_INCLUDE
#define CREATURE_RIMLIGHT_INCLUDE

uniform fixed3 CreatureRimLightColor = fixed3(0,0,0);

//x:1/range (range base)
//y:1-x     (range shift)
//z:intensity
//uniform float4 CreatureRimLightParameter = float4(1, 0, 1, 0);

// for hit color
half CreatureRimLightRangeBase = 1;
half CreatureRimLightRangeShift = 0;
uniform half CreatureRimLightIntensity = 1;
uniform half CreatureRimLightPow = 1;

uniform half3 CreatureRimLightDirection = float3(0, 0, -1);

inline fixed CreatureRimFactor(in half3 viewnormal)
{
	float d = dot(viewnormal, CreatureRimLightDirection);
	d = saturate(d * CreatureRimLightPow + CreatureRimLightRangeBase);
	//d *= d;
	return d * CreatureRimLightIntensity;
}

inline fixed3 CreatureRimColor(in fixed rimFactor)
{
	return rimFactor*CreatureRimLightColor;
}

#endif