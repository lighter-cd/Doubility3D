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
    struct MeshComponent
    {
        public Action actStart;
        public Func<int, int> funcElem;
        public Func<bool> funcExist;
        public Action<VectorOffset> actAdd;

        public MeshComponent(Func<bool> exist,Action start,Func<int, int> elem,Action<VectorOffset> add)
        {
            actStart = start;
            funcElem = elem;
            funcExist = exist;
            actAdd = add;
        }
    };

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
 
		}

		static void ConvertSkeleton (GameObject go, string src, string dstFolder)
		{

			// 第一层，寻找自己不带Renderer的节点
			List<UnityEngine.Transform> lstTfs = new List<UnityEngine.Transform> ();
			CollectTransforms (lstTfs, go.transform);

            FlatBufferBuilder builder = new FlatBufferBuilder(InitBufferSize);

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
            
            int length = buf.Length-buf.Position;
            byte[] wbuf = new byte[length];
            Array.Copy(buf.Data, buf.Position, wbuf, 0, length);

            System.IO.File.WriteAllBytes(dstFolder + "/skeleton.db3d", wbuf);
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
            UnityEngine.Mesh mesh = smr.sharedMesh;


            FlatBufferBuilder builder = new FlatBufferBuilder(InitBufferSize);

            // 创建顶点成分
            VectorOffset[] vectorOffsets = new VectorOffset[10];
            MeshComponent[] components = 
            {
                // vertices
                new MeshComponent(
                    () => { return true; },    
                    () => { Schema.Mesh.StartVerticesVector(builder, mesh.vertexCount); },
                    (i) => { return Vec3.CreateVec3(builder, mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z).Value; },
                    (vo) =>{Schema.Mesh.AddVertices(builder,vo);}
                    ),
                // uv
                new MeshComponent(
                    () => { return mesh.uv != null; },    
                    () => { Schema.Mesh.StartUvVector(builder, mesh.vertexCount); },
                    (i) => { return Vec2.CreateVec2(builder, mesh.uv[i].x, mesh.uv[i].y).Value; },
                    (vo) =>{Schema.Mesh.AddUv(builder,vo);}
                    ),
                // uv2
                new MeshComponent(
                    () => { return mesh.uv2 != null; },    
                    () => { Schema.Mesh.StartUv2Vector(builder, mesh.vertexCount); },
                    (i) => { return Vec2.CreateVec2(builder, mesh.uv2[i].x, mesh.uv2[i].y).Value; },
                    (vo) =>{Schema.Mesh.AddUv2(builder,vo);}
                    ),
                // uv3
                new MeshComponent(
                    () => { return mesh.uv3 != null; },    
                    () => { Schema.Mesh.StartUv3Vector(builder, mesh.vertexCount); },
                    (i) => { return Vec2.CreateVec2(builder, mesh.uv3[i].x, mesh.uv3[i].y).Value; },
                    (vo) =>{Schema.Mesh.AddUv3(builder,vo);}
                    ),
                // uv4
                new MeshComponent(
                    () => { return mesh.uv4 != null; },    
                    () => { Schema.Mesh.StartUv4Vector(builder, mesh.vertexCount); },
                    (i) => { return Vec2.CreateVec2(builder, mesh.uv4[i].x, mesh.uv4[i].y).Value; },
                    (vo) =>{Schema.Mesh.AddUv4(builder,vo);}
                    ),
                // normals
                new MeshComponent(
                    () => { return mesh.normals != null; },    
                    () => { Schema.Mesh.StartNormalsVector(builder, mesh.vertexCount); },
                    (i) => { return Vec3.CreateVec3(builder, mesh.normals[i].x, mesh.normals[i].y, mesh.normals[i].z).Value; },
                    (vo) =>{Schema.Mesh.AddNormals(builder,vo);}
                    ),
                // tangents
                new MeshComponent(
                    () => { return mesh.tangents != null; },    
                    () => { Schema.Mesh.StartTangentsVector(builder, mesh.vertexCount); },
                    (i) => { return Vec3.CreateVec3(builder, mesh.tangents[i].x, mesh.tangents[i].y, mesh.tangents[i].z).Value; },
                    (vo) =>{Schema.Mesh.AddTangents(builder,vo);}
                    ),
                // colors
                new MeshComponent(
                    () => { return mesh.colors != null; },    
                    () => { Schema.Mesh.StartColorsVector(builder, mesh.vertexCount); },
                    (i) => { return Schema.Color.CreateColor(builder, mesh.colors[i].a, mesh.colors[i].b, mesh.colors[i].g, mesh.colors[i].r).Value; },
                    (vo) =>{Schema.Mesh.AddColors(builder,vo);}
                    ),
                // colors32
                new MeshComponent(
                    () => { return mesh.colors32 != null; },    
                    () => { Schema.Mesh.StartColors32Vector(builder, mesh.vertexCount); },
                    (i) => { return Schema.Color32.CreateColor32(builder, mesh.colors32[i].a, mesh.colors32[i].b, mesh.colors32[i].g, mesh.colors32[i].r).Value; },
                    (vo) =>{Schema.Mesh.AddColors32(builder,vo);}
                    ),
                // BoneWeigths
                new MeshComponent(
                    () => { return mesh.boneWeights != null; },    
                    () => { Schema.Mesh.StartBoneWeightsVector(builder, mesh.vertexCount); },
                    (i) => { return Schema.BoneWeight.CreateBoneWeight(builder, 
                        mesh.boneWeights[i].boneIndex0, mesh.boneWeights[i].boneIndex1, mesh.boneWeights[i].boneIndex2, mesh.boneWeights[i].boneIndex3,
                        mesh.boneWeights[i].weight0, mesh.boneWeights[i].weight1, mesh.boneWeights[i].weight2, mesh.boneWeights[i].weight3
                        ).Value; },
                    (vo) =>{Schema.Mesh.AddBoneWeights(builder,vo);}
                    )
            };
            for (int c = 0; c < 9; c++)
            {
                if (components[c].funcExist())
                {
                    components[c].actStart();
                    for (int i = 0; i < mesh.vertexCount; i++)
                    {
                        builder.AddOffset(components[c].funcElem(i));
                    }
                    vectorOffsets[c] = builder.EndVector();
                }
            }

            // 创建 Submesh
            Offset<SubMesh>[] offSubmeshes = new Offset<SubMesh>[mesh.subMeshCount];
            uint offset = 0;
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] _is = mesh.GetIndices(i);
                offSubmeshes[i] = SubMesh.CreateSubMesh(builder, offset, (uint)_is.Length, (Schema.MeshTopology)mesh.GetTopology(i));
                offset += (uint)_is.Length;
            }
            // 网格所涉及到的骨骼
            StringOffset[] offJoints = new StringOffset[smr.bones.Length];
            for (int i = 0; i < smr.bones.Length; i++)
            {
                offJoints[i] = builder.CreateString(smr.bones[i].name);
            }


            // 放入Mesh
            Schema.Mesh.StartMesh(builder);
            // 顶点数据
            for (int c = 0; c < 9; c++)
            {
                if (components[c].funcExist())
                {
                    components[c].actAdd(vectorOffsets[c]);
                }
            }
            // 三角面索引数据
            Schema.Mesh.CreateTrianglesVector(builder, mesh.triangles);
            // Submesh 数据
            Schema.Mesh.CreateSubmeshesVector(builder,offSubmeshes);
            // 涉及到的骨骼
            Schema.Mesh.CreateJointsVector(builder, offJoints);
            // 边界
            Vector3 min = mesh.bounds.min;
            Vector3 max = mesh.bounds.max;
            Schema.Mesh.AddBound(builder, Schema.Bound.CreateBound(builder,min.x,min.y,min.z,max.x,max.y,max.z));
            Offset<Schema.Mesh> offMesh = Schema.Mesh.EndMesh(builder);

            // 创建文件
            Offset<Header> hdr = Header.CreateHeader(builder, 2, 1);
            File.StartFile(builder);
            File.AddHeader(builder, hdr);
            File.AddContextType(builder, Context.Mesh);
            File.AddContext(builder, offMesh.Value);
            Offset<File> file = File.EndFile(builder);

            builder.Finish(file.Value);

		}
	}
}