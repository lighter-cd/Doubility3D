Shader "Next_Generation/Transparent/Cutout/Standard" {
	Properties {
		[Gamma] _Metallic ("Metallic", Range(0.0, 1.0)) = 0.214	
		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_MainTex ("BaseMap (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader {
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 400

		CGPROGRAM
		#pragma surface surf MyStandard alphatest:_Cutoff exclude_path:prepass
		#include "UnityPBSLighting.cginc"
		#include "../CGIncludes/MyLightmap.cginc"

		#pragma multi_compile_fog
		#pragma target 3.0
		#pragma exclude_renderers gles
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _Metallic;
		half _Glossiness;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb;
			o.Alpha = tex.a;
			o.Smoothness = _Glossiness * tex.a;		// alpha通道 和 _Glossiness 一起控制。
			o.Metallic = _Metallic;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}
		
		ENDCG
	}

	Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
