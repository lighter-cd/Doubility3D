using UnityEngine;
using UnityEditor;
using System.Collections;

using Doubility3D.Resource.Loader;
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
        // todo:写文件加入以下属性
        clip1.frameRate = 30;
        clip1.legacy = true;
        clip1.wrapMode = UnityEngine.WrapMode.Loop;

        Animation animation = gameObject.AddComponent<Animation>();
        animation.AddClip(clip1, "daiji1");
        animation.Play("daiji1");
    }

    // Update is called once per frame
    void Update()
    {

    }


    UnityEngine.Object LoadFromFile(string resource)
    {
        UnityEngine.Object result = null;
        string file = Application.streamingAssetsPath + "/.root/" + resource;
        Schema.Context context = Context.Unknown;
        ByteBuffer bb = FileLoader.LoadFromFile(file, out context);
        switch (context)
        {
            case Context.Skeletons:
                result = SkeletonLoader.Load(bb);
                break;
            case Context.Mesh:
                {
                    result = MeshLoader.Load(bb, out joints);
                }
                break;
            case Context.Material:
                {
                    result = MaterialLoader.Load(bb, GetShader, GetTexture);
                }
                break;
            case Context.AnimationClip:
                {
                    result = AnimationClipLoader.Load(bb);
                }
                break;
        }
        return result;
    }
    Shader GetShader(string name)
    {
        return Shader.Find(name);
    }
    Texture GetTexture(string nameTexture, string nameProperty)
    {
        // 暂时使用 assets目录中的纹理
        return AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ArtWork/Character/Suit_Metal_Dragon_Male/Materials/" + nameTexture + ".tga");
    }
}
