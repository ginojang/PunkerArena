Shader "Custom/Diffuse (Dissolve)" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}

		_BurnMap("Burn Map", 2D) = "white"{}
		_BurnAmount("Burn Amount", Range(-0.25, 1.25)) = 0.0
		_BurnLine("Burn Line Size", Range(0.0, 0.2)) = 0.1
		_BurnColor("Burn Color", Color) = (1.0, 0.0, 0.0, 1.0)
	}
	
	SubShader {
     	Tags { "Queue" = "Transparent" }
     	Blend SrcAlpha OneMinusSrcAlpha
		LOD 200
		
		CGPROGRAM
		#include "../CustomCommon.cginc"
		#pragma surface surf Lambert exclude_path:prepass noforwardadd alpha

		fixed4 _Color;
		sampler2D _MainTex;

		sampler2D _BurnMap;
		fixed _BurnAmount;
		fixed _BurnLine;
		fixed4 _BurnColor;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BurnMap;
		};

		void surf( Input IN, inout SurfaceOutput o ) {
			fixed4 tex = tex2D( _MainTex, IN.uv_MainTex ) * _Color;
			tex *= 1.8f;

        	fixed4 burn = tex2D( _BurnMap, IN.uv_BurnMap );
        	fixed4 clear = DISSOLVE_CLEAR( _BurnColor, burn.r, _BurnAmount, _BurnLine );

			o.Albedo = DISSOLVE_COLOR( tex.rgb, clear, burn.r, _BurnAmount );
			o.Alpha = DISSOLVE_ALPHA( burn.r, _BurnAmount, _BurnLine );
		}
		
		ENDCG
	}
}
