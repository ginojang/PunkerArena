// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VFX/Alpha_Threshold"
{
	Properties
	{
		_Test_Smoke_A("Test_Smoke_A", 2D) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 uv2_tex4coord2;
		};

		uniform sampler2D _Test_Smoke_A;
		uniform float4 _Test_Smoke_A_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 color8 = IsGammaSpace() ? float4(0.8301887,0.8000439,0.747953,0) : float4(0.6562665,0.6039018,0.5193384,0);
			float2 uv_Test_Smoke_A = i.uv_texcoord * _Test_Smoke_A_ST.xy + _Test_Smoke_A_ST.zw;
			float4 tex2DNode1 = tex2D( _Test_Smoke_A, uv_Test_Smoke_A );
			o.Emission = ( i.vertexColor * ( color8 * tex2DNode1.r ) ).rgb;
			float ifLocalVar2 = 0;
			if( i.uv2_tex4coord2.x >= tex2DNode1.g )
				ifLocalVar2 = 0.0;
			else
				ifLocalVar2 = 1.0;
			o.Alpha = ( i.vertexColor.a * ( tex2DNode1.b * ifLocalVar2 ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
433;81;879;617;891.1191;546.1857;1.444664;True;True
Node;AmplifyShaderEditor.RangedFloatNode;4;-51.00279,162.6412;Inherit;False;Constant;_Float1;Float 1;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-44.56777,249.4313;Inherit;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-37.46337,-235.8884;Inherit;True;Property;_Test_Smoke_A;Test_Smoke_A;1;0;Create;True;0;0;0;False;0;False;-1;48f227fc35f5ca94693d5b1e859c3da0;48f227fc35f5ca94693d5b1e859c3da0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;9;-233.7973,-133.0119;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;264.6119,-417.6107;Inherit;False;Constant;_Color0;Color 0;2;0;Create;True;0;0;0;False;0;False;0.8301887,0.8000439,0.747953,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;2;313.7968,105.4915;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;585.3271,-300.593;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;10;563.6575,-537.5179;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;553.6143,-17.95517;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-120.0797,73.69757;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0;0;0;0.35;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;858.3689,-326.5968;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;862.7031,-62.22339;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1114.838,-264.4912;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;VFX/Alpha_Threshold;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;9;1
WireConnection;2;1;1;2
WireConnection;2;2;4;0
WireConnection;2;3;4;0
WireConnection;2;4;5;0
WireConnection;7;0;8;0
WireConnection;7;1;1;1
WireConnection;6;0;1;3
WireConnection;6;1;2;0
WireConnection;11;0;10;0
WireConnection;11;1;7;0
WireConnection;12;0;10;4
WireConnection;12;1;6;0
WireConnection;0;2;11;0
WireConnection;0;9;12;0
ASEEND*/
//CHKSM=8D93E8E25E29CD6CFE41692049CF9D781AE29750