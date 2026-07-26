#ifndef OBJECT_FUNCTIONS_CG_INCLUDED
#define OBJECT_FUNCTIONS_CG_INCLUDED

#ifdef TREE_WAVE_ON
	float4 _TreeWaveBasis;
	float4 _TreeWaveValue;
#endif

float4 GetLocalVertex(in float4 vertex)
{
#ifdef TREE_WAVE_ON
	float4 waveRelativePos = vertex - _TreeWaveBasis;
	return vertex + _TreeWaveValue*max(waveRelativePos.y, 0);
#else
	return vertex;
#endif
}

#endif