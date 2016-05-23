using UnityEngine;
using System.Collections;
using System;

using Doubility3D.Resource.Serializer;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using FlatBuffers;

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

    string[] joints = null;
	string home;
	AssetBundle ab;
	Shader shader;

	void Awake(){
		home = Environment.GetEnvironmentVariable ("DOUBILITY_HOME",EnvironmentVariableTarget.Machine);
		if (string.IsNullOrEmpty (home)) {
			home = Application.streamingAssetsPath;
		}
		home = home.Replace ('\\', '/');

		string coreDataBundle = home + "/.coreData/" + PlatformPath.GetPath(Application.platform) + "/coreData.bundle";
		ab = AssetBundle.LoadFromFile(coreDataBundle);
		if(ab != null){
			shader = ab.LoadAsset<Shader>("Assets/Doubility3D/CoreData/Shaders/Charactor/Charactor-BumpSpec.shader");
		}
	}

    // Use this for initialization
    void Start()
    {
        GameObject go = LoadFromFile(skeletonResource) as GameObject;
        go.transform.parent = gameObject.transform;

        UnityEngine.Material material = LoadFromFile(materialResouce) as UnityEngine.Material;

        for (int i = 0; i < meshResources.Length; i++)
        {
            GameObject goMesh = new GameObject();
            SkinnedMeshRenderer smr = goMesh.AddComponent<SkinnedMeshRenderer>();
            joints = null;
            smr.sharedMesh = LoadFromFile(meshResources[i]) as UnityEngine.Mesh;

            UnityEngine.Transform[] bones = new UnityEngine.Transform[joints.Length];
            for (int j = 0; j < joints.Length; j++)
            {
                bones[j] = TransformFinder.Find(transform, joints[j]);
                if (bones[j] == null)
                {
                    UnityEngine.Debug.LogError("TransformFinder.Find(" + joints[j] + ") == null");
                }
            }
            smr.bones = bones;
            smr.sharedMaterial = material;
            goMesh.transform.parent = gameObject.transform;
        }

        UnityEngine.AnimationClip clip1 = LoadFromFile(animationResource) as UnityEngine.AnimationClip;
        clip1.wrapMode = UnityEngine.WrapMode.Loop;

        Animation animation = gameObject.AddComponent<Animation>();
        animation.AddClip(clip1, "daiji1");
		animation.PlayQueued("daiji1",QueueMode.PlayNow);
    }

    // Update is called once per frame
    void Update()
    {

    }


    UnityEngine.Object LoadFromFile(string resource)
    {
		UnityEngine.Object result = null;
        string file = home + "/.root/" + resource;
        Schema.Context context = Context.Unknown;
        ByteBuffer bb = FileSerializer.LoadFromFile(file, out context);
		ISerializer serializer = SerializerFactory.Instance.Create (context);
		if (serializer != null) {
			String[] dependences = null;
			result = serializer.Parse(bb,out dependences);
			for (int i = 0; i < dependences.Length; i++) {
			}
			serializer = null;
		}
        return result;
    }
    Shader GetShader(string name)
    {
		return shader;
		//return Shader.Find(name);
    }
    UnityEngine.Texture GetTexture(string nameTexture, string nameProperty)
    {
		string platform = PlatformPath.GetPath(Application.platform).ToLower();
		string path = home + "/.root/" + nameTexture + "." + platform +".texture";

		UnityEngine.Debug.Log(path);

		Schema.Context context = Context.Unknown;
		ByteBuffer bb = FileSerializer.LoadFromFile(path,out context);
		if(context == Context.Texture && (bb != null)){
			//return TextureSerializer.Load(bb);
		}
		return null;
    }
}
