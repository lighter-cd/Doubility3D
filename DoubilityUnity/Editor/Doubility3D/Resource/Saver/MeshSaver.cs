using System;
using System.Collections.Generic;
using UnityEngine;

using FlatBuffers;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

namespace Doubility3D.Resource.Saver
{
    struct MeshComponent
    {
        public Action actStart;
        public Action<int> actElem;
        public Func<bool> funcExist;
        public Action<VectorOffset> actAdd;

        public MeshComponent(Func<bool> exist, Action start, Action<int> elem, Action<VectorOffset> add)
        {
            actStart = start;
            actElem = elem;
            funcExist = exist;
            actAdd = add;
        }
    };

    public static class MeshSaver
    {
        const int InitBufferSize = 8192;

        public static ByteBuffer Save(UnityEngine.Mesh mesh, UnityEngine.Transform[] bones)
        {
            FlatBufferBuilder builder = new FlatBufferBuilder(InitBufferSize);

            // 创建顶点成分
            MeshComponent[] components = 
            {
                // vertices
                new MeshComponent(
                    () => { return true; },    
                    () => { Schema.Mesh.StartVerticesVector(builder, mesh.vertexCount); },
                    (i) => { Vec3.CreateVec3(builder, mesh.vertices[i].x, mesh.vertices[i].y, mesh.vertices[i].z); },
                    (vo) =>{Schema.Mesh.AddVertices(builder,vo);}
                    ),
                // uv
                new MeshComponent(
					() => { return (mesh.uv != null)&&(mesh.uv.Length>0); },    
                    () => { Schema.Mesh.StartUvVector(builder, mesh.vertexCount); },
                    (i) => {Vec2.CreateVec2(builder, mesh.uv[i].x, mesh.uv[i].y); },
                    (vo) =>{Schema.Mesh.AddUv(builder,vo);}
                    ),
                // uv2
                new MeshComponent(
					() => { return (mesh.uv2 != null)&&(mesh.uv2.Length>0); },    
                    () => { Schema.Mesh.StartUv2Vector(builder, mesh.vertexCount); },
                    (i) => {Vec2.CreateVec2(builder, mesh.uv2[i].x, mesh.uv2[i].y); },
                    (vo) =>{Schema.Mesh.AddUv2(builder,vo);}
                    ),
                // uv3
                new MeshComponent(
					() => { return (mesh.uv3 != null)&&(mesh.uv3.Length>0); },    
                    () => { Schema.Mesh.StartUv3Vector(builder, mesh.vertexCount); },
                    (i) => {Vec2.CreateVec2(builder, mesh.uv3[i].x, mesh.uv3[i].y); },
                    (vo) =>{Schema.Mesh.AddUv3(builder,vo);}
                    ),
                // uv4
                new MeshComponent(
					() => { return (mesh.uv4 != null) && (mesh.uv4.Length>0); },    
                    () => { Schema.Mesh.StartUv4Vector(builder, mesh.vertexCount); },
                    (i) => {Vec2.CreateVec2(builder, mesh.uv4[i].x, mesh.uv4[i].y); },
                    (vo) =>{Schema.Mesh.AddUv4(builder,vo);}
                    ),
                // normals
                new MeshComponent(
					() => { return (mesh.normals != null) && (mesh.normals.Length>0); },    
                    () => { Schema.Mesh.StartNormalsVector(builder, mesh.vertexCount); },
                    (i) => {Vec3.CreateVec3(builder, mesh.normals[i].x, mesh.normals[i].y, mesh.normals[i].z); },
                    (vo) =>{Schema.Mesh.AddNormals(builder,vo);}
                    ),
                // tangents
                new MeshComponent(
					() => { return (mesh.tangents != null) && (mesh.tangents.Length>0); },    
                    () => { Schema.Mesh.StartTangentsVector(builder, mesh.vertexCount); },
                    (i) => {Vec3.CreateVec3(builder, mesh.tangents[i].x, mesh.tangents[i].y, mesh.tangents[i].z); },
                    (vo) =>{Schema.Mesh.AddTangents(builder,vo);}
                    ),
                // colors
                new MeshComponent(
					() => { return (mesh.colors != null) && (mesh.colors.Length>0); },    
                    () => { Schema.Mesh.StartColorsVector(builder, mesh.vertexCount); },
                    (i) => {Schema.Color.CreateColor(builder, mesh.colors[i].a, mesh.colors[i].b, mesh.colors[i].g, mesh.colors[i].r); },
                    (vo) =>{Schema.Mesh.AddColors(builder,vo);}
                    ),
                // colors32
                new MeshComponent(
					() => { return (mesh.colors32 != null) && (mesh.colors.Length>0); },    
                    () => { Schema.Mesh.StartColors32Vector(builder, mesh.vertexCount); },
                    (i) => {Schema.Color32.CreateColor32(builder, mesh.colors32[i].a, mesh.colors32[i].b, mesh.colors32[i].g, mesh.colors32[i].r); },
                    (vo) =>{Schema.Mesh.AddColors32(builder,vo);}
                    ),
                // BoneWeigths
                new MeshComponent(
					() => { return (mesh.boneWeights != null) && (mesh.boneWeights.Length>0); },    
                    () => { Schema.Mesh.StartBoneWeightsVector(builder, mesh.vertexCount); },
                    (i) => {Schema.BoneWeight.CreateBoneWeight(builder, 
                        mesh.boneWeights[i].boneIndex0, mesh.boneWeights[i].boneIndex1, mesh.boneWeights[i].boneIndex2, mesh.boneWeights[i].boneIndex3,
                        mesh.boneWeights[i].weight0, mesh.boneWeights[i].weight1, mesh.boneWeights[i].weight2, mesh.boneWeights[i].weight3
                        ); },
                    (vo) =>{Schema.Mesh.AddBoneWeights(builder,vo);}
                    )
            };

            int componentCount = components.Length;
            VectorOffset[] vectorOffsets = new VectorOffset[componentCount];
            for (int c = 0; c < componentCount; c++)
            {
                if (components[c].funcExist())
                {
                    components[c].actStart();
                    for (int i = mesh.vertexCount-1; i >= 0; i--)   // 添加时顺序颠倒。
                    {
                        components[c].actElem(i);   // 结构vector不需要AddOffset
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
            StringOffset[] offJoints = new StringOffset[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                offJoints[i] = builder.CreateString(bones[i].name);
            }

			// 三角面索引数据  标量数组可以直接添加。
			VectorOffset vecTrangles = Schema.Mesh.CreateTrianglesVector(builder, mesh.triangles);
			// Submesh 数据
			VectorOffset vecSubmeshes = Schema.Mesh.CreateSubmeshesVector(builder, offSubmeshes);
			// 涉及到的骨骼
			VectorOffset vecJoints = Schema.Mesh.CreateJointsVector(builder, offJoints);

            // 放入Mesh
            Schema.Mesh.StartMesh(builder);
            // 顶点数据
            for (int c = 0; c < componentCount; c++)
            {
                if (components[c].funcExist())
                {
                    components[c].actAdd(vectorOffsets[c]);
                }
            }
			Schema.Mesh.AddTriangles(builder,vecTrangles);
			Schema.Mesh.AddSubmeshes(builder,vecSubmeshes);
			Schema.Mesh.AddJoints(builder,vecJoints);

            // 边界
            Vector3 min = mesh.bounds.min;
            Vector3 max = mesh.bounds.max;
            Schema.Mesh.AddBound(builder, Schema.Bound.CreateBound(builder, min.x, min.y, min.z, max.x, max.y, max.z));
            Offset<Schema.Mesh> offMesh = Schema.Mesh.EndMesh(builder);

            builder.Finish(offMesh.Value);
            return builder.DataBuffer;
        }
    }
}
