﻿using System;
using UnityEngine;

using Doubility3D.Resource.Manager;
using Doubility3D.Resource.ResourceObj;
using Doubility3D.Resource.Downloader;
using Doubility3D.Util;


public class CharactorLoader : MonoBehaviour
{

	public string[] urls = {
		"Character/Suit_Metal_Dragon_Male/skeleton.doub",
		"Character/Suit_Metal_Dragon_Male/helmet@Suit_Metal_Dragon_Male.doub",
		"Character/Suit_Metal_Dragon_Male/armor@Suit_Metal_Dragon_Male.doub",
		"Character/Suit_Metal_Dragon_Male/gauntlets@Suit_Metal_Dragon_Male.doub",
		"Character/Suit_Metal_Dragon_Male/legs@Suit_Metal_Dragon_Male.doub",
		"Character/Suit_Metal_Dragon_Male/Suit_Metal_Dragon_Male.doub",
		"Character/Suit_Metal_Dragon_Male/dm_daiji.doub"
	};

    // Use this for initialization
    void Start()
    {
		CoroutineRunner.Instance.ActRunner = (e) => {
			new Task (e);
		};
		try{
			DownloaderFactory.Instance.InitializeWithConfig("file_mode.json");
			ShaderManager.Instance.LoadAssetBundle (OnShaderManagerComplate);
		}catch(Exception e){
			Debug.LogException (e);
		}
	}

    // Update is called once per frame
    void Update()
    {
    }

	void OnDestroy()
	{
		ResourceManager.Instance.delResources (urls);
	}


	private void OnShaderManagerComplate(ShaderLoadResult result, string error)
	{
		if(result == ShaderLoadResult.Ok){
			ResourceManager.Instance.addResources(urls,null,true,OnComplate,OnError);
		}else{
			UnityEngine.Debug.LogError("ShaderLoadResult = " + result.ToString() + " info = " + error);	
		}
	}

	void OnError(Exception e){
		UnityEngine.Debug.LogError(e.Message);
	}

	void OnComplate(ResourceRef[] refs){

		GameObject go = GameObject.Instantiate(refs[0].resourceObject.Unity3dObject as GameObject);
		go.name = refs [0].resourceObject.Unity3dObject.name;
		go.transform.parent = gameObject.transform;

		UnityEngine.Material material = refs[5].resourceObject.Unity3dObject as UnityEngine.Material;

		for (int i = 0; i < 4; i++)
		{
			GameObject goMesh = new GameObject();
			SkinnedMeshRenderer smr = goMesh.AddComponent<SkinnedMeshRenderer>();

			ResourceObjectMesh mesh = refs[i+1].resourceObject as ResourceObjectMesh;
			smr.sharedMesh = mesh.Unity3dObject as UnityEngine.Mesh;

			UnityEngine.Transform[] bones = new UnityEngine.Transform[mesh.joints.Length];
			for (int j = 0; j < mesh.joints.Length; j++)
			{
				bones[j] = TransformFinder.Find(transform, mesh.joints[j]);
				if (bones[j] == null)
				{
					UnityEngine.Debug.LogError("TransformFinder.Find(" + mesh.joints[j] + ") == null");
				}
			}
			smr.bones = bones;
			smr.sharedMaterial = material;
			goMesh.transform.parent = gameObject.transform;
		}

		UnityEngine.AnimationClip clip1 = refs[6].resourceObject.Unity3dObject as UnityEngine.AnimationClip;
		clip1.wrapMode = UnityEngine.WrapMode.Loop;

		Animation animation = gameObject.AddComponent<Animation>();
		animation.AddClip(clip1, "daiji1");
		animation.PlayQueued("daiji1",QueueMode.PlayNow);		
	}
}
