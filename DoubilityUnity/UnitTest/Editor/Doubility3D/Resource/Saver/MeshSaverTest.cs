using UnityEngine;
using NUnit.Framework;

using FlatBuffers;
using Doubility3D.Resource.Saver;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using System.Collections.Generic;
using System;

namespace UnitTest.Doubility3D.Resource.Saver
{
    [TestFixture]
    public class MeshSaverTest
    {
        const string testData_path = "Assets/Doubility3D/UnitTest/.TestData/";

        AssetBundle ab;
        GameObject go;
        
        Schema.Mesh mesh;
        SkinnedMeshRenderer smr;

        [SetUp]
        public void Init()
        {
            string testData_folder = testData_path + PlatformPath.GetPath(Application.platform);

            ab = AssetBundle.LoadFromFile(testData_folder + "/skeletontest.bundle");
            UnityEngine.Object obj = ab.LoadAsset("Assets/ArtWork/Character/Suit_Metal_Dragon_Male/Suit_Metal_Dragon_Male.FBX");
            Assert.IsInstanceOf<GameObject>(obj);
            go = obj as GameObject;

            smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
            ByteBuffer result = MeshSaver.Save(smr.sharedMesh,smr.bones);
            mesh = Schema.Mesh.GetRootAsMesh(result);
        }

        [TearDown]
        public void Cleanup()
        {
            ab.Unload(true);
            go = null;
            ab = null;
            mesh = null;
            smr = null;
        }

        [Test]
        public void VertexComponents()
        {
            UnityEngine.Mesh originMesh = smr.sharedMesh;
            
            Assert.IsTrue(mesh.VerticesLength == originMesh.vertexCount);

            // vertices
            for(int i=0;i<mesh.VerticesLength;i++)
            {
                Vec3 v1 = mesh.GetVertices(i);
                Vector3 v2 = originMesh.vertices[i];
                Assert.AreEqual(v1.X,v2.x);
                Assert.AreEqual(v1.Y,v2.y);
                Assert.AreEqual(v1.Z,v2.z);
            }

            // uv
            Assert.IsTrue(mesh.UvLength == originMesh.uv.Length);
            for(int i=0;i<mesh.UvLength;i++)
            {
                Vec2 v1 = mesh.GetUv(i);
                Vector2 v2 = originMesh.uv[i];
                Assert.AreEqual(v1.X,v2.x);
                Assert.AreEqual(v1.Y,v2.y);
            }

            // uv2
            Assert.IsTrue(mesh.Uv2Length == originMesh.uv2.Length);
            for(int i=0;i<mesh.Uv2Length;i++)
            {
                Vec2 v1 = mesh.GetUv2(i);
                Vector2 v2 = originMesh.uv2[i];
                Assert.AreEqual(v1.X,v2.x);
                Assert.AreEqual(v1.Y,v2.y);
            }

            // uv3
            Assert.IsTrue(mesh.Uv3Length == originMesh.uv3.Length);
            for (int i = 0; i < mesh.Uv3Length; i++)
            {
                Vec2 v1 = mesh.GetUv3(i);
                Vector2 v2 = originMesh.uv3[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
            }

            // uv4
            Assert.IsTrue(mesh.Uv4Length == originMesh.uv4.Length);
            for (int i = 0; i < mesh.Uv4Length; i++)
            {
                Vec2 v1 = mesh.GetUv4(i);
                Vector2 v2 = originMesh.uv4[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
            }

            // normals
            Assert.IsTrue(mesh.NormalsLength == originMesh.normals.Length);
            for (int i = 0; i < mesh.NormalsLength; i++)
            {
                Vec3 v1 = mesh.GetNormals(i);
                Vector3 v2 = originMesh.normals[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
                Assert.AreEqual(v1.Z, v2.z);
            }

            // tangents
            Assert.IsTrue(mesh.TangentsLength == originMesh.tangents.Length);
            for (int i = 0; i < mesh.TangentsLength; i++)
            {
                Vec3 v1 = mesh.GetTangents(i);
                Vector3 v2 = originMesh.tangents[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
                Assert.AreEqual(v1.Z, v2.z);
            }

            // colors
            Assert.IsTrue(mesh.ColorsLength == originMesh.colors.Length);
            for (int i = 0; i < mesh.ColorsLength; i++)
            {
                Schema.Color c1 = mesh.GetColors(i);
                UnityEngine.Color c2 = originMesh.colors[i];
                Assert.AreEqual(c1.A, c2.a);
                Assert.AreEqual(c1.B, c2.b);
                Assert.AreEqual(c1.G, c2.g);
                Assert.AreEqual(c1.R, c2.r);
            }

            // colors32
            Assert.IsTrue(mesh.Colors32Length == originMesh.colors32.Length);
            for (int i = 0; i < mesh.Colors32Length; i++)
            {
                Schema.Color32 c1 = mesh.GetColors32(i);
                UnityEngine.Color32 c2 = originMesh.colors32[i];
                Assert.AreEqual(c1.A, c2.a);
                Assert.AreEqual(c1.B, c2.b);
                Assert.AreEqual(c1.G, c2.g);
                Assert.AreEqual(c1.R, c2.r);
            }

            // BoneWeigths
            Assert.IsTrue(mesh.BoneWeightsLength == originMesh.boneWeights.Length);
            for (int i = 0; i < mesh.BoneWeightsLength; i++)
            {
                Schema.BoneWeight bw1 = mesh.GetBoneWeights(i);
                UnityEngine.BoneWeight bw2 = originMesh.boneWeights[i];
                Assert.AreEqual(bw1.BoneIndex0, bw2.boneIndex0);
                Assert.AreEqual(bw1.BoneIndex1, bw2.boneIndex1);
                Assert.AreEqual(bw1.BoneIndex2, bw2.boneIndex2);
                Assert.AreEqual(bw1.BoneIndex3, bw2.boneIndex3);
                Assert.AreEqual(bw1.Weight0, bw2.weight0);
                Assert.AreEqual(bw1.Weight1, bw2.weight1);
                Assert.AreEqual(bw1.Weight2, bw2.weight2);
                Assert.AreEqual(bw1.Weight3, bw2.weight3);
            }

            // 三角面列表
            Assert.IsTrue(mesh.TrianglesLength == originMesh.triangles.Length);
            for (int i = 0; i < mesh.TrianglesLength; i++)
            {
                int t1 = mesh.GetTriangles(i);
                int t2 = originMesh.triangles[i];
                Assert.AreEqual(t1, t2);
            }
        }
        [Test]
        public void Submesh()
        {
            UnityEngine.Mesh originMesh = smr.sharedMesh;

            Assert.IsTrue(mesh.SubmeshesLength == originMesh.subMeshCount);
            for (int i = 0; i < mesh.SubmeshesLength; i++)
            {

            }
        }
    }
}