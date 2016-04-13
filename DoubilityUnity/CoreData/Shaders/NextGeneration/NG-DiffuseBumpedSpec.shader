Shader "Next_Generation/DiffuseBumpedSpecular" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)	// 在 BlinnPhong 中被处理
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_MainTex ("BaseMap (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}
	SubShader {
		Tags {
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		CGPROGRAM
		#pragma surface surf MyBlinnPhong exclude_path:prepass
		#include "UnityPBSLighting.cginc"
		#include "../CGIncludes/MyLightmap.cginc"

		#pragma multi_compile_fog
		#pragma target 3.0
		#pragma exclude_renderers gles

		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _Shininess;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = tex.rgb;
			o.Alpha = tex.a;
			o.Gloss = tex.a;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}
		
		ENDCG
	}

	Fallback "Legacy Shaders/VertexLit"
}
