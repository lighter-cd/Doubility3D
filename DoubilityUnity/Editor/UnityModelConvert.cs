using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using FlatBuffers;
using Doubility3D.Resource.Schema;

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
                ConvertSkeleton(go, src, dstFolder);

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
            
            
            /*

            builder.StartObject(1);
            Skeletons.AddJoints(builder, jointsOffset);
            Skeletons.EndSkeletons(builder);*/
		}

		static void ConvertSkeleton(GameObject go,string src,string dstFolder)
		{
			// 第一层，寻找自己不带Renderer的节点
            List<UnityEngine.Transform> lstTfs = new List<UnityEngine.Transform>();
            CollectTransforms(lstTfs, go.transform);

            FlatBufferBuilder builder = new FlatBufferBuilder(1024);
            builder.StartObject(lstTfs.Count);
            for (int i = 0; i < lstTfs.Count; i++)
            {
                int parent = lstTfs.FindIndex(new Predicate<UnityEngine.Transform>(
                        (target)=>{
                            return target.Equals(lstTfs[i].parent);
                        }
                    ));
                Doubility3D.Resource.Schema.Joint.AddNames(builder, builder.CreateString(lstTfs[i].name));
                Doubility3D.Resource.Schema.Joint.AddTransform(builder, Doubility3D.Resource.Schema.Transform.CreateTransform(builder,
                       lstTfs[i].localPosition.x, lstTfs[i].localPosition.y, lstTfs[i].localPosition.z,
                       lstTfs[i].localRotation.x,lstTfs[i].localRotation.y,lstTfs[i].localRotation.z,lstTfs[i].localRotation.w,
                       lstTfs[i].localScale.x, lstTfs[i].localScale.y, lstTfs[i].localScale.z
                    ));
                Doubility3D.Resource.Schema.Joint.AddParent(builder, parent);
            }
            Doubility3D.Resource.Schema.Joint.EndJoint(builder);
            
		}

        static void CollectTransforms(List<UnityEngine.Transform> lstTfs,UnityEngine.Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                UnityEngine.Transform tf = parent.transform.GetChild(i);
                if (tf.gameObject.GetComponent<Renderer>() == null)
                {
                    lstTfs.Add(tf);
                    CollectTransforms(lstTfs, tf);
                }
            }
        }

		static void ConvertMeshPart(GameObject go,SkinnedMeshRenderer smr,string src,string dstFolder)
		{
			int count = smr.sharedMesh.subMeshCount;
		}
	}
}