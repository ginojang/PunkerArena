// Upgrade NOTE: upgraded instancing buffer 'FruitDinoFXShadersFXWave001Shadow' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FruitDino FX Shaders/FX Wave 001 Shadow"
{
	Properties
	{
		_WaveTex1("WaveTex 1", 2D) = "white" {}
		_WaveTex1UTile("WaveTex 1 U Tile", Float) = 1
		_WaveTex1VTile("WaveTex 1 V Tile", Float) = 1
		_WaveTex1UOffset("WaveTex 1 U Offset", Float) = 0
		_WaveTex1VOffset("WaveTex 1 V Offset", Float) = 0
		_WaveTex2("WaveTex 2", 2D) = "white" {}
		_WaveTex2Mask("WaveTex2 Mask", 2D) = "white" {}
		_WaveTex2UTile("WaveTex 2 U Tile", Float) = 1
		_WaveTex2VTile("WaveTex 2 V Tile", Float) = 1
		_WaveTex2UOffset("WaveTex 2 U Offset", Float) = 0
		_WaveTex2VOffset("WaveTex 2 V Offset", Float) = 0
		_DissolveTex("DissolveTex", 2D) = "white" {}
		_NoisePower("Noise Power", Float) = 1
		_Noise1Power("Noise 1 Power", Float) = 1
		_Noise1UTile("Noise 1 U Tile", Float) = 1
		_Noise1VTile("Noise 1 V Tile", Float) = 1
		_Noise1USpeed("Noise 1 U Speed", Float) = 0
		_Noise1VSpeed("Noise 1 V Speed", Float) = 0
		_Noise2Power("Noise 2 Power", Float) = 1
		_Noise2UTile("Noise 2 U Tile", Float) = 1
		_Noise2VTile("Noise 2 V Tile", Float) = 1
		_Noise2USpeed("Noise 2 U Speed", Float) = 0
		_Noise2VSpeed("Noise 2 V Speed", Float) = 0
		_Dissolve_Tex_Range("Dissolve_Tex_Range", Float) = 0
		_Dissolve_Shappeness("Dissolve_Shappeness", Range( 0 , 1)) = 1
		[Enum(UnityEngine.Rendering.BlendMode)]_ScrBlendMod("ScrBlend Mod", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_DstBlendMod("DstBlend Mod", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		ZWrite Off
		Blend [_ScrBlendMod] [_DstBlendMod]
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#pragma multi_compile_instancing
		#pragma only_renderers d3d11 glcore gles gles3 metal vulkan 
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv3_texcoord3;
			float2 uv_texcoord;
			float4 uv2_texcoord2;
			float4 vertexColor : COLOR;
		};

		uniform float _CullMode;
		uniform float _DstBlendMod;
		uniform float _ScrBlendMod;
		uniform sampler2D _WaveTex2;
		uniform float _NoisePower;
		uniform float _Noise1USpeed;
		uniform float _Noise1VSpeed;
		uniform float _Noise1UTile;
		uniform float _Noise1VTile;
		uniform float _Noise1Power;
		uniform float _Noise2USpeed;
		uniform float _Noise2VSpeed;
		uniform float _Noise2UTile;
		uniform float _Noise2VTile;
		uniform float _Noise2Power;
		uniform sampler2D _WaveTex2Mask;
		uniform float _WaveTex2UTile;
		uniform float _WaveTex2VTile;
		uniform float _WaveTex2UOffset;
		uniform float _WaveTex2VOffset;
		uniform sampler2D _WaveTex1;
		uniform float _WaveTex1UTile;
		uniform float _WaveTex1VTile;
		uniform float _WaveTex1UOffset;
		uniform float _WaveTex1VOffset;
		uniform sampler2D _DissolveTex;
		uniform float _Dissolve_Shappeness;

		UNITY_INSTANCING_BUFFER_START(FruitDinoFXShadersFXWave001Shadow)
			UNITY_DEFINE_INSTANCED_PROP(float4, _DissolveTex_ST)
#define _DissolveTex_ST_arr FruitDinoFXShadersFXWave001Shadow
			UNITY_DEFINE_INSTANCED_PROP(float, _Dissolve_Tex_Range)
#define _Dissolve_Tex_Range_arr FruitDinoFXShadersFXWave001Shadow
		UNITY_INSTANCING_BUFFER_END(FruitDinoFXShadersFXWave001Shadow)


		float2 voronoihash67( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi67( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash67( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult138 = (float2(i.uv3_texcoord3.x , i.uv3_texcoord3.y));
			float2 appendResult139 = (float2(i.uv3_texcoord3.z , i.uv3_texcoord3.w));
			float2 uv_TexCoord140 = i.uv_texcoord * appendResult138 + appendResult139;
			float time67 = _Time.y;
			float2 voronoiSmoothId67 = 0;
			float2 appendResult49 = (float2(_Noise1USpeed , _Noise1VSpeed));
			float2 appendResult48 = (float2(_Noise1UTile , _Noise1VTile));
			float2 uv_TexCoord58 = i.uv_texcoord * appendResult48;
			float2 panner59 = ( 1.0 * _Time.y * appendResult49 + uv_TexCoord58);
			float2 coords67 = panner59 * 3.0;
			float2 id67 = 0;
			float2 uv67 = 0;
			float voroi67 = voronoi67( coords67, time67, id67, uv67, 0, voronoiSmoothId67 );
			float2 appendResult84 = (float2(_Noise2USpeed , _Noise2VSpeed));
			float2 appendResult82 = (float2(_Noise2UTile , _Noise2VTile));
			float2 uv_TexCoord83 = i.uv_texcoord * appendResult82;
			float2 panner85 = ( 1.0 * _Time.y * appendResult84 + uv_TexCoord83);
			float simplePerlin2D74 = snoise( panner85*2.0 );
			simplePerlin2D74 = simplePerlin2D74*0.5 + 0.5;
			float2 appendResult64 = (float2(0.0 , ( _NoisePower * pow( voroi67 , _Noise1Power ) * pow( simplePerlin2D74 , _Noise2Power ) )));
			float2 appendResult146 = (float2(_WaveTex2UTile , _WaveTex2VTile));
			float2 appendResult147 = (float2(_WaveTex2UOffset , _WaveTex2VOffset));
			float2 uv_TexCoord148 = i.uv_texcoord * appendResult146 + appendResult147;
			float2 appendResult31 = (float2(_WaveTex1UTile , _WaveTex1VTile));
			float2 appendResult32 = (float2(_WaveTex1UOffset , _WaveTex1VOffset));
			float2 uv_TexCoord23 = i.uv_texcoord * appendResult31 + appendResult32;
			float4 _DissolveTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_DissolveTex_ST_arr, _DissolveTex_ST);
			float2 uv_DissolveTex = i.uv_texcoord * _DissolveTex_ST_Instance.xy + _DissolveTex_ST_Instance.zw;
			float _Dissolve_Tex_Range_Instance = UNITY_ACCESS_INSTANCED_PROP(_Dissolve_Tex_Range_arr, _Dissolve_Tex_Range);
			float lerpResult93 = lerp( _Dissolve_Tex_Range_Instance , 1.0 , i.uv2_texcoord2.x);
			float smoothstepResult95 = smoothstep( saturate( ( 1.0 - tex2D( _DissolveTex, uv_DissolveTex ).r ) ) , 1.0 , lerpResult93);
			float temp_output_124_0 = ( ( tex2D( _WaveTex2, ( uv_TexCoord140 + appendResult64 ) ).r * tex2D( _WaveTex2Mask, ( uv_TexCoord148 + appendResult64 ) ).r * i.uv2_texcoord2.y ) + ( tex2D( _WaveTex1, ( uv_TexCoord23 + appendResult64 ) ).r * ( smoothstepResult95 / ( 1.0 - _Dissolve_Shappeness ) ) ) );
			o.Emission = ( temp_output_124_0 * i.vertexColor ).rgb;
			o.Alpha = ( temp_output_124_0 * i.vertexColor.a );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1694;91;1655;1171;3207.33;1745.918;2.384032;True;True
Node;AmplifyShaderEditor.CommentaryNode;50;-3895.354,529.1382;Inherit;False;1514.208;1113.558;Noise;23;86;76;77;74;75;67;73;85;84;83;82;81;80;79;78;59;49;58;48;46;47;44;45;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-3801.504,1214.582;Inherit;False;Property;_Noise2VTile;Noise 2 V Tile;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-3797.604,1120.986;Inherit;False;Property;_Noise2UTile;Noise 2 U Tile;19;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-3807.129,694.7253;Inherit;False;Property;_Noise1VTile;Noise 1 V Tile;15;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-3817.714,600.6147;Inherit;False;Property;_Noise1UTile;Noise 1 U Tile;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-3811.127,1333.504;Inherit;False;Property;_Noise2USpeed;Noise 2 U Speed;21;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-3808.221,1435.971;Inherit;False;Property;_Noise2VSpeed;Noise 2 V Speed;22;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;82;-3594.612,1199.573;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-3813.846,916.1144;Inherit;False;Property;_Noise1VSpeed;Noise 1 V Speed;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-3816.752,813.6475;Inherit;False;Property;_Noise1USpeed;Noise 1 U Speed;16;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;48;-3600.237,679.7156;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;83;-3447.375,1180.917;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;84;-3594.266,1366.654;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;49;-3599.891,846.7974;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;58;-3453,661.0599;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;85;-3213.531,1289.101;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;73;-3230.071,938.4654;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;59;-3219.156,769.2438;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-2916.837,1056.028;Inherit;False;Property;_Noise1Power;Noise 1 Power;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;98;-1798.72,1334.668;Inherit;True;Property;_DissolveTex;DissolveTex;11;0;Create;True;0;0;0;False;0;False;-1;6d8b7ab979ab9654aab7640a47b9f529;8c35f07d8aa38d4469127b73710415c2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;74;-2969.121,1283.074;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-2895.027,1527.331;Inherit;False;Property;_Noise2Power;Noise 2 Power;18;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;67;-2913.925,762.8849;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;3;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.TexCoordVertexDataNode;150;-3035.228,-1139.361;Inherit;True;2;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;30;-2756.733,-91.1384;Inherit;False;Property;_WaveTex1VOffset;WaveTex 1 V Offset;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-2756.002,-307.522;Inherit;False;Property;_WaveTex1VTile;WaveTex 1 V Tile;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;106;-1778.468,1030.529;Inherit;True;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;89;-1376.369,1264.174;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;75;-2669.317,1279.684;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-2247.512,401.2341;Inherit;False;Property;_NoisePower;Noise Power;12;0;Create;True;0;0;0;False;0;False;1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;77;-2671.274,765.3354;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-2761.552,-639.017;Inherit;False;Property;_WaveTex2UOffset;WaveTex 2 U Offset;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-2761.906,-407.5917;Inherit;False;Property;_WaveTex1UTile;WaveTex 1 U Tile;1;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-2764.171,-856.3011;Inherit;False;Property;_WaveTex2UTile;WaveTex 2 U Tile;7;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2759.288,-190.3078;Inherit;False;Property;_WaveTex1UOffset;WaveTex 1 U Offset;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-2758.267,-757.949;Inherit;False;Property;_WaveTex2VTile;WaveTex 2 V Tile;8;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;145;-2758.998,-545.0015;Inherit;False;Property;_WaveTex2VOffset;WaveTex 2 V Offset;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-1358.952,838.9166;Inherit;False;InstancedProperty;_Dissolve_Tex_Range;Dissolve_Tex_Range;23;0;Create;True;0;0;0;False;0;False;0;2.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;147;-2467.284,-611.112;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;146;-2457.56,-802.298;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1977.916,404.2061;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;31;-2455.295,-353.5885;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;94;-1113.01,1205.847;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;93;-1095.26,851.6261;Inherit;True;3;0;FLOAT;0.21;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-876.0908,1540.564;Inherit;False;Property;_Dissolve_Shappeness;Dissolve_Shappeness;24;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;138;-2489.959,-1237.672;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-2460.165,-162.4035;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;139;-2499.683,-1046.486;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-2180.811,61.87384;Inherit;False;Constant;_none;none;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-1970.688,116.2962;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;148;-2150.546,-735.0251;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-2148.281,-286.3163;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;95;-670.5988,988.4262;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;140;-2182.945,-1170.399;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;96;-551.0316,1528.715;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;97;-392.1221,771.9889;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;141;-1782.335,-949.8468;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;149;-1726.282,-675.1922;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-1745.401,-285.9953;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;111;-1449.609,-544.0745;Inherit;True;Property;_WaveTex2Mask;WaveTex2 Mask;6;0;Create;True;0;0;0;False;0;False;-1;9d61fe1abf2674b43aebdbeb1ea57a6d;9d61fe1abf2674b43aebdbeb1ea57a6d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;163;-893.4394,609.7776;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1427.568,-76.4567;Inherit;True;Property;_WaveTex1;WaveTex 1;0;0;Create;True;0;0;0;False;0;False;-1;75bcce8bd12bf694d9fa7fd14a0d545e;75bcce8bd12bf694d9fa7fd14a0d545e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;122;-1457.833,-948.9153;Inherit;True;Property;_WaveTex2;WaveTex 2;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-883.3105,108.4245;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-947.4656,-321.9786;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;6;-241.7242,42.12138;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;20;355.0567,450.8857;Inherit;False;250.767;346.4162;BlendMod;3;4;5;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;124;-637.0915,-22.44876;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;133;107.2998,252.4845;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;406.59,681.3015;Inherit;False;Property;_CullMode;Cull Mode;27;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;86.8971,-129.3058;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;4;405.0567,594.4511;Inherit;False;Property;_DstBlendMod;DstBlend Mod;26;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;405.8237,500.8857;Inherit;False;Property;_ScrBlendMod;ScrBlend Mod;25;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;408.2206,-84.19146;Float;False;True;-1;0;ASEMaterialInspector;0;0;Unlit;FruitDino FX Shaders/FX Wave 001 Shadow;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;6;d3d11;glcore;gles;gles3;metal;vulkan;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;1;5;True;3;10;True;4;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;28;-1;-1;-1;0;False;0;0;True;5;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;82;0;79;0
WireConnection;82;1;78;0
WireConnection;48;0;44;0
WireConnection;48;1;45;0
WireConnection;83;0;82;0
WireConnection;84;0;81;0
WireConnection;84;1;80;0
WireConnection;49;0;46;0
WireConnection;49;1;47;0
WireConnection;58;0;48;0
WireConnection;85;0;83;0
WireConnection;85;2;84;0
WireConnection;59;0;58;0
WireConnection;59;2;49;0
WireConnection;74;0;85;0
WireConnection;67;0;59;0
WireConnection;67;1;73;2
WireConnection;89;0;98;1
WireConnection;75;0;74;0
WireConnection;75;1;86;0
WireConnection;77;0;67;0
WireConnection;77;1;76;0
WireConnection;147;0;144;0
WireConnection;147;1;145;0
WireConnection;146;0;142;0
WireConnection;146;1;143;0
WireConnection;60;0;42;0
WireConnection;60;1;77;0
WireConnection;60;2;75;0
WireConnection;31;0;27;0
WireConnection;31;1;28;0
WireConnection;94;0;89;0
WireConnection;93;0;90;0
WireConnection;93;2;106;1
WireConnection;138;0;150;1
WireConnection;138;1;150;2
WireConnection;32;0;29;0
WireConnection;32;1;30;0
WireConnection;139;0;150;3
WireConnection;139;1;150;4
WireConnection;64;0;65;0
WireConnection;64;1;60;0
WireConnection;148;0;146;0
WireConnection;148;1;147;0
WireConnection;23;0;31;0
WireConnection;23;1;32;0
WireConnection;95;0;93;0
WireConnection;95;1;94;0
WireConnection;140;0;138;0
WireConnection;140;1;139;0
WireConnection;96;0;92;0
WireConnection;97;0;95;0
WireConnection;97;1;96;0
WireConnection;141;0;140;0
WireConnection;141;1;64;0
WireConnection;149;0;148;0
WireConnection;149;1;64;0
WireConnection;39;0;23;0
WireConnection;39;1;64;0
WireConnection;111;1;149;0
WireConnection;163;0;97;0
WireConnection;1;1;39;0
WireConnection;122;1;141;0
WireConnection;10;0;1;1
WireConnection;10;1;163;0
WireConnection;129;0;122;1
WireConnection;129;1;111;1
WireConnection;129;2;106;2
WireConnection;124;0;129;0
WireConnection;124;1;10;0
WireConnection;133;0;124;0
WireConnection;133;1;6;4
WireConnection;159;0;124;0
WireConnection;159;1;6;0
WireConnection;0;2;159;0
WireConnection;0;9;133;0
ASEEND*/
//CHKSM=70D5E85D0D87B860F49186D4FE6E242FB7CF2DD9