// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VESTINEL/Particles/Particle Object" {
	Properties 
	{
		_MainColor ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_MaskTex ("Mask (R:Spec Mask)", 2D) = "white" {}

		_CutOffRange ("Cut Off Range", Range (0, 1)) = 0.5

		_Shininess ("Specular Shininess", Float) = 16
		_SpecularIntensity ("Specular Intensity", Float) = 1
		_SpecularMaterialColor ("Specular Material Color", Color) = (1,1,1,1)

		[HideInInspector]_ObjectDamageValue ("Damage Value", Vector) = (0,0,0,0)

		[HideInInspector]_BillboardMat0 ("BillboardMat0", Vector) = (1,0,0,0)
		[HideInInspector]_BillboardMat1 ("BillboardMat1", Vector) = (0,1,0,0)
		[HideInInspector]_BillboardMat2 ("BillboardMat2", Vector) = (0,0,1,0)
	}
		
	SubShader
	{	
		//Tags { "RenderType"="Opaque" }
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		LOD 80

		// Non-lightmapped	
		Pass 
		{
			Tags { "LightMode" = "Vertex" }		
			Fog {Mode Off}

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "../UtilFunctionsCG.cginc"
			#include "../LightFunctions.cginc"
			#include "../ObjectFunctions.cginc"

			#pragma multi_compile DAMAGE_EFF_OFF DAMAGE_EFF_ON
			#pragma multi_compile CUTOFF_OFF CUTOFF_ON
			#pragma multi_compile SPECULAR_OFF SPECULAR_ON SPECULAR_MASK_ON
			#pragma multi_compile BILLBOARD_OFF BILLBOARD_ON

			#pragma vertex vert		//vertex shader naming
			#pragma fragment frag	//fragment shader naming
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _MainColor;

			#ifdef CUTOFF_ON
				half _CutOffRange;
			#endif

			#ifdef DAMAGE_EFF_ON
				fixed4 _ObjectDamageValue;
			#endif

			#if defined(SPECULAR_ON) || defined(SPECULAR_MASK_ON) 
				half _Shininess;
				half _SpecularIntensity;
				fixed4 _SpecularMaterialColor;
			#endif		

			#ifdef SPECULAR_MASK_ON
				sampler2D _MaskTex;
			#endif

			#ifdef BILLBOARD_ON
				uniform float4 _BillboardMat0;
				uniform float4 _BillboardMat1;
				uniform float4 _BillboardMat2;
			#endif

			//uniform half4 unity_FogColor;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			struct VS_IN
			{
				float4 vertex		: POSITION;
				float2 texcoord		: TEXCOORD0;
				float3 normal		: NORMAL;
			};
		
			struct VS_OUT 
			{
				float4 pos			: SV_POSITION;
				half2 uv_MainTex	: TEXCOORD0;	
				fixed3 diff			: COLOR;

				#if defined(SPECULAR_ON) || defined(SPECULAR_MASK_ON) 
					#ifdef DAMAGE_EFF_ON
						fixed4 spec_dam		: COLOR1;
					#else
						fixed3 spec_dam		: COLOR1;
					#endif
				#else
					#ifdef DAMAGE_EFF_ON
						fixed spec_dam		: COLOR1;
					#endif
				#endif

				half fogFactor		: TEXCOORD1;
			};
		
			VS_OUT vert (VS_IN v)
			{
				VS_OUT o;

				#ifdef BILLBOARD_ON				
					float4 p = v.vertex;
					v.vertex.x = dot(_BillboardMat0, p);
					v.vertex.y = dot(_BillboardMat1, p);
					v.vertex.z = dot(_BillboardMat2, p);
				#endif
				o.pos = UnityObjectToClipPos (v.vertex);

				float4 pos = GetLocalVertex(v.vertex);
				o.pos = UnityObjectToClipPos (pos);

				o.uv_MainTex = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.fogFactor = GetFogFactor(o.pos, unity_FogStart, unity_FogEnd);

				float3 viewpos = ViewPos(v.vertex);
				half3 viewnormal = ViewNormal(v.normal);

				#if defined(SPECULAR_ON) || defined(SPECULAR_MASK_ON) 
					half3 sight = -normalize(viewpos);
					VsDiffuseSpecularLighting(	viewpos, 
												viewnormal, 
												sight,
												_Shininess, 
												_SpecularIntensity, 
												o.diff, 
												o.spec_dam.rgb);						

					o.spec_dam.rgb *= _SpecularMaterialColor;
				#else
					o.diff = VsDiffuseLighting(viewpos, viewnormal);
				#endif

				#ifdef DAMAGE_EFF_ON
					fixed dam = RimFactor(viewnormal, _ObjectDamageValue.w);
					#ifdef SPECULAR_ON		
						o.spec_dam.w = dam;
					#else
						o.spec_dam.x = dam;
					#endif					
				#endif

				o.diff += UNITY_LIGHTMODEL_AMBIENT.xyz;
				o.diff *= 2;

				return o;
			}

			fixed4 frag(VS_OUT i) : COLOR 
			{
				fixed4 c;

				fixed4 mainTex = tex2D (_MainTex, i.uv_MainTex);
				#ifdef CUTOFF_ON
					clip(mainTex.a - _CutOffRange);
				#endif

				c = mainTex;
				c.rgb = c.rgb * i.diff;

				#ifdef SPECULAR_ON
					c.rgb += i.spec_dam.rgb;
				#elif defined(SPECULAR_MASK_ON)
					fixed4 maskTex = tex2D (_MaskTex, i.uv_MainTex);
					c.rgb += i.spec_dam.rgb * maskTex.r;
				#endif
				c*=_MainColor;

				#ifdef DAMAGE_EFF_ON
					fixed dam;
					#ifdef SPECULAR_ON
						dam = i.spec_dam.w;
					#else
						dam = i.spec_dam.x;
					#endif
					c.rgb = lerp(c.rgb, _ObjectDamageValue.rgb, dam);
				#endif

				ApplyFog(c, i.fogFactor, unity_FogColor);

				return c;
			}

			ENDCG
		}
	}
	
//Fallback "Transparent/VertexLit"
CustomEditor "ObjectMaterialInspector"
}