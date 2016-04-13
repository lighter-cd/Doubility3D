#ifndef MY_LIGHTMAP_INCLUDED
#define MY_LIGHTMAP_INCLUDED

#include "UnityGlobalIllumination.cginc"

/////////////////////////////////////////////////////////////////////////
//	MyUnityGI_Base 代替 UnityGI_Base
/////////////////////////////////////////////////////////////////////////

inline UnityGI MyUnityGI_Base(UnityGIInput data, half occlusion, half3 normalWorld)
{
	UnityGI o_gi;
	ResetUnityGI(o_gi);


	#if !defined(LIGHTMAP_ON)
		o_gi.light = data.light;
		o_gi.light.color *= data.atten;
	#endif

	#if UNITY_SHOULD_SAMPLE_SH
		o_gi.indirect.diffuse = ShadeSHPerPixel (normalWorld, data.ambient);
	#endif

	#if defined(LIGHTMAP_ON)
		// Baked lightmaps
		fixed4 bakedColorTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, data.lightmapUV.xy);
		half3 bakedColor = DecodeLightmap(bakedColorTex);

		#ifdef DIRLIGHTMAP_OFF
			o_gi.indirect.diffuse = bakedColorTex;

			#ifdef SHADOWS_SCREEN
				o_gi.indirect.diffuse = MixLightmapWithRealtimeAttenuation (o_gi.indirect.diffuse, data.atten, bakedColorTex);
			#endif // SHADOWS_SCREEN

		#elif DIRLIGHTMAP_COMBINED
			fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER (unity_LightmapInd, unity_Lightmap, data.lightmapUV.xy);
			o_gi.indirect.diffuse = DecodeDirectionalLightmap (bakedColor, bakedDirTex, normalWorld);

			#ifdef SHADOWS_SCREEN
				o_gi.indirect.diffuse = MixLightmapWithRealtimeAttenuation (o_gi.indirect.diffuse, data.atten, bakedColorTex);
			#endif // SHADOWS_SCREEN

		#elif DIRLIGHTMAP_SEPARATE
			// Direct
			half2 uvLightmap = data.lightmapUV;
			uvLightmap.x = min(uvLightmap.x,0.49);
			uvLightmap.x = max(uvLightmap.x,0.01);
			uvLightmap.y = min(uvLightmap.y,0.99);
			uvLightmap.y = max(uvLightmap.y,0.01);
			fixed4 bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, uvLightmap);
			o_gi.indirect.diffuse = DecodeDirectionalSpecularLightmap (bakedColor, bakedDirTex, normalWorld, false, 0, o_gi.light);

			// Indirect
			half2 uvIndirect = data.lightmapUV.xy + half2(0.5, 0);
			uvIndirect.x = min(uvIndirect.x,0.99);
			uvIndirect.x = max(uvIndirect.x,0.51);
			uvIndirect.y = min(uvIndirect.y,0.99);
			uvIndirect.y = max(uvIndirect.y,0.01);

			bakedColor = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, uvIndirect));
			bakedDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd, unity_Lightmap, uvIndirect);
			o_gi.indirect.diffuse += DecodeDirectionalSpecularLightmap (bakedColor, bakedDirTex, normalWorld, false, 0, o_gi.light2);

			// Left halves of both intensity and direction lightmaps store direct light; right halves - indirect.
			#ifdef SHADOWS_SCREEN
				o_gi.light.color = MixLightmapWithRealtimeAttenuation (o_gi.light.color, data.atten, bakedColorTex);
			#endif // SHADOWS_SCREEN
		#endif
	#endif

	#ifdef DYNAMICLIGHTMAP_ON
		// Dynamic lightmaps
		fixed4 realtimeColorTex = UNITY_SAMPLE_TEX2D(unity_DynamicLightmap, data.lightmapUV.zw);
		half3 realtimeColor = DecodeRealtimeLightmap (realtimeColorTex);

		#ifdef DIRLIGHTMAP_OFF
			o_gi.indirect.diffuse += realtimeColor;

		#elif DIRLIGHTMAP_COMBINED
			half4 realtimeDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, data.lightmapUV.zw);
			o_gi.indirect.diffuse += DecodeDirectionalLightmap (realtimeColor, realtimeDirTex, normalWorld);

		#elif DIRLIGHTMAP_SEPARATE
			half4 realtimeDirTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicDirectionality, unity_DynamicLightmap, data.lightmapUV.zw);
			half4 realtimeNormalTex = UNITY_SAMPLE_TEX2D_SAMPLER(unity_DynamicNormal, unity_DynamicLightmap, data.lightmapUV.zw);
			o_gi.indirect.diffuse += DecodeDirectionalSpecularLightmap (realtimeColor, realtimeDirTex, normalWorld, true, realtimeNormalTex, o_gi.light3);
		#endif
	#endif

	o_gi.indirect.diffuse *= occlusion;

	return o_gi;
}

/////////////////////////////////////////////////////////////////////////
//	MyUnityGlobalIllumination 代替 UnityGlobalIllumination
/////////////////////////////////////////////////////////////////////////

inline UnityGI MyUnityGlobalIllumination (UnityGIInput data, half occlusion, half3 normalWorld)
{
	return MyUnityGI_Base(data, occlusion, normalWorld);
}

inline UnityGI MyUnityGlobalIllumination (UnityGIInput data, half occlusion, half3 normalWorld, Unity_GlossyEnvironmentData glossIn)
{
	UnityGI o_gi = MyUnityGI_Base(data, occlusion, normalWorld);
	o_gi.indirect.specular = UnityGI_IndirectSpecular(data, occlusion, normalWorld, glossIn);
	return o_gi;
}

//
// Old MyUnityGlobalIllumination signatures. Kept only for backward compatibility and will be removed soon
//

inline UnityGI MyUnityGlobalIllumination (UnityGIInput data, half occlusion, half oneMinusRoughness, half3 normalWorld, bool reflections)
{
	if(reflections)
	{
		Unity_GlossyEnvironmentData g;
		g.roughness		= 1 - oneMinusRoughness;
		g.reflUVW		= normalWorld;
		return MyUnityGlobalIllumination(data, occlusion, normalWorld, g);
	}
	else
	{
		return MyUnityGlobalIllumination(data, occlusion, normalWorld);
	}
}
inline UnityGI MyUnityGlobalIllumination (UnityGIInput data, half occlusion, half oneMinusRoughness, half3 normalWorld)
{
#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
	// No need to sample reflection probes during deferred G-buffer pass
	bool sampleReflections = false;
#else
	bool sampleReflections = true;
#endif
	return MyUnityGlobalIllumination (data, occlusion, oneMinusRoughness, normalWorld, sampleReflections);
}

/////////////////////////////////////////////////////////////////////////
//	MY_UNITY_GI 代替 UNITY_GI
/////////////////////////////////////////////////////////////////////////

#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
	#define MY_UNITY_GI(x, s, data) x = MyUnityGlobalIllumination (data, s.Occlusion, s.Normal);
#else
	#define MY_UNITY_GI(x, s, data) 								\
		UNITY_GLOSSY_ENV_FROM_SURFACE(g, s, data);				\
		x = MyUnityGlobalIllumination (data, s.Occlusion, s.Normal, g);
#endif

/////////////////////////////////////////////////////////////////////////
//	MyStandard 光照模式
/////////////////////////////////////////////////////////////////////////

inline half4 LightingMyStandard (SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
{
	return LightingStandard(s,viewDir,gi);
}
inline void LightingMyStandard_GI (
	SurfaceOutputStandard s,
	UnityGIInput data,
	inout UnityGI gi)
{
	MY_UNITY_GI(gi, s, data);
}
inline half4 LightingMyStandard_Deferred (SurfaceOutputStandard s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
{
	return LightingStandard_Deferred(s,viewDir,gi,outDiffuseOcclusion,outSpecSmoothness,outNormal);
}

/////////////////////////////////////////////////////////////////////////
//	MyStandardSpecular 光照模式
/////////////////////////////////////////////////////////////////////////

inline half4 LightingMyStandardSpecular (SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi)
{
	return LightingStandardSpecular(s,viewDir,gi);
}
inline void LightingMyStandardSpecular_GI (
	SurfaceOutputStandardSpecular s,
	UnityGIInput data,
	inout UnityGI gi)
{
	MY_UNITY_GI(gi, s, data);
}
inline half4 LightingMyStandardSpecular_Deferred (SurfaceOutputStandardSpecular s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
{
	return LightingStandardSpecular_Deferred(s,viewDir,gi,outDiffuseOcclusion,outSpecSmoothness,outNormal);
}

/////////////////////////////////////////////////////////////////////////
//	MyBlinnPhong 光照模式
/////////////////////////////////////////////////////////////////////////

inline half4 LightingMyBlinnPhong (SurfaceOutput s, half3 viewDir, UnityGI gi)
{
	return LightingBlinnPhong(s,viewDir,gi);
}
inline void LightingMyBlinnPhong_GI (
	SurfaceOutput s,
	UnityGIInput data,
	inout UnityGI gi)
{
	gi = MyUnityGlobalIllumination (data, 1.0, s.Normal);
}
inline half4 LightingMyBlinnPhong_Deferred (SurfaceOutput s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
{
	return LightingBlinnPhong_Deferred(s,viewDir,gi,outDiffuseOcclusion,outSpecSmoothness,outNormal);
}

/////////////////////////////////////////////////////////////////////////
//	MyLambert 光照模式
/////////////////////////////////////////////////////////////////////////

inline half4 LightingMyLambert (SurfaceOutput s, UnityGI gi)
{
	return LightingLambert(s,gi);
}
inline void LightingMyLambert_GI (
	SurfaceOutput s,
	UnityGIInput data,
	inout UnityGI gi)
{
	gi = MyUnityGlobalIllumination (data, 1.0, s.Normal);
}

inline half4 LightingMyLambert_Deferred (SurfaceOutput s, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
{
	return LightingLambert_Deferred(s,gi,outDiffuseOcclusion,outSpecSmoothness,outNormal);
}


#endif // MY_LIGHTMAP_INCLUDED
