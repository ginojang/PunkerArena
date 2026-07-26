// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Simplified Alpha Blended Particle shader. Differences from regular Alpha Blended Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "VESTINEL/Particles/Mask Alpha Blended" 
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (1,1,1,0.5)
		_ColorIntensity ("Color Intensity", Float) = 1
		_MainTex ("Particle Texture", 2D) = "white" {}
		_MaskTex ("Mask", 2D) = "white" {}
		_CutOffRange ("Cut Off Range", Range (0, 1)) = 0.5
		_FlowSpeed ("Flow Speed(XY:Main ZW:Distortion)", Vector) = (0,0,0,0)
		[HideInInspector]_ControlAlpha ("Control Alpha", Float) = 1

		[HideInInspector]_BillboardMat0 ("BillboardMat0", Vector) = (1,0,0,0)
		[HideInInspector]_BillboardMat1 ("BillboardMat1", Vector) = (0,1,0,0)
		[HideInInspector]_BillboardMat2 ("BillboardMat2", Vector) = (0,0,1,0)
	}

	Subshader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off 
			ZWrite Off 
			Fog {Mode Off}

			CGPROGRAM

			#pragma multi_compile NO_CUTOFF CUTOFF_WITH_ALPHA CUTOFF_ONLY
			#pragma multi_compile UVCUT_OFF UVCUT_ON
			#pragma multi_compile SECOND_UV_OFF SECOND_UV_ON
			#pragma multi_compile SECOND_UV_MASK_OFF SECOND_UV_MASK_ON
			#pragma multi_compile BILLBOARD_OFF BILLBOARD_ON

			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "../UtilFunctionsCG.cginc"

			struct VS_IN
			{
				float4 vertex		: POSITION;			
				float2 texcoord		: TEXCOORD0;
				float4 color		: COLOR;

				#if defined(SECOND_UV_ON) || defined(SECOND_UV_MASK_ON)
					float2 texcoord1	: TEXCOORD1;	
				#endif
			};

			struct v2f
			{
				float4 pos		: SV_POSITION;
				half4 uv		: TEXCOORD0;
				half ff			: TEXCOORD1;
				fixed4 color	: COLOR;
			};
 
			fixed4 _TintColor;
			fixed _ControlAlpha;		

			sampler2D _MainTex;
			sampler2D _MaskTex;
			half4 _MainTex_ST;
			half4 _MaskTex_ST;
			half4 _FlowSpeed;

			half _ColorIntensity;

			#if defined(CUTOFF_WITH_ALPHA) || defined(CUTOFF_ONLY) 
				half _CutOffRange;
			#endif

			#ifdef BILLBOARD_ON
				uniform float4 _BillboardMat0;
				uniform float4 _BillboardMat1;
				uniform float4 _BillboardMat2;
			#endif

			//uniform half4 unity_FogColor;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			v2f vert(VS_IN v)
			{
				v2f o;
				
				#ifdef BILLBOARD_ON				
					float4 p = v.vertex;
					v.vertex.x = dot(_BillboardMat0, p);
					v.vertex.y = dot(_BillboardMat1, p);
					v.vertex.z = dot(_BillboardMat2, p);
				#endif
				o.pos = UnityObjectToClipPos (v.vertex);

				#ifdef SECOND_UV_ON
					o.uv.xy = TRANSFORM_TEX (v.texcoord1, _MainTex) + fmod(_FlowSpeed.xy * _Time.y, 1.0f);  
				#else
					o.uv.xy = TRANSFORM_TEX (v.texcoord, _MainTex) + fmod(_FlowSpeed.xy * _Time.y, 1.0f);  
				#endif
				#ifdef SECOND_UV_MASK_ON
					o.uv.zw = TRANSFORM_TEX (v.texcoord1, _MaskTex);  
				#else
					o.uv.zw = TRANSFORM_TEX (v.texcoord, _MaskTex);  
				#endif
				o.color = saturate(v.color * _TintColor * 2);
				o.color.rgb *= _ColorIntensity;
				o.color.a *= _ControlAlpha;
				
				o.ff = GetFogFactor(o.pos, unity_FogStart, unity_FogEnd);
				return o;
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				#ifdef UVCUT_ON
					clip(float4(i.uv.xy, 1-i.uv.xy));
				#endif

				fixed4 tex = tex2D(_MainTex, i.uv.xy) * i.color;
				
				fixed mask = tex2D(_MaskTex, i.uv.zw).r;
				tex.a *= mask;

				#if defined(CUTOFF_WITH_ALPHA) || defined(CUTOFF_ONLY) 
					clip(tex.a-_CutOffRange);
				#endif

				#ifdef CUTOFF_ONLY
					tex.a=1;
				#endif

				ApplyFog(tex, i.ff, unity_FogColor);

				return tex;
			}
			ENDCG		
		}
	}
CustomEditor "ParticleMaterialInspector"
}