// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VFX/Blueberry_Dissolve"
{
	Properties
	{
		_Tx_Voronoi_B("Tx_Voronoi_B", 2D) = "white" {}
		_Blueberrycolor("Blueberrycolor", Color) = (0.2705882,0.254902,0.6235294,1)
		_Tile("Tile", Float) = 1
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
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float4 uv2_tex4coord2;
			float2 uv_texcoord;
		};

		uniform float4 _Blueberrycolor;
		uniform sampler2D _Tx_Voronoi_B;
		uniform float _Tile;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = ( i.vertexColor * _Blueberrycolor ).rgb;
			float2 temp_cast_1 = (_Tile).xx;
			float2 temp_cast_2 = (i.uv2_tex4coord2.y).xx;
			float2 uv_TexCoord4 = i.uv_texcoord * temp_cast_1 + temp_cast_2;
			float2 panner14 = ( 1.0 * _Time.y * float2( 0.2,0.1 ) + uv_TexCoord4);
			float4 tex2DNode1 = tex2D( _Tx_Voronoi_B, panner14 );
			float ifLocalVar15 = 0;
			if( i.uv2_tex4coord2.x <= tex2DNode1.r )
				ifLocalVar15 = 1.0;
			else
				ifLocalVar15 = 0.0;
			float clampResult9 = clamp( ( _Blueberrycolor.a * ifLocalVar15 ) , 0.0 , 1.0 );
			o.Alpha = ( i.vertexColor.a * clampResult9 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
123;411;1307;578;1663.545;-90.78622;1.645209;True;True
Node;AmplifyShaderEditor.RangedFloatNode;5;-1031.234,128.2455;Inherit;False;Property;_Tile;Tile;3;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;13;-1235.707,642.5149;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-807.5471,150.1251;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;14;-567.2794,259.9799;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;17;8.660902,641.421;Inherit;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;13.41364,563.7921;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-357.1535,242.4228;Inherit;True;Property;_Tx_Voronoi_B;Tx_Voronoi_B;1;0;Create;True;0;0;0;False;0;False;-1;9e864b54f6d509848a0938ff4942f504;9e864b54f6d509848a0938ff4942f504;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-305.7831,-285.407;Inherit;False;Property;_Blueberrycolor;Blueberrycolor;2;0;Create;True;0;0;0;False;0;False;0.2705882,0.254902,0.6235294,1;0.270588,0.2549018,0.6235294,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;15;204.2531,511.8771;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;148.8442,125.556;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;9;320.9344,139.6828;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;10;168.1079,-344.4821;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-377.1486,444.0517;Inherit;False;Property;_Subtract;Subtract;4;0;Create;True;0;0;0;False;0;False;0;0.3971954;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;7;-36.08898,332.3218;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;-827.1923,302.7832;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;624.0188,98.58665;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;696.8578,-175.6075;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;926.8637,-162.9498;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;VFX/Blueberry_Dissolve;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;5;0
WireConnection;4;1;13;2
WireConnection;14;0;4;0
WireConnection;1;1;14;0
WireConnection;15;0;13;1
WireConnection;15;1;1;1
WireConnection;15;2;16;0
WireConnection;15;3;17;0
WireConnection;15;4;17;0
WireConnection;6;0;2;4
WireConnection;6;1;15;0
WireConnection;9;0;6;0
WireConnection;7;0;1;1
WireConnection;7;1;13;1
WireConnection;12;0;10;4
WireConnection;12;1;9;0
WireConnection;11;0;10;0
WireConnection;11;1;2;0
WireConnection;0;2;11;0
WireConnection;0;9;12;0
ASEEND*/
//CHKSM=C53CE3441EB6116C805F881021D633C7B1F29500