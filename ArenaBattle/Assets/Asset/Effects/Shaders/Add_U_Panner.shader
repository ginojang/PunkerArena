// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VFX/Add_U_Panner"
{
	Properties
	{
		_Tx_Aura_04_Alpha("Tx_Aura_04_Alpha", 2D) = "white" {}
		_Color0("Color 0", Color) = (1,1,1,0)
		_color_pow("color_pow", Float) = 0
		_Tx_Mask_02("Tx_Mask_02", 2D) = "white" {}
		_Panner_Speed("Panner_Speed", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform sampler2D _Tx_Aura_04_Alpha;
		uniform float _Panner_Speed;
		uniform float4 _Color0;
		uniform float _color_pow;
		uniform sampler2D _Tx_Mask_02;
		uniform float4 _Tx_Mask_02_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float mulTime10 = _Time.y * _Panner_Speed;
			float2 panner3 = ( mulTime10 * float2( 2,-2 ) + i.uv_texcoord);
			float4 tex2DNode1 = tex2D( _Tx_Aura_04_Alpha, panner3 );
			o.Emission = ( i.vertexColor * tex2DNode1 * _Color0 * _color_pow ).rgb;
			float2 uv_Tx_Mask_02 = i.uv_texcoord * _Tx_Mask_02_ST.xy + _Tx_Mask_02_ST.zw;
			o.Alpha = ( i.vertexColor.a * tex2DNode1.a * tex2D( _Tx_Mask_02, uv_Tx_Mask_02 ) ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
219;160;1498;711;2260.538;595.1706;1.869677;True;True
Node;AmplifyShaderEditor.RangedFloatNode;14;-1106.947,64.82539;Inherit;False;Property;_Panner_Speed;Panner_Speed;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-858,-95.5;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;10;-900.5751,76.01189;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;3;-627,-24.5;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;2,-2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-429,-94.5;Inherit;True;Property;_Tx_Aura_04_Alpha;Tx_Aura_04_Alpha;1;0;Create;True;0;0;0;False;0;False;-1;None;b9c007233f6721f40be58420619cdd33;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;4;-328,-326.5;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-133.657,-362.5986;Inherit;False;Property;_color_pow;color_pow;3;0;Create;True;0;0;0;False;0;False;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;7;-155.5531,-534.7748;Inherit;False;Property;_Color0;Color 0;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.990566,0.7341509,0.2943662,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-410.7205,120.8839;Inherit;True;Property;_Tx_Mask_02;Tx_Mask_02;4;0;Create;True;0;0;0;False;0;False;-1;bd7a83cd666b8f54bb6fd9133144e295;bd7a83cd666b8f54bb6fd9133144e295;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;100,-250.5;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-4.092893,65.0639;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;419,-227;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;VFX/Add_U_Panner;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;4;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;14;0
WireConnection;3;0;2;0
WireConnection;3;1;10;0
WireConnection;1;1;3;0
WireConnection;6;0;4;0
WireConnection;6;1;1;0
WireConnection;6;2;7;0
WireConnection;6;3;8;0
WireConnection;5;0;4;4
WireConnection;5;1;1;4
WireConnection;5;2;9;0
WireConnection;0;2;6;0
WireConnection;0;9;5;0
ASEEND*/
//CHKSM=74EB94143EE63C809DC11D8FBC7CC9CC4AB53981