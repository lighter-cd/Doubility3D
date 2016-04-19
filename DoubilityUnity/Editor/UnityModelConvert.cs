using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using FlatBuffers;
using Doubility3D.Resource.Schema;

using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Resource.Saver;


namespace Doubility3D
{
	static public class UnityModelConvert
	{
        const int InitBufferSize = 8192;

		static public void Convert (string src, string dstFolder)
		{
			if (src.LastIndexOf ('@') < 0) {
				ConvertMesh (src, dstFolder);
			} else {
				ConvertAction (src, dstFolder);
			}

			EditorUtility.UnloadUnusedAssetsImmediate ();
            UnityEngine.Debug.Log(src);
		}

		static void ConvertMesh (string src, string dstFolder)
		{
			GameObject go = AssetDatabase.LoadAssetAtPath<GameObject> (src);
			if (go != null) {
				// 输出骨架
                ByteBuffer bfSkeleton = SkeletonSaver.Save(go);
                FileSaver.Save(bfSkeleton, Context.Skeletons, dstFolder + "/skeleton.db3d");

				// 输出网格
				SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer> ();
				for (int i = 0; i < smrs.Length; i++) {

                    string nameMesh = System.IO.Path.GetFileNameWithoutExtension(src);
                    if (smrs.Length > 1)
                    {
                        nameMesh = smrs[i].sharedMesh.name + "@" + nameMesh;
                    }
                    
                    ByteBuffer bfMesh = MeshSaver.Save(smrs[i].sharedMesh, smrs[i].bones);
                    FileSaver.Save(bfMesh, Context.Mesh, dstFolder + "/" + nameMesh + ".db3d");

                    for(int j=0;j<smrs[i].sharedMaterials.Length;j++){
                        ByteBuffer bfMaterial = MaterialSaver.Save(smrs[i].sharedMaterials[j]);
                        FileSaver.Save(bfMaterial, Context.Material, dstFolder + "/" + smrs[i].sharedMaterials[j].name + ".db3d");
                    }
				}
			} else {
				UnityEngine.Debug.LogError ("资源装载失败:" + src);
			}
		}

		static void ConvertAction (string src, string dstFolder)
		{
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(src);
            if (go != null)
            {
                UnityEngine.AnimationClip[] clips = AnimationUtility.GetAnimationClips(go);
                for (int i = 0; i < clips.Length; i++)
                {
                    AnimationClipSaver.Save(clips[i], dstFolder);
                }
            }
            else
            {
                UnityEngine.Debug.LogError("资源装载失败:" + src);
            }
		}
	}
}