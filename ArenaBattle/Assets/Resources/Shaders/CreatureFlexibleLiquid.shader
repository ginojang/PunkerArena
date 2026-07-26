
Shader "VESTINEL/CHA - Liquid" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask (R:emis/flow G:spec B:alpha)", 2D) = "black" {}
		_FlowTex("Flow (RGB)", 2D) = "white" {}

		_CutOffRange("Cut Off Range", Range(0, 1)) = 0.5

		_Shininess("Specular Shininess", Float) = 8
		_SpecularIntensity("Specular Intensity", Float) = 7
		_SpecularMaterialColor("Specular Material Color", Color) = (1,1,1,1)
		_MainColor("Main Color", Color) = (0.5,0.5,0.5,1)

		_EmissiveColor("Emissive Color", Color) = (1,1,1,1)
		_EmissiveShininess("Emissive Shininess", Float) = 1
		_EmissiveIntensity("Emissive Intensity", Float) = 2
		_EmissiveParameter("Emissive Parameter (x:shininess speed y:blink speed)", Vector) = (2,0,0,0)

		_FlowSpeed("Flow Speed(u,v)", Vector) = (1,1,1,1)
		_FlowIntensity("Flow Intensity", Float) = 1
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent-1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		//Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		LOD 80
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Front
		// Non-lightmapped
		Pass
		{
			Tags{ "LightMode" = "Vertex" }
			Fog{ Mode Off }

			CGPROGRAM

			#pragma vertex vert		//vertex shader naming
			#pragma fragment frag	//fragment shader naming
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0

			#pragma multi_compile CUTOFF_OFF CUTOFF_ON
			#pragma multi_compile SPECULAR_OFF SPECULAR_ON
			#pragma multi_compile FLOW_TEX_OFF FLOW_TEX_ON
			#pragma multi_compile FLOW_MODE_ADD FLOW_MODE_OVERLAY FLOW_MODE_MULTIPLY
			#pragma multi_compile EMISSIVE_OFF EMISSIVE_ON EMISSIVE_ON_RUNTIME

			#define FLEXIBLE_LIQUID

			#include "UnityCG.cginc"
			#include "UtilFunctionsCG.cginc"
			#include "LightFunctions.cginc"
			#include "CreatureRimLightInclude.cginc"
			#include "CreatureFlexibleForward.cginc"

			ENDCG

		}

		// Pass to render object as a shadow caster
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }

			Fog{ Mode Off }
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

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
					return o;
			}

			float4 frag(v2f i) : COLOR
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
			Tags{ "LightMode" = "ShadowCollector" }

			Fog{ Mode Off }
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

			v2f vert(appdata v)
			{
				v2f o;
				TRANSFER_SHADOW_COLLECTOR(o)
					return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				SHADOW_COLLECTOR_FRAGMENT(i)
			}
			ENDCG
		}
	}
	CustomEditor "CreatureFlexibleMaterialInspector"
}
