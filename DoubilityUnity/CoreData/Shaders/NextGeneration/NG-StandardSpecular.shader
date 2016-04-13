Shader "Next_Generation/Standard (Specular setup)" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_SpecColor("Specular", Color) = (0.2,0.2,0.2)
		_MainTex ("BaseMap (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	SubShader {
		Tags {
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		CGPROGRAM
		#pragma surface surf MyStandardSpecular exclude_path:prepass
		#include "UnityPBSLighting.cginc"
		#include "../CGIncludes/MyLightmap.cginc"

		#pragma multi_compile_fog
		#pragma target 3.0
		#pragma exclude_renderers gles

		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _Glossiness;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = tex.rgb;
			o.Alpha = tex.a;
			o.Smoothness = _Glossiness * tex.a;		// alpha通道 和 _Glossiness 一起控制。
			o.Specular = _SpecColor;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}
		
		ENDCG
	}

	Fallback "Legacy Shaders/VertexLit"
}
