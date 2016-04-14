using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using FlatBuffers;
using Doubility3D.Resource.Schema;

using Schema = Doubility3D.Resource.Schema;


namespace Doubility3D
{
	static public class UnityModelConvert
	{
		static public void Convert (string src, string dstFolder)
		{
			if (src.LastIndexOf ('@') < 0) {
				ConvertMesh (src, dstFolder);
			} else {
				ConvertAction (src, dstFolder);
			}

			EditorUtility.UnloadUnusedAssetsImmediate ();

		}

		static void ConvertMesh (string src, string dstFolder)
		{
			GameObject go = AssetDatabase.LoadAssetAtPath<GameObject> (src);
			if (go != null) {
				// 输出骨架
				ConvertSkeleton (go, src, dstFolder);

				// 输出网格
				SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer> ();
				for (int i = 0; i < smrs.Length; i++) {
					GameObject child = smrs [i].gameObject;
					ConvertMeshPart (child, smrs [i], src, dstFolder);
				}
			} else {
				UnityEngine.Debug.LogError ("资源装载失败:" + src);
			}
		}

		static void ConvertAction (string src, string dstFolder)
		{
            
            
			/*

            builder.StartObject(1);
            Skeletons.AddJoints(builder, jointsOffset);
            Skeletons.EndSkeletons(builder);*/
		}

		static void ConvertSkeleton (GameObject go, string src, string dstFolder)
		{

			// 第一层，寻找自己不带Renderer的节点
			List<UnityEngine.Transform> lstTfs = new List<UnityEngine.Transform> ();
			CollectTransforms (lstTfs, go.transform);

			FlatBufferBuilder builder = new FlatBufferBuilder (1);

			Offset<Header> hdr =  Header.CreateHeader(builder,1,1);

			Offset<Schema.Joint>[] joints = new Offset<Schema.Joint>[lstTfs.Count];
			for (int i = 0; i < lstTfs.Count; i++) {
				int parent = lstTfs.FindIndex (new Predicate<UnityEngine.Transform> (
					                         (target) => {
						return target.Equals (lstTfs [i].parent);
					}
				                         ));
				var name = builder.CreateString (lstTfs[i].name);

				Schema.Joint.StartJoint(builder);
				Schema.Joint.AddNames (builder, name);
				Schema.Joint.AddTransform (builder, Schema.Transform.CreateTransform (builder,
					lstTfs [i].localPosition.x, lstTfs [i].localPosition.y, lstTfs [i].localPosition.z,
					lstTfs [i].localRotation.x, lstTfs [i].localRotation.y, lstTfs [i].localRotation.z, lstTfs [i].localRotation.w,
					lstTfs [i].localScale.x, lstTfs [i].localScale.y, lstTfs [i].localScale.z
				));
				Schema.Joint.AddParent (builder, parent);
				joints[i] = Schema.Joint.EndJoint (builder);
			}
			VectorOffset jointVector = Skeletons.CreateJointsVector(builder,joints);

			Skeletons.StartSkeletons(builder);
			Skeletons.AddJoints(builder,jointVector);
			Offset<Schema.Skeletons> skeleton = Skeletons.EndSkeletons(builder);

			File.StartFile(builder);
			File.AddHeader(builder,hdr);
			File.AddContextType(builder,Context.Skeletons);
			File.AddContext(builder,skeleton.Value);
			Offset<File> file = File.EndFile(builder);

			builder.Finish(file.Value);

			ByteBuffer buf = builder.DataBuffer;
			System.IO.File.WriteAllBytes(dstFolder + "/skeleton.db3d",  buf.Data);
		}

		static void CollectTransforms (List<UnityEngine.Transform> lstTfs, UnityEngine.Transform parent)
		{
			for (int i = 0; i < parent.childCount; i++) {
				UnityEngine.Transform tf = parent.transform.GetChild (i);
				if (tf.gameObject.GetComponent<Renderer> () == null) {
					lstTfs.Add (tf);
					CollectTransforms (lstTfs, tf);
				}
			}
		}

		static void ConvertMeshPart (GameObject go, SkinnedMeshRenderer smr, string src, string dstFolder)
		{
			int count = smr.sharedMesh.subMeshCount;
		}
	}
}