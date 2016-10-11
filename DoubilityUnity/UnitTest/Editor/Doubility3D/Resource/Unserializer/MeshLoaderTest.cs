using UnityEngine;
using NUnit.Framework;

using FlatBuffers;
using Doubility3D.Resource.Unserializing;
using Doubility3D.Resource.Schema;
using Doubility3D.Resource.ResourceObj;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using System.Collections.Generic;

using UnitTest.Doubility3D;

namespace UnitTest.Doubility3D.Resource.Unserializing
{
    [TestFixture]
    public class MeshLoaderTest
    {
        Schema.Mesh mesh;
		ResourceObjectMesh _resultMesh;

        [SetUp]
        public void Init()
        {
            Context context = Context.Unknown;
            ByteBuffer bb = TestData.LoadResource("armor@Suit_Metal_Dragon_Male.doub", out context);
            Assert.IsNotNull(bb);
            Assert.AreNotEqual(context, Context.Unknown);
            mesh = Schema.Mesh.GetRootAsMesh(bb);

			MeshUnserializer unserializer = UnserializerFactory.Instance.Create (context) as MeshUnserializer;
			_resultMesh = unserializer.Parse(bb) as ResourceObjectMesh;
        }

        [TearDown]
        public void Cleanup()
        {
			_resultMesh.Dispose();
			_resultMesh = null;
            mesh = null;
        }

        [Test]
        public void VertexComponents()
        {
			UnityEngine.Mesh resultMesh = _resultMesh.Unity3dObject as UnityEngine.Mesh;

            Assert.IsTrue(mesh.VerticesLength == resultMesh.vertexCount);

            // vertices
            for (int i = 0; i < mesh.VerticesLength; i++)
            {
                Vec3 v1 = mesh.GetVertices(i);
                Vector3 v2 = resultMesh.vertices[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
                Assert.AreEqual(v1.Z, v2.z);
            }

            // uv
            Assert.IsTrue(mesh.UvLength == resultMesh.uv.Length);
            for (int i = 0; i < mesh.UvLength; i++)
            {
                Vec2 v1 = mesh.GetUv(i);
                Vector2 v2 = resultMesh.uv[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
            }

            // uv2
            Assert.IsTrue(mesh.Uv2Length == resultMesh.uv2.Length);
            for (int i = 0; i < mesh.Uv2Length; i++)
            {
                Vec2 v1 = mesh.GetUv2(i);
                Vector2 v2 = resultMesh.uv2[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
            }

            // uv3
            Assert.IsTrue(mesh.Uv3Length == resultMesh.uv3.Length);
            for (int i = 0; i < mesh.Uv3Length; i++)
            {
                Vec2 v1 = mesh.GetUv3(i);
                Vector2 v2 = resultMesh.uv3[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
            }

            // uv4
            Assert.IsTrue(mesh.Uv4Length == resultMesh.uv4.Length);
            for (int i = 0; i < mesh.Uv4Length; i++)
            {
                Vec2 v1 = mesh.GetUv4(i);
                Vector2 v2 = resultMesh.uv4[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
            }

            // normals
            Assert.IsTrue(mesh.NormalsLength == resultMesh.normals.Length);
            for (int i = 0; i < mesh.NormalsLength; i++)
            {
                Vec3 v1 = mesh.GetNormals(i);
                Vector3 v2 = resultMesh.normals[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
                Assert.AreEqual(v1.Z, v2.z);
            }

            // tangents
            Assert.IsTrue(mesh.TangentsLength == resultMesh.tangents.Length);
            for (int i = 0; i < mesh.TangentsLength; i++)
            {
                Vec4 v1 = mesh.GetTangents(i);
                Vector4 v2 = resultMesh.tangents[i];
                Assert.AreEqual(v1.X, v2.x);
                Assert.AreEqual(v1.Y, v2.y);
                Assert.AreEqual(v1.Z, v2.z);
                Assert.AreEqual(v1.W, v2.w);
            }

            // colors
            Assert.IsTrue(mesh.ColorsLength == resultMesh.colors.Length);
            for (int i = 0; i < mesh.ColorsLength; i++)
            {
                Schema.Color c1 = mesh.GetColors(i);
                UnityEngine.Color c2 = resultMesh.colors[i];
                Assert.AreEqual(c1.A, c2.a);
                Assert.AreEqual(c1.B, c2.b);
                Assert.AreEqual(c1.G, c2.g);
                Assert.AreEqual(c1.R, c2.r);
            }

            // colors32
            Assert.IsTrue(mesh.Colors32Length == resultMesh.colors32.Length);
            for (int i = 0; i < mesh.Colors32Length; i++)
            {
                Schema.Color32 c1 = mesh.GetColors32(i);
                UnityEngine.Color32 c2 = resultMesh.colors32[i];
                Assert.AreEqual(c1.A, c2.a);
                Assert.AreEqual(c1.B, c2.b);
                Assert.AreEqual(c1.G, c2.g);
                Assert.AreEqual(c1.R, c2.r);
            }

            // BoneWeigths
            Assert.IsTrue(mesh.BoneWeightsLength == resultMesh.boneWeights.Length);
            for (int i = 0; i < mesh.BoneWeightsLength; i++)
            {
                Schema.BoneWeight bw1 = mesh.GetBoneWeights(i);
                UnityEngine.BoneWeight bw2 = resultMesh.boneWeights[i];
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
            Assert.IsTrue(mesh.TrianglesLength == resultMesh.triangles.Length);
            for (int i = 0; i < mesh.TrianglesLength; i++)
            {
                int t1 = mesh.GetTriangles(i);
                int t2 = resultMesh.triangles[i];
                Assert.AreEqual(t1, t2);
            }
        }

        [Test]
        public void Submesh()
        {
			UnityEngine.Mesh resultMesh = _resultMesh.Unity3dObject as UnityEngine.Mesh;

            Assert.IsTrue(mesh.SubmeshesLength == resultMesh.subMeshCount);
            for (int i = 0; i < mesh.SubmeshesLength; i++)
            {
                Schema.SubMesh sb = mesh.GetSubmeshes(i);
                int[] indices = resultMesh.GetIndices(i);

                Assert.AreEqual(sb.MeshTopology, (Schema.MeshTopology)resultMesh.GetTopology(i));
                Assert.AreEqual(sb.NumOfTriangles, indices.Length);

                for (int j = 0; j < sb.NumOfTriangles; j++)
                {
                    int t1 = mesh.GetTriangles((int)sb.StartTriangle + j);
                    Assert.AreEqual(t1, indices[j]);
                }
            }
        }

        [Test]
        public void Joints()
        {
			Assert.AreEqual(_resultMesh.joints.Length, mesh.JointsLength);
            for (int i = 0; i < mesh.JointsLength; i++)
            {
				Assert.AreEqual(_resultMesh.joints[i], mesh.GetJoints(i));
            }
        }

        [Test]
        public void BindPoses()
        {
			UnityEngine.Mesh resultMesh = _resultMesh.Unity3dObject as UnityEngine.Mesh;
            Assert.AreEqual(resultMesh.bindposes.Length, mesh.BindposesLength);
            for (int i = 0; i < mesh.BindposesLength; i++)
            {
                Matrix4x4 resultMatrix = resultMesh.bindposes[i];
                Matrix16 matrix = mesh.GetBindposes(i);

                Assert.AreEqual(resultMatrix.m00, matrix.M00);
                Assert.AreEqual(resultMatrix.m01, matrix.M01);
                Assert.AreEqual(resultMatrix.m02, matrix.M02);
                Assert.AreEqual(resultMatrix.m03, matrix.M03);

                Assert.AreEqual(resultMatrix.m10, matrix.M10);
                Assert.AreEqual(resultMatrix.m11, matrix.M11);
                Assert.AreEqual(resultMatrix.m12, matrix.M12);
                Assert.AreEqual(resultMatrix.m13, matrix.M13);

                Assert.AreEqual(resultMatrix.m20, matrix.M20);
                Assert.AreEqual(resultMatrix.m21, matrix.M21);
                Assert.AreEqual(resultMatrix.m22, matrix.M22);
                Assert.AreEqual(resultMatrix.m23, matrix.M23);

                Assert.AreEqual(resultMatrix.m30, matrix.M30);
                Assert.AreEqual(resultMatrix.m31, matrix.M31);
                Assert.AreEqual(resultMatrix.m32, matrix.M32);
                Assert.AreEqual(resultMatrix.m33, matrix.M33);
            }
        }
    }
}
