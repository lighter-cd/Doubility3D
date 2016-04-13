Shader "Hidden/Next_Generation/TerrainEngine/Splatmap/Diffuse-Base" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

	CGPROGRAM
	#pragma surface surf MyLambert
	#include "UnityPBSLighting.cginc"
	#include "../CGIncludes/MyLightmap.cginc"		

	sampler2D _MainTex;
	fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}


	FallBack "Diffuse"
}
