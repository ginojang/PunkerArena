// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Simplified Alpha Blended Particle shader. Differences from regular Alpha Blended Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "VESTINEL/Particles/Alpha Blended" 
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (1,1,1,0.5)
		_ColorIntensity ("Color Intensity", Float) = 1
		_MainTex ("Particle Texture", 2D) = "white" {}
		_DistortionTex ("Distortion Texture", 2D) = "0.5, 0.5, 0.5, 0.5" {}
		_CutOffRange ("Cut Off Range", Range (0, 1)) = 0.5
		_DistortionRate ("Distortion Rate", Float) = 0
		_FlowSpeed ("Flow Speed(XY:Main ZW:Distortion)", Vector) = (0,0,0,0)
		_RotateUVParameter ("Rotate UV(XY:Center ZW:Degree)", Vector) = (0.5,0.5,0,0)
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
			#pragma multi_compile DISTORTION_OFF DISTORTION_ON
			#pragma multi_compile SECOND_UV_OFF SECOND_UV_ON
			#pragma multi_compile BILLBOARD_OFF BILLBOARD_ON
			#pragma multi_compile ROTATE_UV_OFF ROTATE_UV_ON

			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "../UtilFunctionsCG.cginc"

			struct VS_IN
			{
				float4 vertex		: POSITION;			
				float4 color		: COLOR;

				#ifdef SECOND_UV_ON
					float2 texcoord		: TEXCOORD1;
				#else
					float2 texcoord		: TEXCOORD0;
				#endif
			};

			struct v2f
			{
				float4 pos		: SV_POSITION;
				fixed4 color	: COLOR;

				half3 uv_ff		: TEXCOORD0;

				#ifdef DISTORTION_ON
					half2 distortion_uv	: TEXCOORD1;
				#endif
			};
 
			fixed4 _TintColor;
			fixed _ControlAlpha;
			sampler2D _MainTex;
			half4 _MainTex_ST;
			half4 _FlowSpeed;
			half _ColorIntensity;

			#if defined(CUTOFF_WITH_ALPHA) || defined(CUTOFF_ONLY) 
				half _CutOffRange;
			#endif

			#ifdef DISTORTION_ON
				sampler2D _DistortionTex;
				half4 _DistortionTex_ST;
				half _DistortionRate;
			#endif
			
			#ifdef BILLBOARD_ON
				uniform float4 _BillboardMat0;
				uniform float4 _BillboardMat1;
				uniform float4 _BillboardMat2;
			#endif

			#ifdef ROTATE_UV_ON
				half4 _RotateUVParameter;
			#endif

			//uniform half4 unity_FogColor;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			float2 rotateUVs(float2 Texcoords, float2 center, float theta)
			{
				// compute sin and cos for this angle 
				float2 sc;
				sincos( (theta/180.0f*3.14159f), sc.x, sc.y ); 

				// pi to dgree
				//sincos(x,s,c) : sin(x)와 cos(x)를 동시에 s, c로 리턴한다. 여기서 s, c는 x와 동일한 차원의 타입이어야 한다.

				// move the rotation center to the origin : 중점이동 (center는 기초값을 0.5로 하면 중심이 되것지)
				float2 uv = Texcoords - center;

				// rotate the uv : 기본 UV 좌표와의 dot연산 
				float2 rotateduv; 
				rotateduv.x = dot( uv, float2( sc.y, -sc.x ) ); 
				rotateduv.y = dot( uv, sc.xy );

				// move the uv's back to the correct place
				rotateduv += center; 

				return rotateduv;
			}

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
				
				o.uv_ff.xy = TRANSFORM_TEX (v.texcoord, _MainTex) + _FlowSpeed.xy * _Time.y;  

				#ifdef ROTATE_UV_ON
					o.uv_ff.xy = rotateUVs(o.uv_ff.xy, _RotateUVParameter.xy, _RotateUVParameter.z);
				#endif

				o.color = saturate(v.color * _TintColor * 2);
				o.color.rgb *= _ColorIntensity;
				o.color.a *= _ControlAlpha;

				#ifdef DISTORTION_ON
					o.distortion_uv = TRANSFORM_TEX (v.texcoord, _DistortionTex) + _FlowSpeed.zw * _Time.y;  
				#endif

				o.uv_ff.z = GetFogFactor(o.pos, unity_FogStart, unity_FogEnd);
				return o;
			}
 
			fixed4 frag (v2f i) : COLOR
			{
				half2 uv = i.uv_ff.xy;

				#ifdef DISTORTION_ON
					half2 distortionOffset = tex2D(_DistortionTex, i.distortion_uv).xy * 2-1;
					uv += distortionOffset*_DistortionRate;
				#endif

				#ifdef UVCUT_ON
					clip(float4(uv, 1-uv));
				#endif

				fixed4 tex = tex2D(_MainTex, uv) * i.color;

				#if defined(CUTOFF_WITH_ALPHA) || defined(CUTOFF_ONLY) 
					clip(tex.a-_CutOffRange);
				#endif

				#ifdef CUTOFF_ONLY
					tex.a=1;
				#endif

				half ff = i.uv_ff.z;
				ApplyFog(tex, ff, unity_FogColor);

				return tex;
			}
			ENDCG		
		}
	}
CustomEditor "ParticleMaterialInspector"
}