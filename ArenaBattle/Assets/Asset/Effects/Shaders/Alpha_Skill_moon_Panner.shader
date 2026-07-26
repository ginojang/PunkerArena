// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VFX/Alpha_Skill_moon_Panner"
{
	Properties
	{
		_Tx_Mask_02_RGB("Tx_Mask_02_RGB", 2D) = "white" {}
		_Main_color("Main_color", Color) = (0,0,0,0)
		_Main_pow1("Main_pow1", Range( 0 , 10)) = 1
		_Back_pow3("Back_pow3", Range( 0 , 10)) = 0.5
		_Tip_Color("Tip_Color", Color) = (0,0,0,0)
		_Tip_pow2("Tip_pow2", Range( 0 , 10)) = 1
		_Back_color("Back_color", Color) = (0,0,0,0)
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
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform sampler2D _Tx_Mask_02_RGB;
		uniform float4 _Tx_Mask_02_RGB_ST;
		uniform float4 _Main_color;
		uniform float _Main_pow1;
		uniform float4 _Tip_Color;
		uniform float _Tip_pow2;
		uniform float4 _Back_color;
		uniform float _Back_pow3;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Tx_Mask_02_RGB = i.uv_texcoord * _Tx_Mask_02_RGB_ST.xy + _Tx_Mask_02_RGB_ST.zw;
			float4 tex2DNode1 = tex2D( _Tx_Mask_02_RGB, uv_Tx_Mask_02_RGB );
			float4 temp_output_53_0 = ( ( tex2DNode1.r * _Main_color * _Main_pow1 ) + ( tex2DNode1.g * _Tip_Color * _Tip_pow2 ) + ( tex2DNode1.b * _Back_color * _Back_pow3 ) );
			o.Emission = ( i.vertexColor * temp_output_53_0 ).rgb;
			o.Alpha = ( i.vertexColor.a * temp_output_53_0 ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
428;67;1358;864;-961.7248;1434.142;2.634084;True;True
Node;AmplifyShaderEditor.ColorNode;55;593.8323,359.2676;Inherit;False;Property;_Tip_Color;Tip_Color;14;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.8735831,0.8867924,0.6734603,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;250.0017,290.0897;Inherit;False;Property;_Main_pow1;Main_pow1;12;0;Create;True;0;0;0;False;0;False;1;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;51;278.4639,114.6095;Inherit;False;Property;_Main_color;Main_color;11;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0.4965908,0.1084905,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;57;838.9824,769.0389;Inherit;False;Property;_Back_pow3;Back_pow3;13;0;Create;True;0;0;0;False;0;False;0.5;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;560.5815,540.3299;Inherit;False;Property;_Tip_pow2;Tip_pow2;15;0;Create;True;0;0;0;False;0;False;1;2.5;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;58;872.2332,590.972;Inherit;False;Property;_Back_color;Back_color;16;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0.4467708,0.1745283,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-46.7955,-146.1553;Inherit;True;Property;_Tx_Mask_02_RGB;Tx_Mask_02_RGB;1;0;Create;True;0;0;0;False;0;False;-1;8259fefae2a9e53429d0f61fbf6b97dd;8259fefae2a9e53429d0f61fbf6b97dd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;1178.109,572.2347;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;725.2404,85.92643;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;899.7084,340.5304;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;1714.07,67.61022;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;60;1899.48,-248.5147;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;637.3135,-1438.227;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;43;900.1265,-1341.542;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.5,0.1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;747.3511,-729.0721;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;46;2317.36,-1000.951;Inherit;False;Property;_Color0;Color 0;9;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.9716981,0.803349,0.1970897,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;27;987.3505,-729.0723;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ComponentMaskNode;32;2628.983,-380.3214;Inherit;True;True;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;11;272.5812,-738.5681;Inherit;True;Property;_Flow_Tex;Flow_Tex;5;0;Create;True;0;0;0;False;0;False;-1;None;8d1b52a74b46c0945afa904b1579b307;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;41;1887.314,-1065.767;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-455.4674,-1011.462;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;3;-712.403,-773.3693;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.02,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;5;-464.3814,-635.5516;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;0;False;0;False;-1;None;542777ff64db63b41a6d8195293ae90f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-468.3814,-819.5516;Inherit;True;Property;_FlowTexture_RGB;FlowTexture_RGB;2;0;Create;True;0;0;0;False;0;False;-1;None;542777ff64db63b41a6d8195293ae90f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;35;1687.641,-393.0909;Inherit;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;0;False;0;False;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;1091.422,-1129.677;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;1974.251,-781.4261;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;24;-43.44268,-341.0425;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-90.38142,-694.5516;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1566.162,-1174.502;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;145.7507,-426.6724;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;8;73.20617,-768.7474;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DeltaTime;38;-850.3404,-34.89325;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;28;1182.987,-837.8713;Inherit;True;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;851.2605,-127.3133;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;30;1703.804,-777.2224;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;48;2765.638,-778.5493;Inherit;False;Property;_color_pow;color_pow;10;0;Create;True;0;0;0;False;0;False;1;1.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;39;-1059.139,-259.8917;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;1445.517,-844.9074;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;15;-154.4293,-968.9395;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-1256.313,-388.1163;Inherit;False;Property;_Speed;Speed;7;0;Create;True;0;0;0;False;0;False;0;6.8;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;2902.904,-948.8251;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-967.1355,-599.6879;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.2,0.5;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;22;286.5506,-468.2724;Inherit;True;Property;_TextureSample1;Texture Sample 1;6;0;Create;True;0;0;0;False;0;False;-1;None;8d1b52a74b46c0945afa904b1579b307;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;1823.84,-450.6906;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;14;570.9268,-143.0705;Inherit;True;True;False;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-344.2341,-419.6087;Inherit;False;Property;_Dist_pow;Dist_pow;4;0;Create;True;0;0;0;False;0;False;0.2;-0.6;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;40;1213.958,-1138.08;Inherit;True;Property;_Tx_Aura_04_Alpha;Tx_Aura_04_Alpha;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-327.4558,-310.7028;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0.5;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;49;2605.787,-844.5751;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;18;-966.4153,-776.2179;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,2;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;33;2320.747,-789.1847;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;2157.396,-223.9881;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-850.3405,-331.8912;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;6;-722.4125,-596.4366;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.01,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;2153.298,164.816;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3570.679,-679.8785;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;VFX/Alpha_Skill_moon_Panner;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;59;0;1;3
WireConnection;59;1;58;0
WireConnection;59;2;57;0
WireConnection;50;0;1;1
WireConnection;50;1;51;0
WireConnection;50;2;52;0
WireConnection;54;0;1;2
WireConnection;54;1;55;0
WireConnection;54;2;56;0
WireConnection;53;0;50;0
WireConnection;53;1;54;0
WireConnection;53;2;59;0
WireConnection;43;0;42;0
WireConnection;26;0;11;0
WireConnection;26;1;22;0
WireConnection;27;0;26;0
WireConnection;32;0;33;0
WireConnection;11;1;8;0
WireConnection;41;0;45;0
WireConnection;41;1;29;0
WireConnection;3;0;18;0
WireConnection;3;1;36;0
WireConnection;5;1;6;0
WireConnection;4;1;3;0
WireConnection;44;0;43;0
WireConnection;44;1;7;0
WireConnection;31;0;30;0
WireConnection;31;1;1;2
WireConnection;24;0;23;0
WireConnection;24;1;36;0
WireConnection;7;0;4;2
WireConnection;7;1;5;2
WireConnection;7;2;10;0
WireConnection;45;0;40;0
WireConnection;45;1;1;3
WireConnection;25;0;7;0
WireConnection;25;1;24;0
WireConnection;8;0;15;0
WireConnection;8;1;7;0
WireConnection;28;0;27;0
WireConnection;28;1;27;1
WireConnection;28;2;27;2
WireConnection;12;0;11;0
WireConnection;12;1;14;0
WireConnection;30;0;41;0
WireConnection;30;1;1;1
WireConnection;29;0;28;0
WireConnection;29;1;1;3
WireConnection;15;0;9;0
WireConnection;15;1;36;0
WireConnection;47;0;46;0
WireConnection;47;1;49;0
WireConnection;47;2;48;0
WireConnection;22;1;25;0
WireConnection;34;0;1;3
WireConnection;34;1;35;0
WireConnection;14;0;1;0
WireConnection;40;1;44;0
WireConnection;49;0;33;0
WireConnection;33;0;31;0
WireConnection;33;1;34;0
WireConnection;61;0;60;0
WireConnection;61;1;53;0
WireConnection;36;0;37;0
WireConnection;36;1;39;0
WireConnection;6;0;19;0
WireConnection;6;1;36;0
WireConnection;62;0;60;4
WireConnection;62;1;53;0
WireConnection;0;2;61;0
WireConnection;0;9;62;0
ASEEND*/
//CHKSM=B4652DC0C81D0CE7250E497C628F011AF0D715D6