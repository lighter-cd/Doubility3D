Shader "Hidden/Fade/Next_Generation/Transparent/Cutout/DiffuseBumped" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="Transparent+1" "IgnoreProjector"="True" "RenderType"="Transparent"}

    Pass {
        ZWrite On
        ColorMask 0
        AlphaTest Greater [_Cutoff]
        SetTexture [_MainTex] {
			combine texture * primary, texture
        }
    }
	LOD 300
	
	CGPROGRAM
	#pragma surface surf Lambert alpha:blend exclude_path:prepass

	sampler2D _MainTex;
	sampler2D _BumpMap;
	fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
	ENDCG
}

FallBack "Legacy Shaders/Transparent/Cutout/Diffuse"
}
