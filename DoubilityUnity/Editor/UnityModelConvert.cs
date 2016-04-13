using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Doubility3D
{
	static public class UnityModelConvert
	{
		static public void Convert(string src,string dstFolder)
		{
			if(src.LastIndexOf('@')<0){
				ConvertMesh(src,dstFolder);
			}else{
				ConvertAction(src,dstFolder);
			}

			EditorUtility.UnloadUnusedAssetsImmediate();

		}
		static void ConvertMesh(string src,string dstFolder)
		{
			GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(src);
			if(go != null){
				// 输出骨架


				// 输出网格
				SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
				for(int i=0;i<smrs.Length;i++){
					GameObject child = smrs[i].gameObject;
					ConvertMeshPart(child,smrs[i],src,dstFolder);
				}
			}
			else{
				UnityEngine.Debug.LogError("资源装载失败:" + src);
			}
		}
		static void ConvertAction(string src,string dstFolder)
		{
			
		}

		static void ConvertSkeleton(GameObject go,string src,string dstFolder)
		{
			
		}

		static void ConvertMeshPart(GameObject go,SkinnedMeshRenderer smr,string src,string dstFolder)
		{
			int count = smr.sharedMesh.subMeshCount;
		}
	}
}