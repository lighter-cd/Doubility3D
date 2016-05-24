using UnityEngine;
using System.Collections;
using System;

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

	ResourceRef skeletonRef;
	ResourceRef[] meshRef = new ResourceRef[4];
	ResourceRef materialRef;
	ResourceRef animationRef;

    // Use this for initialization
    void Start()
    {
		ResourceManager.Instance.resourceEvent += OnResourceComplateEvent;

		skeletonRef = ResourceManager.Instance.addResource (skeletonResource, 0, true);
		for (int i = 0; i < meshResources.Length; i++) {
			meshRef[i] = ResourceManager.Instance.addResource (meshResources[i], 0, true);
		}
		materialRef = ResourceManager.Instance.addResource (materialResouce, 0, true);
		animationRef = ResourceManager.Instance.addResource (animationResource, 0, true);
	}

    // Update is called once per frame
    void Update()
    {
    }

	void OnDestroy()
	{
		ResourceManager.Instance.delResource (skeletonResource);
		for (int i = 0; i < meshResources.Length; i++) {
			ResourceManager.Instance.delResource (meshResources[i]);
		}
		ResourceManager.Instance.delResource (materialResouce);
		ResourceManager.Instance.delResource (animationResource);
	}

	private void OnResourceComplateEvent(object sender, ResourceEventArgs e)
	{
		ResourceRef resource = e.Resources;
		if (resource.State != ResourceState.Error)
		{
			bool bEnd = skeletonRef.IsDone && materialRef.IsDone && animationRef.IsDone;
			if (bEnd) {
				for (int i = 0; i < meshResources.Length; i++) {
					if (!meshRef [i].IsDone) {
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
		GameObject go = skeletonRef.resourceObject.Unity3dObject as GameObject;
		go.transform.parent = gameObject.transform;

		UnityEngine.Material material = materialRef.resourceObject.Unity3dObject as UnityEngine.Material;

		for (int i = 0; i < meshResources.Length; i++)
		{
			GameObject goMesh = new GameObject();
			SkinnedMeshRenderer smr = goMesh.AddComponent<SkinnedMeshRenderer>();

			ResourceObjectMesh mesh = meshRef [i].resourceObject as ResourceObjectMesh;
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

		UnityEngine.AnimationClip clip1 = animationRef.resourceObject.Unity3dObject as UnityEngine.AnimationClip;
		clip1.wrapMode = UnityEngine.WrapMode.Loop;

		Animation animation = gameObject.AddComponent<Animation>();
		animation.AddClip(clip1, "daiji1");
		animation.PlayQueued("daiji1",QueueMode.PlayNow);		
	}
}
