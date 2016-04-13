Shader "Hidden/Fade/Next_Generation/Diffuse" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,0.5)
		_MainTex ("BaseMap (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent"}

		Pass {
			ZWrite On
			ColorMask 0
		}
		
		CGPROGRAM
		#pragma surface surf Lambert alpha:blend exclude_path:prepass

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

	Fallback "Legacy Shaders/Transparent/VertexLit"
}
