using System;
using System.Collections.Generic;

using UnityEngine;

using FlatBuffers;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Resource.ResourceObj;

//using Doubility3D.Util;

namespace Doubility3D.Resource.Unserializing
{
	public class MeshUnserializer : IUnserializer
    {
		public ResourceObject Parse(ByteBuffer bb)
        {
            Schema.Mesh fbMesh = Schema.Mesh.GetRootAsMesh(bb);

            UnityEngine.Mesh mesh = new UnityEngine.Mesh();

            Vec3 v3 = new Vec3();
            Vec2 v2 = new Vec2();
            Vec4 v4 = new Vec4();

            Vector3[] inVertices = new Vector3[fbMesh.VerticesLength];
            for(int i=0;i<fbMesh.VerticesLength;i++){
                Vec3 v = fbMesh.GetVertices(v3,i);
                inVertices[i] = new Vector3(v.X,v.Y,v.Z);
            }
            mesh.vertices = inVertices;

            if (fbMesh.UvLength > 0)
            {
                Vector2[] uvs = new Vector2[fbMesh.UvLength];
                for (int i = 0; i < fbMesh.UvLength; i++)
                {
                    Vec2 v = fbMesh.GetUv(v2,i);
                    uvs[i] = new Vector2(v.X, v.Y);
                }
                mesh.uv = uvs;
            }
            if (fbMesh.Uv2Length > 0)
            {
                Vector2[] uvs = new Vector2[fbMesh.Uv2Length];
                for (int i = 0; i < fbMesh.Uv2Length; i++)
                {
                    Vec2 v = fbMesh.GetUv2(v2, i);
                    uvs[i] = new Vector2(v.X, v.Y);
                }
                mesh.uv2 = uvs;
            }
            if (fbMesh.Uv3Length > 0)
            {
                Vector2[] uvs = new Vector2[fbMesh.Uv3Length];
                for (int i = 0; i < fbMesh.Uv3Length; i++)
                {
                    Vec2 v = fbMesh.GetUv3(v2, i);
                    uvs[i] = new Vector2(v.X, v.Y);
                }
                mesh.uv3 = uvs;
            }
            if (fbMesh.Uv4Length > 0)
            {
                Vector2[] uvs = new Vector2[fbMesh.Uv4Length];
                for (int i = 0; i < fbMesh.Uv4Length; i++)
                {
                    Vec2 v = fbMesh.GetUv4(v2, i);
                    uvs[i] = new Vector2(v.X, v.Y);
                }
                mesh.uv4 = uvs;
            }
            if (fbMesh.NormalsLength > 0)
            {
                Vector3[] normals = new Vector3[fbMesh.NormalsLength];
                for (int i = 0; i < fbMesh.NormalsLength; i++)
                {
                    Vec3 v = fbMesh.GetNormals(v3, i);
                    normals[i] = new Vector3(v.X, v.Y,v.Z);
                }
                mesh.normals = normals;
            }
            if (fbMesh.TangentsLength > 0)
            {
                Vector4[] tangents = new Vector4[fbMesh.TangentsLength];
                for (int i = 0; i < fbMesh.TangentsLength; i++)
                {
                    Vec4 v = fbMesh.GetTangents(v4, i);
                    tangents[i] = new Vector4(v.X, v.Y, v.Z,v.W);
                }
                mesh.tangents = tangents;
            }
            if (fbMesh.ColorsLength > 0)
            {
                Schema.Color cobj = new Schema.Color();
                UnityEngine.Color[] colors = new UnityEngine.Color[fbMesh.ColorsLength];
                for (int i = 0; i < fbMesh.ColorsLength; i++)
                {
                    Schema.Color c = fbMesh.GetColors(cobj, i);
                    colors[i] = new UnityEngine.Color(c.R,c.G,c.B,c.A);
                }
                mesh.colors = colors;
            }
            if (fbMesh.Colors32Length > 0)
            {
                Schema.Color32 cobj = new Schema.Color32();
                UnityEngine.Color32[] colors = new UnityEngine.Color32[fbMesh.Colors32Length];
                for (int i = 0; i < fbMesh.Colors32Length; i++)
                {
                    Schema.Color32 c = fbMesh.GetColors32(cobj, i);
                    colors[i] = new UnityEngine.Color32(c.R, c.G, c.B, c.A);
                }
                mesh.colors32 = colors;
            }
            if (fbMesh.BoneWeightsLength > 0)
            {
                Schema.BoneWeight bwobj = new Schema.BoneWeight();
                UnityEngine.BoneWeight[] boneWeights = new UnityEngine.BoneWeight[fbMesh.BoneWeightsLength];
                for (int i = 0; i < fbMesh.BoneWeightsLength; i++)
                {
                    Schema.BoneWeight _bw = fbMesh.GetBoneWeights(bwobj, i);
                    UnityEngine.BoneWeight bw = new UnityEngine.BoneWeight();
                    bw.boneIndex0 = _bw.BoneIndex0;
                    bw.boneIndex1 = _bw.BoneIndex1;
                    bw.boneIndex2 = _bw.BoneIndex2;
                    bw.boneIndex3 = _bw.BoneIndex3;
                    bw.weight0 = _bw.Weight0;
                    bw.weight1 = _bw.Weight1;
                    bw.weight2 = _bw.Weight2;
                    bw.weight3 = _bw.Weight3;
                    boneWeights[i] = bw;
                }
                mesh.boneWeights = boneWeights;
            }

            int[] triangles = new int[fbMesh.TrianglesLength];
            for(int i=0;i<fbMesh.TrianglesLength;i++){
                triangles[i] = fbMesh.GetTriangles(i);
            }
            mesh.triangles = triangles;
            
            if (fbMesh.SubmeshesLength > 1)
            {
                for (int i = 0; i < fbMesh.SubmeshesLength; i++)
                {
                    Schema.SubMesh sb = fbMesh.GetSubmeshes(i);
                    int[] indices = new int[sb.NumOfTriangles];
                    for (int j = 0; j < sb.NumOfTriangles; j++)
                    {
                        indices[j] = fbMesh.GetTriangles((int)sb.StartTriangle + j);
                    }
                    mesh.SetIndices(indices, (UnityEngine.MeshTopology)sb.MeshTopology, i);
                }
            }

            if (fbMesh.BindposesLength > 0)
            {
                Matrix16 matrixObj = new Matrix16();
                Matrix4x4[] matrices = new Matrix4x4[fbMesh.BindposesLength];
                for (int i = 0; i < fbMesh.BindposesLength; i++)
                {
                    Matrix16 _matrix = fbMesh.GetBindposes(matrixObj,i);
                    Matrix4x4 matrix = new Matrix4x4();
                    matrix.m00 = _matrix.M00;
                    matrix.m01 = _matrix.M01;
                    matrix.m02 = _matrix.M02;
                    matrix.m03 = _matrix.M03;
                    matrix.m10 = _matrix.M10;
                    matrix.m11 = _matrix.M11;
                    matrix.m12 = _matrix.M12;
                    matrix.m13 = _matrix.M13;
                    matrix.m20 = _matrix.M20;
                    matrix.m21 = _matrix.M21;
                    matrix.m22 = _matrix.M22;
                    matrix.m23 = _matrix.M23;
                    matrix.m30 = _matrix.M30;
                    matrix.m31 = _matrix.M31;
                    matrix.m32 = _matrix.M32;
                    matrix.m33 = _matrix.M33;
                    matrices[i] = matrix;
                }
                mesh.bindposes = matrices;
            }

            Bound _b = fbMesh.GetBound(new Bound());
            Vec3 _min = _b.GetMin(v3);
            Vector3 min = new Vector3(_min.X, _min.Y, _min.Z); 
            Vec3 _max = _b.GetMax(v3);
            Vector3 max = new Vector3(_max.X, _max.Y, _max.Z);
            Bounds b = new Bounds(min, max);
            mesh.bounds = b;

			string[] joints = new string[fbMesh.JointsLength];
            for (int i = 0; i < fbMesh.JointsLength; i++)
            {
                joints[i] = fbMesh.GetJoints(i);
            }

			return new ResourceObjectMesh(mesh,joints);
        }
    }
}
