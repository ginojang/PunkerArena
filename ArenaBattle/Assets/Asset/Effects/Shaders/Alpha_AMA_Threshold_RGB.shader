// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VFX/Alpha_AMA_Threshold_RGB"
{
	Properties
	{
		_Tex_Main_RGB("Tex_Main_RGB", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (1,1,1,0)
		_Tile_V("Tile_V", Range( 0 , 2)) = 1
		_Tx_Mask("Tx_Mask", 2D) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv2_tex4coord2;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float4 _Color0;
		uniform sampler2D _Tex_Main_RGB;
		uniform float _Tile_V;
		uniform sampler2D _Tx_Mask;
		uniform float4 _Tx_Mask_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 appendResult6 = (float2(i.uv2_tex4coord2.z , _Tile_V));
			float2 appendResult5 = (float2(i.uv2_tex4coord2.x , i.uv2_tex4coord2.y));
			float2 uv_TexCoord7 = i.uv_texcoord * appendResult6 + appendResult5;
			float4 tex2DNode8 = tex2D( _Tex_Main_RGB, uv_TexCoord7 );
			o.Emission = ( _Color0 * tex2DNode8.r * i.vertexColor ).rgb;
			float2 uv_Tx_Mask = i.uv_texcoord * _Tx_Mask_ST.xy + _Tx_Mask_ST.zw;
			float clampResult17 = clamp( ( tex2DNode8.g * tex2D( _Tx_Mask, uv_Tx_Mask ).r * ( tex2DNode8.b - i.uv2_tex4coord2.w ) ) , 0.0 , 1.0 );
			o.Alpha = ( i.vertexColor.a * clampResult17 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
79;100;1664;838;1366.338;651.0594;1.665001;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1395.529,-91.63537;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-1454.099,-176.3054;Inherit;False;Property;_Tile_V;Tile_V;5;0;Create;True;0;0;0;False;0;False;1;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;5;-1029.549,58.7782;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;-1144.604,-217.9051;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-850.2313,1.609264;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;-586.7537,-48.079;Inherit;True;Property;_Tex_Main_RGB;Tex_Main_RGB;1;0;Create;True;0;0;0;False;0;False;-1;9e064053442dfc3468c8fc478649cd37;9e064053442dfc3468c8fc478649cd37;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;15;-237.4663,253.8689;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-575.6853,514.713;Inherit;True;Property;_Tx_Mask;Tx_Mask;6;0;Create;True;0;0;0;False;0;False;-1;573a43d5c233860488800c4c60fd1574;573a43d5c233860488800c4c60fd1574;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;67.25601,210.332;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;17;273.6889,212.2439;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;10;-264.7415,-456.6496;Inherit;False;Property;_Color0;Color 0;3;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;2.996078,0.7686275,0.1411765,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;9;-224.6032,-273.9057;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;16;-575.4615,305.4841;Inherit;False;Property;_Subtract;Subtract;7;0;Create;True;0;0;0;False;0;False;-1;-1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-1249.951,289.8782;Inherit;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1447.005,-251.5052;Inherit;False;Property;_Tile_U;Tile_U;4;0;Create;True;0;0;0;False;0;False;1;1.5;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1385.656,86.58319;Inherit;False;Property;_Offset_U;Offset_U;2;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;40.05848,-341.6496;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;495.133,84.87103;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;768.9277,-295.3595;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;VFX/Alpha_AMA_Threshold_RGB;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;2;1
WireConnection;5;1;2;2
WireConnection;6;0;2;3
WireConnection;6;1;4;0
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;8;1;7;0
WireConnection;15;0;8;3
WireConnection;15;1;2;4
WireConnection;14;0;8;2
WireConnection;14;1;11;1
WireConnection;14;2;15;0
WireConnection;17;0;14;0
WireConnection;13;0;10;0
WireConnection;13;1;8;1
WireConnection;13;2;9;0
WireConnection;19;0;9;4
WireConnection;19;1;17;0
WireConnection;0;2;13;0
WireConnection;0;9;19;0
ASEEND*/
//CHKSM=6C815AC25E75B0638ECB68EE61EED21765C68D4C