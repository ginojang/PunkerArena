// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: commented out 'half4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "VESTINEL/ENV - Nature (Transparent baking lightmap)" {
	Properties 
	{
		_MainColor ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_MaskTex ("Mask (R:Spec Mask)", 2D) = "white" {}

		_CutOffRange ("Cut Off Range", Range (0, 1)) = 0.5
	
		_Shininess ("Specular Shininess", Float) = 16
		_SpecularIntensity ("Specular Intensity", Float) = 1
		_SpecularMaterialColor ("Specular Material Color", Color) = (1,1,1,1)

		_WindAxis ("Wind Axis", Vector) = (1,0,0.2,0)
		_WindParameter ("Wind Parameter (x:shuffle y:speed z:power)", Vector) = (0,2,0.1,0)

		[Enum(Opaque, 0, Transparent, 1)]_RenderingType("RenderingType", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("__src", Float) = 1.0
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("__dst", Float) = 0.0
	}
		
	SubShader
	{	
		//Tags { "RenderType"="Opaque" }
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		Blend [_SrcBlend][_DstBlend]
		LOD 80

		// Non-lightmapped	
		Pass 
		{
			Tags { "LightMode" = "Vertex" }		
			Fog {Mode Off}

			CGPROGRAM
			#pragma vertex vert		//vertex shader naming
			#pragma fragment frag	//fragment shader naming
			#pragma fragmentoption ARB_precision_hint_fastest	

			#pragma multi_compile CUTOFF_OFF CUTOFF_ON
			#pragma multi_compile SPECULAR_OFF SPECULAR_ON SPECULAR_MASK_ON

			#include "UnityCG.cginc"
			#include "UtilFunctionsCG.cginc"
			#include "LightFunctions.cginc"
			#include "GrassFunctions.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _MainColor;

			#ifdef CUTOFF_ON
				half _CutOffRange;
			#endif

			#if defined(SPECULAR_ON) || defined(SPECULAR_MASK_ON) 
				half _Shininess;
				half _SpecularIntensity;
				fixed4 _SpecularMaterialColor;
			#endif		

			#ifdef SPECULAR_MASK_ON
				sampler2D _MaskTex;
			#endif

			//uniform half4 unity_FogColor;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			struct VS_IN
			{
				float4 vertex		: POSITION;
				fixed4 color		: COLOR;

				float2 texcoord		: TEXCOORD0;
				float3 normal		: NORMAL;
			};
		
			struct VS_OUT 
			{
				float4 pos			: SV_POSITION;
				half2 uv_MainTex	: TEXCOORD0;	
				fixed3 diff			: COLOR;

				#if defined(SPECULAR_ON) || defined(SPECULAR_MASK_ON) 
					fixed3 spec			: COLOR1;
				#endif

				half fogFactor		: TEXCOORD1;
			};
		
			VS_OUT vert (VS_IN v)
			{
				VS_OUT o;

				float4 pos = GetGrassVertexWithColor(v.vertex, v.color);
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
												o.spec);						

					o.spec *= _SpecularMaterialColor;
				#else
					o.diff = VsDiffuseLighting(viewpos, viewnormal);
				#endif

				o.diff += UNITY_LIGHTMODEL_AMBIENT.xyz;

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
				c.rgb = c.rgb * i.diff * 2;
				//c.rgb = i.diff * 2;

				#ifdef SPECULAR_ON
					c.rgb += i.spec;
				#elif defined(SPECULAR_MASK_ON)
					fixed4 maskTex = tex2D (_MaskTex, i.uv_MainTex);
					c.rgb += i.spec * maskTex.r;
				#endif
				c*=_MainColor*2.0f;

				ApplyFog(c, i.fogFactor, unity_FogColor);

				return c;
			}

			ENDCG
		}

		// Lightmapped, encoded as dLDR
		Pass 
		{
			Tags { "LightMode" = "VertexLM" }
			Fog {Mode Off}

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "UtilFunctionsCG.cginc"
			#include "LightFunctions.cginc"
			#include "GrassFunctions.cginc"

			#pragma multi_compile CUTOFF_OFF CUTOFF_ON
			#pragma multi_compile LIGHTMAP_ONLY_OFF LIGHTMAP_ONLY_ON 
			#pragma multi_compile SPECULAR_OFF SPECULAR_ON SPECULAR_MASK_ON

			#pragma vertex vert		//vertex shader naming
			#pragma fragment frag	//fragment shader naming
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _MainTex;
			half4 _MainTex_ST;

			fixed4 _MainColor;

			// sampler2D unity_Lightmap;
			// half4 unity_LightmapST;

			#ifdef CUTOFF_ON
				half _CutOffRange;
			#endif		

			#if defined(SPECULAR_ON) || defined(SPECULAR_MASK_ON) 
				half _Shininess;
				half _SpecularIntensity;
				fixed4 _SpecularMaterialColor;
			#endif		

			#ifdef SPECULAR_MASK_ON
				sampler2D _MaskTex;
			#endif

			//uniform half4 unity_FogColor;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			struct VS_IN
			{
				float4 vertex		: POSITION;
				fixed4 color		: COLOR;

				float2 texcoord		: TEXCOORD0;
				float2 texcoord1	: TEXCOORD1;

				#ifdef LIGHTMAP_ONLY_OFF	
					float3 normal	: NORMAL;
				#endif
			};

			struct VS_OUT 
			{
				float4 pos			: SV_POSITION;
				half2 uv_MainTex	: TEXCOORD0;		
				half2 lmap			: TEXCOORD1;	
			
				#ifdef LIGHTMAP_ONLY_OFF	
					fixed3 diff			: COLOR;

						#if defined(SPECULAR_ON) || defined(SPECULAR_MASK_ON) 
							fixed3 spec			: COLOR1;
						#endif
				#endif	

				half fogFactor		: TEXCOORD2;
			};

			VS_OUT vert (VS_IN v)
			{
				VS_OUT o;
				
				float4 pos = GetGrassVertexWithColor(v.vertex, v.color);
				o.pos = UnityObjectToClipPos (pos);

				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.fogFactor = GetFogFactor(o.pos, unity_FogStart, unity_FogEnd);
								
				#ifdef LIGHTMAP_ONLY_OFF
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
													o.spec);						

						o.spec *= _SpecularMaterialColor;
					#else
						o.diff = VsDiffuseLighting(viewpos, viewnormal);
					#endif

				#endif			

				return o;
			}

			fixed4 frag(VS_OUT i) : COLOR 
			{
				fixed4 c;
			
				fixed4 mainTex = tex2D (_MainTex, i.uv_MainTex);
				#ifdef CUTOFF_ON
					clip(mainTex.a - _CutOffRange);
				#endif

				fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap.xy);

				#ifdef LIGHTMAP_ONLY_OFF

					c = fixed4((lmtex.rgb * lmtex.a * 2 + i.diff) * 4, 1);

					#ifdef SPECULAR_ON
						c.rgb += i.spec;
					#elif defined(SPECULAR_MASK_ON)
						fixed4 maskTex = tex2D (_MaskTex, i.uv_MainTex);
						c.rgb += i.spec * maskTex.r;
					#endif

				#else
					c = fixed4(lmtex.rgb * lmtex.a * 8, 1);
				#endif

				c *= mainTex*_MainColor*2.0f;			

				ApplyFog(c, i.fogFactor, unity_FogColor);

				return c;
			}

			ENDCG 
		}

		//Lightmap pass, RGBM;
		Pass 
		{
			Tags { "LightMode" = "VertexLMRGBM" }
			Fog {Mode Off}

			CGPROGRAM

			#pragma vertex vert		//vertex shader naming
			#pragma fragment frag	//fragment shader naming
			#pragma fragmentoption ARB_precision_hint_fastest

			#pragma multi_compile CUTOFF_OFF CUTOFF_ON
			#pragma multi_compile LIGHTMAP_ONLY_OFF LIGHTMAP_ONLY_ON 
			#pragma multi_compile SPECULAR_OFF SPECULAR_ON SPECULAR_MASK_ON

			#include "UnityCG.cginc"
			#include "UtilFunctionsCG.cginc"
			#include "LightFunctions.cginc"
			#include "GrassFunctions.cginc"

			sampler2D _MainTex;
			half4 _MainTex_ST;

			fixed4 _MainColor;
						
			// sampler2D unity_Lightmap;
			// half4 unity_LightmapST;

			#ifdef CUTOFF_ON
				half _CutOffRange;
			#endif

			#if defined(SPECULAR_ON) || defined(SPECULAR_MASK_ON) 
				half _Shininess;
				half _SpecularIntensity;
				fixed4 _SpecularMaterialColor;
			#endif		

			#ifdef SPECULAR_MASK_ON
				sampler2D _MaskTex;
			#endif

			//uniform half4 unity_FogColor;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;
			
			struct VS_IN
			{
				float4 vertex		: POSITION;
				fixed4 color		: COLOR;

				float2 texcoord		: TEXCOORD0;
				float2 texcoord1	: TEXCOORD1;

				#ifdef LIGHTMAP_ONLY_OFF
					float3 normal	: NORMAL;
				#endif
			};

			struct VS_OUT 
			{
				float4 pos			: SV_POSITION;
				half2 uv_MainTex	: TEXCOORD0;		
				half2 lmap			: TEXCOORD1;	
			
				#ifdef LIGHTMAP_ONLY_OFF	
					fixed3 diff			: COLOR;

						#if defined(SPECULAR_ON) || defined(SPECULAR_MASK_ON) 
							fixed3 spec			: COLOR1;
						#endif
				#endif	

				half fogFactor		: TEXCOORD2;
			};

			VS_OUT vert (VS_IN v)
			{
				VS_OUT o;
				
				float4 pos = GetGrassVertexWithColor(v.vertex, v.color);
				o.pos = UnityObjectToClipPos (pos);

				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.uv_MainTex = TRANSFORM_TEX (v.texcoord, _MainTex);
				o.fogFactor = GetFogFactor(o.pos, unity_FogStart, unity_FogEnd);
	
				#ifdef LIGHTMAP_ONLY_OFF
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
													o.spec);						

						o.spec *= _SpecularMaterialColor;
					#else
						o.diff = VsDiffuseLighting(viewpos, viewnormal);
					#endif
				#endif			
				
				return o;
			}

			fixed4 frag(VS_OUT i) : COLOR 
			{
				fixed4 c;
			
				fixed4 mainTex = tex2D (_MainTex, i.uv_MainTex);

				#ifdef CUTOFF_ON
					clip(mainTex.a - _CutOffRange);
				#endif

				fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap.xy);

				#ifdef LIGHTMAP_ONLY_OFF

					c = fixed4((lmtex.rgb * lmtex.a * 2 + i.diff) * 4, 1);

					#ifdef SPECULAR_ON
						c.rgb += i.spec;
					#elif defined(SPECULAR_MASK_ON)
						fixed4 maskTex = tex2D (_MaskTex, i.uv_MainTex);
						c.rgb += i.spec * maskTex.r;
					#endif

				#else
					c = fixed4(lmtex.rgb * lmtex.a * 8, 1);
				#endif

				c *= mainTex*_MainColor*2.0f;		

				ApplyFog(c, i.fogFactor, unity_FogColor);
				return c;
			}

			ENDCG
		}

		// Pass to render object as a shadow caster
		Pass 
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
		
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f { 
				V2F_SHADOW_CASTER;
			};

			v2f vert( appdata_base v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}

			float4 frag( v2f i ) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	
		// Pass to render object as a shadow collector
		// note: editor needs this pass as it has a collector pass.
		Pass
		{
			Name "ShadowCollector"
			Tags { "LightMode" = "ShadowCollector" }
		
			Fog {Mode Off}
			ZWrite On ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcollector

			#define SHADOW_COLLECTOR_PASS
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				V2F_SHADOW_COLLECTOR;
			};

			v2f vert (appdata v)
			{
				v2f o;
				TRANSFER_SHADOW_COLLECTOR(o)
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				SHADOW_COLLECTOR_FRAGMENT(i)
			}
			ENDCG
		}
	}
	
//Fallback "Transparent/VertexLit"
CustomEditor "ObjectMaterialInspector"
}