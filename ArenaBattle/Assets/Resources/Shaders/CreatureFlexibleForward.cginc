// Upgrade NOTE: replaced 'defined FLEXIBLE_ALPHA' with 'defined (FLEXIBLE_ALPHA)'

#ifndef CREATUREFLEXIBLE_FORWARD
#define CREATUREFLEXIBLE_FORWARD

#if defined (FLEXIBLE_RIM) || defined(FLEXIBLE_RIM_ALPHA) || defined(FLEXIBLE_GHOST)
	// Custom RimLight
	fixed3 _RimLightColor = fixed3(0,0,0);
	//float4 _RimLightParameter = float4(1, 0, 1, 0);
	float _RimLightRangeBase = 1;
	float _RimLightRangeShift = 0;
	float _RimLightIntensity = 1;
	half3 _RimLightDirection = float3(0, 0, -1);
	fixed4 _VertexColor;

	inline fixed CreatureRimFactor(in half3 viewnormal)
	{
		float d = dot(viewnormal, _RimLightDirection);
		d = saturate(d * _RimLightRangeBase + _RimLightRangeShift);
		d *= d;
		return d * _RimLightIntensity;
	}
	inline fixed3 CreatureRimColor(in fixed rimFactor)
	{
		return rimFactor*_RimLightColor;
	}
#endif

sampler2D _MainTex;
float4 _MainTex_ST;

#if defined(CUTOFF_ON) || defined(SPECULAR_ON) || defined(FLOW_TEX_ON) || defined(EMISSIVE_ON) || defined(EMISSIVE_ON_RUNTIME) || defined(FLEXIBLE_FACE) || defined(FLEXIBLE_GHOST)
	sampler2D _MaskTex;
#endif

#ifndef FLEXIBLE_GHOST
	uniform fixed3 _ActorAmbientColor = fixed3(0.5f, 0.5f, 0.5f);

	#ifdef FLEXIBLE_HAIR
		fixed4 _Color1;
		fixed4 _Color2;
		fixed4 _Color3;
	#endif

	#ifdef FLOW_TEX_ON
		sampler2D _FlowTex;
		half4 _FlowTex_ST;
		half4 _FlowSpeed;
		half _FlowIntensity;
	#endif

	#ifdef SPECULAR_ON
		half _Shininess;
		half _SpecularIntensity;
		fixed4 _SpecularMaterialColor;
	#endif
	fixed4 _MainColor;

	#ifdef FLEXIBLE_FACE
		fixed4 _EyeColor;
	#else
		#ifdef CUTOFF_ON
			half _CutOffRange;
		#endif
	#endif
	#if defined(EMISSIVE_ON) || (EMISSIVE_ON_RUNTIME)
		fixed4 _EmissiveColor;
		half _EmissiveShininess;
		half _EmissiveIntensity;
		half4 _EmissiveParameter;
	#endif
#endif
//uniform half4 unity_FogColor;
uniform half4 unity_FogStart;
uniform half4 unity_FogEnd;
			
struct VS_IN
{
	float4 vertex		: POSITION;
	float2 texcoord		: TEXCOORD0;
	float3 normal		: NORMAL;

	#ifndef FLEXIBLE_GHOST
		#ifdef FLOW_TEX_ON
			float2 flowTexcoord : TEXCOORD1;
		#endif
	#endif
};

struct VS_OUT 
{
	float4 pos											: SV_POSITION;
	half3 viewnormal									: NORMAL;
	#ifdef FLEXIBLE_GHOST
		half2 uv										: TEXCOORD0;
	#else
		fixed3 diff_rim									: COLOR0;
		#ifdef SPECULAR_ON
			fixed3 specular								: COLOR1;
		#endif
				
		#ifdef FLOW_TEX_ON
			half4 uv									: TEXCOORD0;
		#else
			half2 uv									: TEXCOORD0;
		#endif
	#endif
	half sight_ff										: TEXCOORD1;
};
		
VS_OUT vert (VS_IN v)
{
	VS_OUT o;
	o.pos = UnityObjectToClipPos (v.vertex);
	o.uv.xy = TRANSFORM_TEX (v.texcoord, _MainTex);				
	half fogFactor = GetFogFactor(o.pos, unity_FogStart, unity_FogEnd);

	float3 viewpos = ViewPos(v.vertex);
	half3 viewnormal = NormalizeViewNormal(v.normal);
	half3 toVert = normalize(viewpos);
	half3 sight = -toVert;
	o.sight_ff.x = fogFactor;

	#ifndef FLEXIBLE_GHOST
		#ifdef SPECULAR_ON
			VsDiffuseSpecularLighting(	viewpos, 
										viewnormal, 
										sight,
										_Shininess, 
										_SpecularIntensity, 
										o.diff_rim.rgb, 
										o.specular.rgb);
			o.specular.rgb *= _SpecularMaterialColor.rgb;
		#else
			o.diff_rim.rgb = VsDiffuseLighting(viewpos, viewnormal);
		#endif

		o.diff_rim.rgb += _ActorAmbientColor.xyz;
		o.diff_rim.rgb *= _MainColor.rgb * 2.0f;

		#if defined (FLEXIBLE_RIM) || defined(FLEXIBLE_RIM_ALPHA)
			o.diff_rim.rgb *= _VertexColor.rgb;
		#endif

		#ifdef FLOW_TEX_ON
			o.uv.zw = TRANSFORM_TEX(v.texcoord, _FlowTex) + _FlowSpeed.xy * _Time.y;
		#endif
	#endif

	o.viewnormal = viewnormal;		
	return o;
}

fixed4 frag(VS_OUT i) : COLOR 
{
	fixed4 c;
	half ff = i.sight_ff.x;

	#ifdef FLEXIBLE_GHOST
		fixed3 maskTex = tex2D (_MaskTex, i.uv).rgb;
		c.rgb = _RimLightColor.rgb;
		c.a = CreatureRimFactor(i.viewnormal);
	#else
		fixed3 diff = i.diff_rim.rgb;

		#if defined(CUTOFF_ON) || defined(SPECULAR_ON) || defined(FLOW_TEX_ON) || defined(EMISSIVE_ON) || defined(EMISSIVE_ON_RUNTIME) || defined(FLEXIBLE_FACE)
			fixed3 maskTex = tex2D (_MaskTex, i.uv).rgb;
		#endif

		fixed3 mainTex = tex2D (_MainTex, i.uv.xy).rgb;

		#ifdef FLEXIBLE_HAIR
			fixed3 color1 = mainTex.r * _Color1;
			fixed3 color2 = mainTex.g * _Color2;
			fixed3 color3 = mainTex.b * _Color3;
			mainTex = color1 + color2 + color3;
		#endif

		#ifdef FLEXIBLE_FACE
			// step 1
			fixed3 c1 = (1.0f - maskTex.b) * mainTex.rgb * diff * 2;
			// step 2
			fixed3 c2 = maskTex.b * mainTex.rgb * diff * _EyeColor * 2;
			c = fixed4(c1 + c2, 1);
		#else
			#ifdef FLEXIBLE_HAIR
				c = fixed4(mainTex * diff * 2.5f, 1);
			#else
				c = fixed4(mainTex * diff * 2, 1);
			#endif

			#ifdef CUTOFF_ON
				clip(maskTex.b - _CutOffRange);
			#endif
		#endif

		#if defined (FLEXIBLE_ALPHA) || defined(FLEXIBLE_RIM_ALPHA)
			#if defined(CUTOFF_ON) || defined(SPECULAR_ON) || defined(FLOW_TEX_ON) || defined(EMISSIVE_ON) || defined(EMISSIVE_ON_RUNTIME)
			c.a = maskTex.b;
			#else
				#ifdef FLEXIBLE_RIM_ALPHA
					c.a = 1.0f;
				#endif
			#endif
		#endif

		#ifdef SPECULAR_ON
			fixed3 spec = i.specular.rgb;
			fixed specMask = maskTex.g;
			c.rgb += spec * specMask;
		#endif

		#ifdef EMISSIVE_ON
			half emissiveRatio = saturate(maskTex.r - (_EmissiveShininess * (cos(_Time.y * _EmissiveParameter.x) + 1.0f) * 0.5f));
			half emissive = cos(_Time.y * _EmissiveParameter.y) * 0.25f + 0.75f;
			fixed4 emissiveColor = _EmissiveColor * emissive * _EmissiveIntensity;
			c.rgb = lerp(c.rgb, emissiveColor.rgb, emissiveRatio);
		#elif EMISSIVE_ON_RUNTIME
			half emissiveRatio = saturate(maskTex.r - (_EmissiveShininess * (_EmissiveParameter.z + 1.0f) * 0.5f));
			half emissive = _EmissiveParameter.w * 0.25f + 0.75f;
			fixed4 emissiveColor = _EmissiveColor * emissive * _EmissiveIntensity;
			c.rgb = lerp(c.rgb, emissiveColor.rgb, emissiveRatio);
		#endif

		c.rgb += CreatureRimColor(CreatureRimFactor(i.viewnormal));

		#ifdef FLOW_TEX_ON
			fixed flowMask = maskTex.r;
			fixed3 flowColor = tex2D(_FlowTex, i.uv.zw).rgb * _FlowIntensity;

			#ifdef FLOW_MODE_OVERLAY
				c.rgb = lerp(c.rgb, flowColor, flowMask);
			#elif defined(FLOW_MODE_MULTIPLY)
				c.rgb = lerp(c.rgb, c.rgb*flowColor*8, flowMask);
			#else
				c.rgb += flowColor * flowMask;
			#endif
		#endif
	#endif
	ApplyFog(c, ff, unity_FogColor);

	return c;
}
#endif