// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FruitDino FX Shaders/FX Amber"
{
	Properties
	{
		[HDR]_MainColor("MainColor", Color) = (1,1,1,1)
		[HDR]_MainRimlightColor("Main Rimlight Color", Color) = (1,1,1,1)
		_MainRimlightBias("Main Rimlight Bias", Float) = 0
		_MainRimlightScale("Main Rimlight Scale", Float) = 1
		_MainRimlightPower("Main Rimlight Power", Float) = 1
		[HDR]_GemTex01Color("GemTex 01 Color", Color) = (1,1,1,1)
		_GemTex01Mask("GemTex 01 Mask", 2D) = "white" {}
		_GemTex01("GemTex 01", 2D) = "white" {}
		_GemTex01UTile("GemTex 01 U Tile", Float) = 1
		_GemTex01VTile("GemTex 01 V Tile", Float) = 1
		_GemTex01UOffset("GemTex 01 U Offset", Float) = 0
		_GemTex01VOffset("GemTex 01 V Offset", Float) = 0
		_GemTex01USpeed("GemTex 01 U Speed", Float) = 0
		_GemTex01VSpeed("GemTex 01 V Speed", Float) = 0
		[HDR]_GemTex02Color("GemTex 02 Color", Color) = (1,1,1,1)
		_GemTex02("GemTex 02", 2D) = "white" {}
		_GemTex02UTile("GemTex 02 U Tile", Float) = 1
		_GemTex02VTile("GemTex 02 V Tile", Float) = 1
		_GemTex02UOffset("GemTex 02 U Offset", Float) = 0
		_GemTex02VOffset("GemTex 02 V Offset", Float) = 0
		_GemTex02USpeed("GemTex 02 U Speed", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_ScrBlendMod("ScrBlend Mod", Float) = 0
		_GemTex02VSpeed("GemTex 02 V Speed", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_DstBlendMod("DstBlend Mod", Float) = 0
		_GemTex02RimlightBias("GemTex 02 Rimlight Bias", Float) = 0
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("Cull Mode", Float) = 0
		_GemTex02RimlightScale("GemTex 02 Rimlight Scale", Float) = 1
		_GemTex02RimlightPower("GemTex 02 Rimlight Power", Float) = 1
		[HDR]_GemTex03Color("GemTex 03 Color", Color) = (1,1,1,1)
		_GemTex03("GemTex 03", 2D) = "white" {}
		_GemTex03UTile("GemTex 03 U Tile", Float) = 1
		_GemTex03VTile("GemTex 03 V Tile", Float) = 1
		_GemTex03UOffset("GemTex 03 U Offset", Float) = 0
		_GemTex03VOffset("GemTex 03 V Offset", Float) = 0
		_GemTex03USpeed("GemTex 03 U Speed", Float) = 0
		_GemTex03VSpeed("GemTex 03 V Speed", Float) = 0
		_GemTex03RimlightBias("GemTex 03 Rimlight Bias", Float) = 0
		_GemTex03RimlightScale("GemTex 03 Rimlight Scale", Float) = 1
		_GemTex03RimlightPower("GemTex 03 Rimlight Power", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform float _DstBlendMod;
		uniform float _ScrBlendMod;
		uniform float _CullMode;
		uniform float4 _MainRimlightColor;
		uniform float _MainRimlightBias;
		uniform float _MainRimlightScale;
		uniform float _MainRimlightPower;
		uniform float4 _MainColor;
		uniform float4 _GemTex01Color;
		uniform sampler2D _GemTex01;
		uniform float _GemTex01USpeed;
		uniform float _GemTex01VSpeed;
		uniform float _GemTex01UTile;
		uniform float _GemTex01VTile;
		uniform float _GemTex01UOffset;
		uniform float _GemTex01VOffset;
		uniform sampler2D _GemTex01Mask;
		uniform float4 _GemTex01Mask_ST;
		uniform float _GemTex02RimlightBias;
		uniform float _GemTex02RimlightScale;
		uniform float _GemTex02RimlightPower;
		uniform float4 _GemTex02Color;
		uniform sampler2D _GemTex02;
		uniform float _GemTex02USpeed;
		uniform float _GemTex02VSpeed;
		uniform float _GemTex02UTile;
		uniform float _GemTex02VTile;
		uniform float _GemTex02UOffset;
		uniform float _GemTex02VOffset;
		uniform float _GemTex03RimlightBias;
		uniform float _GemTex03RimlightScale;
		uniform float _GemTex03RimlightPower;
		uniform float4 _GemTex03Color;
		uniform sampler2D _GemTex03;
		uniform float _GemTex03USpeed;
		uniform float _GemTex03VSpeed;
		uniform float _GemTex03UTile;
		uniform float _GemTex03VTile;
		uniform float _GemTex03UOffset;
		uniform float _GemTex03VOffset;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV14 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode14 = ( _MainRimlightBias + _MainRimlightScale * pow( 1.0 - fresnelNdotV14, _MainRimlightPower ) );
			float2 appendResult90 = (float2(_GemTex01USpeed , _GemTex01VSpeed));
			float2 appendResult31 = (float2(_GemTex01UTile , _GemTex01VTile));
			float2 appendResult32 = (float2(_GemTex01UOffset , _GemTex01VOffset));
			float2 uv_TexCoord23 = i.uv_texcoord * appendResult31 + appendResult32;
			float2 panner87 = ( 1.0 * _Time.y * appendResult90 + uv_TexCoord23);
			float2 uv_GemTex01Mask = i.uv_texcoord * _GemTex01Mask_ST.xy + _GemTex01Mask_ST.zw;
			float fresnelNdotV119 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode119 = ( _GemTex02RimlightBias + _GemTex02RimlightScale * pow( 1.0 - fresnelNdotV119, _GemTex02RimlightPower ) );
			float2 appendResult115 = (float2(_GemTex02USpeed , _GemTex02VSpeed));
			float2 appendResult111 = (float2(_GemTex02UTile , _GemTex02VTile));
			float2 appendResult113 = (float2(_GemTex02UOffset , _GemTex02VOffset));
			float2 uv_TexCoord116 = i.uv_texcoord * appendResult111 + appendResult113;
			float2 panner121 = ( 1.0 * _Time.y * appendResult115 + uv_TexCoord116);
			float fresnelNdotV155 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode155 = ( _GemTex03RimlightBias + _GemTex03RimlightScale * pow( 1.0 - fresnelNdotV155, _GemTex03RimlightPower ) );
			float2 appendResult153 = (float2(_GemTex03USpeed , _GemTex03VSpeed));
			float2 appendResult148 = (float2(_GemTex03UTile , _GemTex03VTile));
			float2 appendResult149 = (float2(_GemTex03UOffset , _GemTex03VOffset));
			float2 uv_TexCoord151 = i.uv_texcoord * appendResult148 + appendResult149;
			float2 panner156 = ( 1.0 * _Time.y * appendResult153 + uv_TexCoord151);
			o.Emission = ( ( _MainRimlightColor * fresnelNode14 ) + _MainColor + ( _GemTex01Color * tex2D( _GemTex01, panner87 ).r * tex2D( _GemTex01Mask, uv_GemTex01Mask ).r ) + saturate( ( ( 1.0 - fresnelNode119 ) * _GemTex02Color * tex2D( _GemTex02, panner121 ).r ) ) + saturate( ( ( 1.0 - fresnelNode155 ) * _GemTex03Color * tex2D( _GemTex03, panner156 ).r ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-2560;0;2560;1379;4006.596;951.4504;2.483381;True;True
Node;AmplifyShaderEditor.CommentaryNode;140;-2956.487,872.3734;Inherit;False;2392.162;975.0707;Comment;20;138;124;125;122;135;121;119;114;115;118;116;117;113;111;110;112;108;107;106;109;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;141;-2952.308,1973.682;Inherit;False;2392.162;975.0707;Comment;20;161;160;159;158;157;156;155;154;153;152;151;150;149;148;147;146;145;144;143;142;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-2848.849,1403.342;Inherit;False;Property;_GemTex02UOffset;GemTex 02 U Offset;22;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-2851.467,1186.058;Inherit;False;Property;_GemTex02UTile;GemTex 02 U Tile;20;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;103;-2948.912,-680.9376;Inherit;False;2095.032;1254.37;Comment;16;28;30;27;29;89;31;88;32;90;23;87;1;91;2;92;127;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-2841.384,2385.718;Inherit;False;Property;_GemTex03VTile;GemTex 03 V Tile;35;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-2846.294,1497.357;Inherit;False;Property;_GemTex02VOffset;GemTex 02 V Offset;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-2847.288,2287.366;Inherit;False;Property;_GemTex03UTile;GemTex 03 U Tile;34;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;145;-2844.67,2504.65;Inherit;False;Property;_GemTex03UOffset;GemTex 03 U Offset;36;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-2842.115,2598.666;Inherit;False;Property;_GemTex03VOffset;GemTex 03 V Offset;37;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;109;-2845.563,1284.41;Inherit;False;Property;_GemTex02VTile;GemTex 02 V Tile;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-2893.008,-5.322861;Inherit;False;Property;_GemTex01VTile;GemTex 01 V Tile;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;149;-2545.546,2532.554;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;148;-2540.677,2341.369;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-2537.928,2734.437;Inherit;False;Property;_GemTex03USpeed;GemTex 03 U Speed;38;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;147;-2534.979,2848.472;Inherit;False;Property;_GemTex03VSpeed;GemTex 03 V Speed;39;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-2542.107,1633.128;Inherit;False;Property;_GemTex02USpeed;GemTex 02 U Speed;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-2893.739,207.6249;Inherit;False;Property;_GemTex01VOffset;GemTex 01 V Offset;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;113;-2549.725,1431.246;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-2898.912,-103.6744;Inherit;False;Property;_GemTex01UTile;GemTex 01 U Tile;8;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-2896.294,113.6094;Inherit;False;Property;_GemTex01UOffset;GemTex 01 U Offset;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;111;-2544.856,1240.061;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-2539.158,1747.164;Inherit;False;Property;_GemTex02VSpeed;GemTex 02 V Speed;26;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-2251.209,1047.267;Inherit;False;Property;_GemTex02RimlightScale;GemTex 02 Rimlight Scale;30;0;Create;True;0;0;0;False;0;False;1;1.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;151;-2304.107,2396.062;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;154;-2247.03,2148.575;Inherit;False;Property;_GemTex03RimlightScale;GemTex 03 Rimlight Scale;41;0;Create;True;0;0;0;False;0;False;1;1.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-2246.69,2242.584;Inherit;False;Property;_GemTex03RimlightPower;GemTex 03 Rimlight Power;42;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-2239.241,2039.402;Inherit;False;Property;_GemTex03RimlightBias;GemTex 03 Rimlight Bias;40;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;31;-2592.301,-49.67135;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;153;-2286.124,2752.736;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-2597.17,141.5137;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-2589.552,343.396;Inherit;False;Property;_GemTex01USpeed;GemTex 01 U Speed;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;116;-2308.286,1294.753;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;118;-2250.869,1141.276;Inherit;False;Property;_GemTex02RimlightPower;GemTex 02 Rimlight Power;31;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-2243.42,938.0933;Inherit;False;Property;_GemTex02RimlightBias;GemTex 02 Rimlight Bias;28;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;115;-2290.303,1651.428;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-2586.603,457.432;Inherit;False;Property;_GemTex01VSpeed;GemTex 01 V Speed;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;90;-2337.748,361.6959;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FresnelNode;119;-1906.147,951.2589;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-2355.731,5.020859;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;11;-1725.494,-1444.157;Inherit;False;895.5863;564.9632;Rimlight;6;16;15;14;13;12;66;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;121;-1989.581,1461.287;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FresnelNode;155;-1901.968,2052.567;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;156;-1985.402,2562.595;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;122;-1502.063,1612.557;Inherit;True;Property;_GemTex02;GemTex 02;19;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;158;-1497.884,2713.865;Inherit;True;Property;_GemTex03;GemTex 03;33;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;124;-1305.094,1230.982;Inherit;False;Property;_GemTex02Color;GemTex 02 Color;18;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;66;-1690.336,-1197.231;Inherit;False;Property;_MainRimlightBias;Main Rimlight Bias;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;160;-1300.915,2332.291;Inherit;False;Property;_GemTex03Color;GemTex 03 Color;32;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;12;-1698.124,-1083.661;Inherit;False;Property;_MainRimlightScale;Main Rimlight Scale;3;0;Create;True;0;0;0;False;0;False;1;1.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1697.784,-998.687;Inherit;False;Property;_MainRimlightPower;Main Rimlight Power;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;157;-1224.401,2044.217;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;135;-1228.58,942.9084;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;87;-2037.026,171.5546;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-1581.75,256.0369;Inherit;True;Property;_GemTex01;GemTex 01;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;91;-1502.142,23.73734;Inherit;False;Property;_GemTex01Color;GemTex 01 Color;5;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;125;-939.9454,1210.586;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;159;-935.7661,2311.895;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;14;-1448.638,-1155.535;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;127;-1594.724,472.6189;Inherit;True;Property;_GemTex01Mask;GemTex 01 Mask;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;-1406.636,-1383.752;Inherit;False;Property;_MainRimlightColor;Main Rimlight Color;1;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,0.8916426,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1054.68,-1190.66;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-1130.367,-53.71952;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;161;-765.5645,2180.129;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;92;-2059,-591.6597;Inherit;False;895.5863;564.9632;Rimlight;6;98;97;96;95;94;93;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;138;-762.3248,1175.268;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;99;-495.0844,-297.8823;Inherit;False;Property;_MainColor;MainColor;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;162;357.3625,514.9345;Inherit;False;250.767;346.4162;BlendMod;3;165;164;163;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;59.43949,-149.4107;Inherit;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;165;408.8961,745.3503;Inherit;False;Property;_CullMode;Cull Mode;29;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-2023.841,-344.7323;Inherit;False;Property;_GemTex01RimlightBias;GemTex 01 Rimlight Bias;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-2031.29,-146.1887;Inherit;False;Property;_GemTex01RimlightPower;GemTex 01 Rimlight Power;17;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;408.1298,564.9345;Inherit;False;Property;_ScrBlendMod;ScrBlend Mod;25;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;96;-1782.141,-303.0374;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;97;-1740.14,-531.254;Inherit;False;Property;_GemTex01RimlightColor;GemTex 01 Rimlight Color;14;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,0.8916426,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;93;-2031.63,-235.5583;Inherit;False;Property;_GemTex01RimlightScale;GemTex 01 Rimlight Scale;16;0;Create;True;0;0;0;False;0;False;1;1.34;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-1458.288,-394.2465;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;163;407.3628,658.4999;Inherit;False;Property;_DstBlendMod;DstBlend Mod;27;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;384.0772,-46.65108;Float;False;True;-1;0;ASEMaterialInspector;0;0;Unlit;FruitDino FX Shaders/FX Amber;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;6;d3d11;glcore;gles;gles3;metal;vulkan;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;True;164;10;True;163;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;43;-1;-1;-1;0;False;0;0;True;165;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;149;0;145;0
WireConnection;149;1;143;0
WireConnection;148;0;144;0
WireConnection;148;1;142;0
WireConnection;113;0;108;0
WireConnection;113;1;106;0
WireConnection;111;0;107;0
WireConnection;111;1;109;0
WireConnection;151;0;148;0
WireConnection;151;1;149;0
WireConnection;31;0;27;0
WireConnection;31;1;28;0
WireConnection;153;0;146;0
WireConnection;153;1;147;0
WireConnection;32;0;29;0
WireConnection;32;1;30;0
WireConnection;116;0;111;0
WireConnection;116;1;113;0
WireConnection;115;0;112;0
WireConnection;115;1;110;0
WireConnection;90;0;88;0
WireConnection;90;1;89;0
WireConnection;119;1;117;0
WireConnection;119;2;114;0
WireConnection;119;3;118;0
WireConnection;23;0;31;0
WireConnection;23;1;32;0
WireConnection;121;0;116;0
WireConnection;121;2;115;0
WireConnection;155;1;150;0
WireConnection;155;2;154;0
WireConnection;155;3;152;0
WireConnection;156;0;151;0
WireConnection;156;2;153;0
WireConnection;122;1;121;0
WireConnection;158;1;156;0
WireConnection;157;0;155;0
WireConnection;135;0;119;0
WireConnection;87;0;23;0
WireConnection;87;2;90;0
WireConnection;1;1;87;0
WireConnection;125;0;135;0
WireConnection;125;1;124;0
WireConnection;125;2;122;1
WireConnection;159;0;157;0
WireConnection;159;1;160;0
WireConnection;159;2;158;1
WireConnection;14;1;66;0
WireConnection;14;2;12;0
WireConnection;14;3;13;0
WireConnection;16;0;15;0
WireConnection;16;1;14;0
WireConnection;2;0;91;0
WireConnection;2;1;1;1
WireConnection;2;2;127;1
WireConnection;161;0;159;0
WireConnection;138;0;125;0
WireConnection;19;0;16;0
WireConnection;19;1;99;0
WireConnection;19;2;2;0
WireConnection;19;3;138;0
WireConnection;19;4;161;0
WireConnection;96;1;94;0
WireConnection;96;2;93;0
WireConnection;96;3;95;0
WireConnection;98;0;97;0
WireConnection;98;1;96;0
WireConnection;0;2;19;0
ASEEND*/
//CHKSM=8D7150946BD568BF209FD9D0C4E5304D48AD6F0C