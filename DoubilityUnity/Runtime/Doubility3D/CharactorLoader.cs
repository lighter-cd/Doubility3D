using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using RSG;

using Doubility3D.Resource.Manager;
using Doubility3D.Resource.ResourceObj;
using Doubility3D.Util;


public class CharactorLoader : MonoBehaviour
{

    public string skeletonResource = "Character/Suit_Metal_Dragon_Male/skeleton.doub";
    public string[] meshResources = {
                                        "Character/Suit_Metal_Dragon_Male/helmet@Suit_Metal_Dragon_Male.doub",
                                        "Character/Suit_Metal_Dragon_Male/armor@Suit_Metal_Dragon_Male.doub",
                                        "Character/Suit_Metal_Dragon_Male/gauntlets@Suit_Metal_Dragon_Male.doub",
                                        "Character/Suit_Metal_Dragon_Male/legs@Suit_Metal_Dragon_Male.doub"
                                    };
    public string materialResouce = "Character/Suit_Metal_Dragon_Male/Suit_Metal_Dragon_Male.doub";
    public string animationResource = "Character/Suit_Metal_Dragon_Male/dm_daiji.doub";

	Dictionary<string, ResourceRef> dictResources = new Dictionary<string, ResourceRef> ();

    // Use this for initialization
    void Start()
    {
		ShaderManager.Instance.LoadAssetBundle (OnShaderManagerComplate);
	}

    // Update is called once per frame
    void Update()
    {
    }

	void OnDestroy()
	{
		dictResources.Add (skeletonResource, null);
		for (int i = 0; i < meshResources.Length; i++) {
			dictResources.Add (meshResources[i], null);
		}
		dictResources.Add (materialResouce, null);
		dictResources.Add (animationResource, null);

		ResourceManager.Instance.delResource (skeletonResource);
		for (int i = 0; i < meshResources.Length; i++) {
			ResourceManager.Instance.delResource (meshResources[i]);
		}
		ResourceManager.Instance.delResource (materialResouce);
		ResourceManager.Instance.delResource (animationResource);
	}


	private void OnShaderManagerComplate(string error)
	{
		Action<ResourceRef> actResolved = OnResolved;
		ResourceManager.Instance.addResource (skeletonResource, 0, true).Then (actResolved).Done();
		for (int i = 0; i < meshResources.Length; i++) {
			ResourceManager.Instance.addResource (meshResources[i], 0, true).Then (actResolved).Done();
		}
		ResourceManager.Instance.addResource (materialResouce, 0, true).Then (actResolved).Done();
		ResourceManager.Instance.addResource (animationResource, 0, true).Then (actResolved).Done();
	}

	void OnResolved(ResourceRef resource){
		if (resource.State != ResourceState.Error)
		{
			dictResources [resource.Path] = resource;
			bool bEnd = false;
			if (bEnd) {
				Dictionary<string, ResourceRef>.Enumerator e = dictResources.GetEnumerator ();
				while(e.MoveNext()){
					if (e.Current.Value == null) {
						bEnd = false;
						break;
					}
				}
			}
			if (bEnd) {
				OnComplate ();
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("资源 " + resource.Path + "装载出错");
		}
	}


	void OnComplate(){
		GameObject go = dictResources[skeletonResource].resourceObject.Unity3dObject as GameObject;
		go.transform.parent = gameObject.transform;

		UnityEngine.Material material = dictResources[materialResouce].resourceObject.Unity3dObject as UnityEngine.Material;

		for (int i = 0; i < meshResources.Length; i++)
		{
			GameObject goMesh = new GameObject();
			SkinnedMeshRenderer smr = goMesh.AddComponent<SkinnedMeshRenderer>();

			ResourceObjectMesh mesh = dictResources[meshResources[i]].resourceObject as ResourceObjectMesh;
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

		UnityEngine.AnimationClip clip1 = dictResources[animationResource].resourceObject.Unity3dObject as UnityEngine.AnimationClip;
		clip1.wrapMode = UnityEngine.WrapMode.Loop;

		Animation animation = gameObject.AddComponent<Animation>();
		animation.AddClip(clip1, "daiji1");
		animation.PlayQueued("daiji1",QueueMode.PlayNow);		
	}
}
