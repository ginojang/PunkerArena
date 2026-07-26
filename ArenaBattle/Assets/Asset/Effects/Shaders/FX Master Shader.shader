// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FruitDino FX Shaders/FX Master Shader"
{
	Properties
	{
		[HDR]_TintColor("TintColor", Color) = (1,1,1,1)
		_Emissive("Emissive", Float) = 1
		_MainTex("MainTex", 2D) = "white" {}
		_MainTexUTile("MainTex U Tile", Float) = 1
		_MainTexVTile("MainTex V Tile", Float) = 1
		_MainTexUOffset("MainTex U Offset", Float) = 0
		_MainTexVOffset("MainTex V Offset", Float) = 0
		[ToggleUI]_UVCustomVertexStreamUse("UV CustomVertexStream Use", Float) = 1
		_MaskTex("MaskTex", 2D) = "white" {}
		[Toggle]_NoiseUse("Noise Use", Float) = 0
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
		[Toggle]_RimlightUse("Rimlight Use", Float) = 0
		[HDR]_RimlightColor("Rimlight Color", Color) = (1,1,1,1)
		_RimlightBias("Rimlight Bias", Float) = 0
		_RimlightScale("Rimlight Scale", Float) = 1
		_RimlightPower("Rimlight Power", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)]_ScrBlendMod("ScrBlend Mod", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_DstBlendMod("DstBlend Mod", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
		#pragma only_renderers d3d11 glcore gles gles3 metal vulkan 
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float4 uv2_texcoord2;
			float4 vertexColor : COLOR;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _DstBlendMod;
		uniform float _ScrBlendMod;
		uniform float _CullMode;
		uniform float _RimlightUse;
		uniform sampler2D _MainTex;
		uniform float _NoiseUse;
		uniform float _UVCustomVertexStreamUse;
		uniform float _MainTexUTile;
		uniform float _MainTexVTile;
		uniform float _MainTexUOffset;
		uniform float _MainTexVOffset;
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
		uniform float _Emissive;
		uniform float4 _TintColor;
		uniform float4 _RimlightColor;
		uniform float _RimlightBias;
		uniform float _RimlightScale;
		uniform float _RimlightPower;
		uniform sampler2D _MaskTex;


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
			float2 appendResult31 = (float2(_MainTexUTile , _MainTexVTile));
			float2 appendResult32 = (float2(_MainTexUOffset , _MainTexVOffset));
			float2 uv_TexCoord23 = i.uv_texcoord * appendResult31 + appendResult32;
			float2 appendResult22 = (float2(i.uv2_texcoord2.x , i.uv2_texcoord2.y));
			float2 appendResult25 = (float2(i.uv2_texcoord2.z , i.uv2_texcoord2.w));
			float2 uv_TexCoord24 = i.uv_texcoord * appendResult22 + appendResult25;
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
			float4 tex2DNode1 = tex2D( _MainTex, (( _NoiseUse )?( ( (( _UVCustomVertexStreamUse )?( uv_TexCoord24 ):( uv_TexCoord23 )) + appendResult64 ) ):( (( _UVCustomVertexStreamUse )?( uv_TexCoord24 ):( uv_TexCoord23 )) )) );
			float4 temp_output_2_0 = ( tex2DNode1 * i.vertexColor * _Emissive * _TintColor );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV14 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode14 = ( _RimlightBias + _RimlightScale * pow( 1.0 - fresnelNdotV14, _RimlightPower ) );
			o.Emission = (( _RimlightUse )?( ( ( _RimlightColor * fresnelNode14 ) + temp_output_2_0 ) ):( temp_output_2_0 )).rgb;
			o.Alpha = ( tex2DNode1.a * i.vertexColor.a * tex2D( _MaskTex, i.uv_texcoord ).r * _TintColor.a );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-2560;3;2560;1376;3196.407;970.0549;1.722848;True;True
Node;AmplifyShaderEditor.CommentaryNode;50;-3283.356,568.5861;Inherit;False;1514.208;1113.558;Noise;23;86;76;77;74;75;67;73;85;84;83;82;81;80;79;78;59;49;58;48;46;47;44;45;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-3189.507,1254.03;Inherit;False;Property;_Noise2VTile;Noise 2 V Tile;18;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-3185.606,1160.434;Inherit;False;Property;_Noise2UTile;Noise 2 U Tile;17;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-3195.132,734.1732;Inherit;False;Property;_Noise1VTile;Noise 1 V Tile;13;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-3205.717,640.0627;Inherit;False;Property;_Noise1UTile;Noise 1 U Tile;12;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-3196.224,1475.419;Inherit;False;Property;_Noise2VSpeed;Noise 2 V Speed;20;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-3199.13,1372.952;Inherit;False;Property;_Noise2USpeed;Noise 2 U Speed;19;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-3201.849,955.5623;Inherit;False;Property;_Noise1VSpeed;Noise 1 V Speed;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-3204.755,853.0953;Inherit;False;Property;_Noise1USpeed;Noise 1 U Speed;14;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;48;-2988.239,719.1636;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;82;-2982.614,1239.021;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;84;-2982.269,1406.102;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;58;-2841.003,700.5078;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;83;-2835.378,1220.365;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;49;-2987.894,886.2452;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;85;-2601.532,1328.549;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;59;-2607.157,808.6918;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;73;-2618.073,977.9132;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;35;-2729.825,3.587312;Inherit;False;877.8597;342.0667;UV CustomVertexStream;4;21;22;25;24;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2567.627,-264.9054;Inherit;False;Property;_MainTexUOffset;MainTex U Offset;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-2570.245,-482.1892;Inherit;False;Property;_MainTexUTile;MainTex U Tile;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-2283.029,1566.779;Inherit;False;Property;_Noise2Power;Noise 2 Power;16;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-2304.839,1095.476;Inherit;False;Property;_Noise1Power;Noise 1 Power;11;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;67;-2301.926,802.3329;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;3;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.NoiseGeneratorNode;74;-2357.123,1322.522;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-2564.341,-383.8377;Inherit;False;Property;_MainTexVTile;MainTex V Tile;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;21;-2679.825,82.02866;Inherit;True;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;30;-2565.072,-170.8899;Inherit;False;Property;_MainTexVOffset;MainTex V Offset;6;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;25;-2336.158,210.6536;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;77;-2059.277,804.7832;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-1863.689,410.2585;Inherit;False;Property;_NoisePower;Noise Power;10;0;Create;True;0;0;0;False;0;False;1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;75;-2057.32,1319.132;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-2268.504,-237.0011;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;31;-2263.634,-428.1862;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-2333.393,53.5872;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1585.921,229.171;Inherit;False;Constant;_none;none;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1594.093,413.2304;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-2093.966,113.2752;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-2027.066,-373.494;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;64;-1450.32,292.9086;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;26;-1745.891,34.57784;Inherit;False;Property;_UVCustomVertexStreamUse;UV CustomVertexStream Use;7;0;Create;True;0;0;0;False;0;False;1;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-1351.534,138.6382;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;11;-1034.385,-686.4702;Inherit;False;895.5863;564.9632;Rimlight;6;16;15;14;13;12;66;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-999.2264,-439.5432;Inherit;False;Property;_RimlightBias;Rimlight Bias;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;40;-1177.745,38.77531;Inherit;False;Property;_NoiseUse;Noise Use;9;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1007.015,-330.3685;Inherit;False;Property;_RimlightScale;Rimlight Scale;24;0;Create;True;0;0;0;False;0;False;1;1.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1006.675,-240.999;Inherit;False;Property;_RimlightPower;Rimlight Power;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;88;-491.5359,544.3283;Inherit;False;Property;_TintColor;TintColor;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;6;-470.3984,141.9902;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;14;-757.5285,-397.8477;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;15;-715.5267,-626.0646;Inherit;False;Property;_RimlightColor;Rimlight Color;22;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,0.8916426,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-474.1614,-60.62637;Inherit;False;Property;_Emissive;Emissive;1;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-903.6443,15.66763;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-433.6761,-489.0574;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;63;-1301.792,389.4034;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-226.9001,23.94225;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;-57.26192,-58.35852;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;37;-909.0539,356.9244;Inherit;True;Property;_MaskTex;MaskTex;8;0;Create;True;0;0;0;False;0;False;-1;None;8c35f07d8aa38d4469127b73710415c2;True;1;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;20;355.0567,450.8857;Inherit;False;250.767;346.4162;BlendMod;3;4;5;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;5;406.59,681.3015;Inherit;False;Property;_CullMode;Cull Mode;28;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;18;115.4609,24.14479;Inherit;False;Property;_RimlightUse;Rimlight Use;21;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3;405.8237,500.8857;Inherit;False;Property;_ScrBlendMod;ScrBlend Mod;26;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;405.0567,594.4511;Inherit;False;Property;_DstBlendMod;DstBlend Mod;27;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-218.9308,334.0819;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;376.7365,-19.12409;Float;False;True;-1;0;ASEMaterialInspector;0;0;Unlit;FruitDino FX Shaders/FX Master Shader;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;6;d3d11;glcore;gles;gles3;metal;vulkan;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;1;5;True;3;10;True;4;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;29;-1;-1;-1;0;False;0;0;True;5;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;48;0;44;0
WireConnection;48;1;45;0
WireConnection;82;0;79;0
WireConnection;82;1;78;0
WireConnection;84;0;81;0
WireConnection;84;1;80;0
WireConnection;58;0;48;0
WireConnection;83;0;82;0
WireConnection;49;0;46;0
WireConnection;49;1;47;0
WireConnection;85;0;83;0
WireConnection;85;2;84;0
WireConnection;59;0;58;0
WireConnection;59;2;49;0
WireConnection;67;0;59;0
WireConnection;67;1;73;2
WireConnection;74;0;85;0
WireConnection;25;0;21;3
WireConnection;25;1;21;4
WireConnection;77;0;67;0
WireConnection;77;1;76;0
WireConnection;75;0;74;0
WireConnection;75;1;86;0
WireConnection;32;0;29;0
WireConnection;32;1;30;0
WireConnection;31;0;27;0
WireConnection;31;1;28;0
WireConnection;22;0;21;1
WireConnection;22;1;21;2
WireConnection;60;0;42;0
WireConnection;60;1;77;0
WireConnection;60;2;75;0
WireConnection;24;0;22;0
WireConnection;24;1;25;0
WireConnection;23;0;31;0
WireConnection;23;1;32;0
WireConnection;64;0;65;0
WireConnection;64;1;60;0
WireConnection;26;0;23;0
WireConnection;26;1;24;0
WireConnection;39;0;26;0
WireConnection;39;1;64;0
WireConnection;40;0;26;0
WireConnection;40;1;39;0
WireConnection;14;1;66;0
WireConnection;14;2;12;0
WireConnection;14;3;13;0
WireConnection;1;1;40;0
WireConnection;16;0;15;0
WireConnection;16;1;14;0
WireConnection;2;0;1;0
WireConnection;2;1;6;0
WireConnection;2;2;9;0
WireConnection;2;3;88;0
WireConnection;19;0;16;0
WireConnection;19;1;2;0
WireConnection;37;1;63;0
WireConnection;18;0;2;0
WireConnection;18;1;19;0
WireConnection;10;0;1;4
WireConnection;10;1;6;4
WireConnection;10;2;37;1
WireConnection;10;3;88;4
WireConnection;0;2;18;0
WireConnection;0;9;10;0
ASEEND*/
//CHKSM=A0106393715BA2FF9A66613288C1049A21C29296