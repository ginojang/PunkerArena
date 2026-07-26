// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FruitDino FX Shaders/FX Wave 002"
{
	Properties
	{
		_TintColor("TintColor", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_MaskTex("MaskTex", 2D) = "white" {}
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
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float2 uv2_texcoord2;
		};

		uniform float _DstBlendMod;
		uniform float _ScrBlendMod;
		uniform float _CullMode;
		uniform float4 _TintColor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
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
		uniform sampler2D _MaskTex;
		uniform float4 _MaskTex_ST;


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


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


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1));
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1));
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			Gradient gradient90 = NewGradient( 0, 2, 2, float4( 0, 0, 0, 0.123537 ), float4( 1, 1, 1, 0.1764706 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
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
			o.Emission = ( _TintColor * ( SampleGradient( gradient90, saturate( ( tex2D( _MainTex, uv_MainTex ).r + ( _NoisePower * pow( voroi67 , _Noise1Power ) * pow( simplePerlin2D74 , _Noise2Power ) ) ) ) ) * i.vertexColor ) ).rgb;
			float2 uv2_MaskTex = i.uv2_texcoord2 * _MaskTex_ST.xy + _MaskTex_ST.zw;
			o.Alpha = ( i.vertexColor.a * SampleGradient( gradient90, saturate( ( tex2D( _MainTex, uv_MainTex ).r + ( _NoisePower * pow( voroi67 , _Noise1Power ) * pow( simplePerlin2D74 , _Noise2Power ) ) ) ) ).r * tex2D( _MaskTex, uv2_MaskTex ).r * _TintColor.a );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1898;1418;1655;1171;1756.165;1103.067;1.895019;True;True
Node;AmplifyShaderEditor.CommentaryNode;50;-3283.356,568.5861;Inherit;False;1514.208;1113.558;Noise;23;86;76;77;74;75;67;73;85;84;83;82;81;80;79;78;59;49;58;48;46;47;44;45;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-3189.507,1254.03;Inherit;False;Property;_Noise2VTile;Noise 2 V Tile;11;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;79;-3185.606,1160.434;Inherit;False;Property;_Noise2UTile;Noise 2 U Tile;10;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-3195.132,734.1732;Inherit;False;Property;_Noise1VTile;Noise 1 V Tile;6;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-3205.717,640.0627;Inherit;False;Property;_Noise1UTile;Noise 1 U Tile;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;82;-2982.614,1239.021;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;48;-2988.239,719.1636;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-3204.755,853.0953;Inherit;False;Property;_Noise1USpeed;Noise 1 U Speed;7;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-3199.13,1372.952;Inherit;False;Property;_Noise2USpeed;Noise 2 U Speed;12;0;Create;True;0;0;0;False;0;False;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-3196.224,1475.419;Inherit;False;Property;_Noise2VSpeed;Noise 2 V Speed;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-3201.849,955.5623;Inherit;False;Property;_Noise1VSpeed;Noise 1 V Speed;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;58;-2841.003,700.5078;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;84;-2982.269,1406.102;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;83;-2835.378,1220.365;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;49;-2987.894,886.2452;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;59;-2607.157,808.6918;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;73;-2618.073,977.9132;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;85;-2601.532,1328.549;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-2283.029,1566.779;Inherit;False;Property;_Noise2Power;Noise 2 Power;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;74;-2357.123,1322.522;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;67;-2301.926,802.3329;Inherit;True;0;0;1;0;1;False;1;False;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;3;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.RangedFloatNode;76;-2304.839,1095.476;Inherit;False;Property;_Noise1Power;Noise 1 Power;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;77;-2059.277,804.7832;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-1863.689,410.2585;Inherit;False;Property;_NoisePower;Noise Power;3;0;Create;True;0;0;0;False;0;False;1;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;75;-2057.32,1319.132;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1430.767,-228.5175;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1645.115,486.6332;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-1030.62,-199.5374;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;90;-1051.711,-417.7399;Inherit;False;0;2;2;0,0,0,0.123537;1,1,1,0.1764706;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SaturateNode;93;-842.5386,-188.064;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;6;-194.7799,27.23987;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientSampleNode;89;-655.2457,-204.3743;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;55.20913,-190.3075;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;94;-300.7909,-458.7601;Inherit;False;Property;_TintColor;TintColor;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;37;-970.8055,361.6745;Inherit;True;Property;_MaskTex;MaskTex;2;0;Create;True;0;0;0;False;0;False;-1;None;8c35f07d8aa38d4469127b73710415c2;True;1;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;20;386.8508,419.0916;Inherit;False;250.767;346.4162;BlendMod;3;4;5;3;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;60.03728,340.4889;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;152.1186,-354.5341;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3;437.6178,469.0916;Inherit;False;Property;_ScrBlendMod;ScrBlend Mod;14;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;436.8508,562.6569;Inherit;False;Property;_DstBlendMod;DstBlend Mod;15;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;438.3841,649.5073;Inherit;False;Property;_CullMode;Cull Mode;16;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;391.0373,-149.8189;Float;False;True;-1;0;ASEMaterialInspector;0;0;Unlit;FruitDino FX Shaders/FX Wave 002;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;6;d3d11;glcore;gles;gles3;metal;vulkan;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;1;5;True;3;10;True;4;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;17;-1;-1;-1;0;False;0;0;True;5;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;82;0;79;0
WireConnection;82;1;78;0
WireConnection;48;0;44;0
WireConnection;48;1;45;0
WireConnection;58;0;48;0
WireConnection;84;0;81;0
WireConnection;84;1;80;0
WireConnection;83;0;82;0
WireConnection;49;0;46;0
WireConnection;49;1;47;0
WireConnection;59;0;58;0
WireConnection;59;2;49;0
WireConnection;85;0;83;0
WireConnection;85;2;84;0
WireConnection;74;0;85;0
WireConnection;67;0;59;0
WireConnection;67;1;73;2
WireConnection;77;0;67;0
WireConnection;77;1;76;0
WireConnection;75;0;74;0
WireConnection;75;1;86;0
WireConnection;60;0;42;0
WireConnection;60;1;77;0
WireConnection;60;2;75;0
WireConnection;92;0;1;1
WireConnection;92;1;60;0
WireConnection;93;0;92;0
WireConnection;89;0;90;0
WireConnection;89;1;93;0
WireConnection;2;0;89;0
WireConnection;2;1;6;0
WireConnection;10;0;6;4
WireConnection;10;1;89;1
WireConnection;10;2;37;1
WireConnection;10;3;94;4
WireConnection;95;0;94;0
WireConnection;95;1;2;0
WireConnection;0;2;95;0
WireConnection;0;9;10;0
ASEEND*/
//CHKSM=E00D069495ADBCC4969040E59ADACD5FE48076E8