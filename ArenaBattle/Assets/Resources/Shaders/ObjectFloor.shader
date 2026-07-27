// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VESTINEL/ENV GROUND - Static (baking lightmap)" 
{
	Properties
	{
		_MainTex("Base (RGBA)", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue"="Geometry"}

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }	// pass for 4 vertex lights, ambient light & first pixel light (directional light)

			CGPROGRAM

			// Apparently need to add this declaration 
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "ObjectFunctions.cginc"
			#include "UtilFunctionsCG.cginc"

			struct appdata_t 
			{
				float4 vertex		: POSITION;
				float3 normal		: NORMAL;
				float2 texcoord		: TEXCOORD0;
				float2 texcoord1	: TEXCOORD1;
			};

			struct v2f 
			{
				float4 pos				: SV_POSITION;
				half2 uv_MainTex		: TEXCOORD0;
				half2 lmap				: TEXCOORD1;
				half fogFactor			: TEXCOORD2;
				LIGHTING_COORDS(3, 4)

			};

			sampler2D _MainTex;
			half4 _MainTex_ST;

			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			v2f vert(appdata_t v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv_MainTex = v.texcoord;
				o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				o.fogFactor = GetFogFactor(o.pos, unity_FogStart, unity_FogEnd);

				/*float3 vertexLight = Shade4PointLights(
				unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
				unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
				unity_4LightAtten0, mul(_Object2World, i.vertex), normalize(mul(float4(i.normal, 0.0), _World2Object).xyz));
				o.vertexLighting = vertexLight;*/

				// pass lighting information to pixel shader
				TRANSFER_VERTEX_TO_FRAGMENT(o)
				return o;
			}

			float4 frag(v2f i) : COLOR
			{

				fixed4 mainTex = tex2D(_MainTex, i.uv_MainTex);

				float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 diffuseReflection = LIGHT_ATTENUATION(i) * _LightColor0.rgb;

				fixed4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap.xy);

				float4 c = fixed4(ambientLighting + diffuseReflection * DecodeLightmap(lmtex), 1);

				c *= mainTex;

				ApplyFog(c, i.fogFactor, unity_FogColor);

				return c;
			}
			ENDCG
		
		}
	}
}
