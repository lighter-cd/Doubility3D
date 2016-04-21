// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Mesh : Table {
  public static Mesh GetRootAsMesh(ByteBuffer _bb) { return GetRootAsMesh(_bb, new Mesh()); }
  public static Mesh GetRootAsMesh(ByteBuffer _bb, Mesh obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Mesh __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Doubility3D.Resource.Schema.Vec3 GetVertices(int j) { return GetVertices(new Doubility3D.Resource.Schema.Vec3(), j); }
  public Doubility3D.Resource.Schema.Vec3 GetVertices(Doubility3D.Resource.Schema.Vec3 obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__vector(o) + j * 12, bb) : null; }
  public int VerticesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Vec2 GetUv(int j) { return GetUv(new Doubility3D.Resource.Schema.Vec2(), j); }
  public Doubility3D.Resource.Schema.Vec2 GetUv(Doubility3D.Resource.Schema.Vec2 obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__vector(o) + j * 8, bb) : null; }
  public int UvLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Vec2 GetUv2(int j) { return GetUv2(new Doubility3D.Resource.Schema.Vec2(), j); }
  public Doubility3D.Resource.Schema.Vec2 GetUv2(Doubility3D.Resource.Schema.Vec2 obj, int j) { int o = __offset(8); return o != 0 ? obj.__init(__vector(o) + j * 8, bb) : null; }
  public int Uv2Length { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Vec2 GetUv3(int j) { return GetUv3(new Doubility3D.Resource.Schema.Vec2(), j); }
  public Doubility3D.Resource.Schema.Vec2 GetUv3(Doubility3D.Resource.Schema.Vec2 obj, int j) { int o = __offset(10); return o != 0 ? obj.__init(__vector(o) + j * 8, bb) : null; }
  public int Uv3Length { get { int o = __offset(10); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Vec2 GetUv4(int j) { return GetUv4(new Doubility3D.Resource.Schema.Vec2(), j); }
  public Doubility3D.Resource.Schema.Vec2 GetUv4(Doubility3D.Resource.Schema.Vec2 obj, int j) { int o = __offset(12); return o != 0 ? obj.__init(__vector(o) + j * 8, bb) : null; }
  public int Uv4Length { get { int o = __offset(12); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Vec3 GetNormals(int j) { return GetNormals(new Doubility3D.Resource.Schema.Vec3(), j); }
  public Doubility3D.Resource.Schema.Vec3 GetNormals(Doubility3D.Resource.Schema.Vec3 obj, int j) { int o = __offset(14); return o != 0 ? obj.__init(__vector(o) + j * 12, bb) : null; }
  public int NormalsLength { get { int o = __offset(14); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Vec4 GetTangents(int j) { return GetTangents(new Doubility3D.Resource.Schema.Vec4(), j); }
  public Doubility3D.Resource.Schema.Vec4 GetTangents(Doubility3D.Resource.Schema.Vec4 obj, int j) { int o = __offset(16); return o != 0 ? obj.__init(__vector(o) + j * 16, bb) : null; }
  public int TangentsLength { get { int o = __offset(16); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Color GetColors(int j) { return GetColors(new Doubility3D.Resource.Schema.Color(), j); }
  public Doubility3D.Resource.Schema.Color GetColors(Doubility3D.Resource.Schema.Color obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__vector(o) + j * 16, bb) : null; }
  public int ColorsLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Color32 GetColors32(int j) { return GetColors32(new Doubility3D.Resource.Schema.Color32(), j); }
  public Doubility3D.Resource.Schema.Color32 GetColors32(Doubility3D.Resource.Schema.Color32 obj, int j) { int o = __offset(20); return o != 0 ? obj.__init(__vector(o) + j * 4, bb) : null; }
  public int Colors32Length { get { int o = __offset(20); return o != 0 ? __vector_len(o) : 0; } }
  public SubMesh GetSubmeshes(int j) { return GetSubmeshes(new SubMesh(), j); }
  public SubMesh GetSubmeshes(SubMesh obj, int j) { int o = __offset(22); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SubmeshesLength { get { int o = __offset(22); return o != 0 ? __vector_len(o) : 0; } }
  public int GetTriangles(int j) { int o = __offset(24); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int TrianglesLength { get { int o = __offset(24); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetTrianglesBytes() { return __vector_as_arraysegment(24); }
  public string GetJoints(int j) { int o = __offset(26); return o != 0 ? __string(__vector(o) + j * 4) : null; }
  public int JointsLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Matrix16 GetBindposes(int j) { return GetBindposes(new Doubility3D.Resource.Schema.Matrix16(), j); }
  public Doubility3D.Resource.Schema.Matrix16 GetBindposes(Doubility3D.Resource.Schema.Matrix16 obj, int j) { int o = __offset(28); return o != 0 ? obj.__init(__vector(o) + j * 64, bb) : null; }
  public int BindposesLength { get { int o = __offset(28); return o != 0 ? __vector_len(o) : 0; } }
  public BoneWeight GetBoneWeights(int j) { return GetBoneWeights(new BoneWeight(), j); }
  public BoneWeight GetBoneWeights(BoneWeight obj, int j) { int o = __offset(30); return o != 0 ? obj.__init(__vector(o) + j * 32, bb) : null; }
  public int BoneWeightsLength { get { int o = __offset(30); return o != 0 ? __vector_len(o) : 0; } }
  public BlendShape GetBlendShapes(int j) { return GetBlendShapes(new BlendShape(), j); }
  public BlendShape GetBlendShapes(BlendShape obj, int j) { int o = __offset(32); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BlendShapesLength { get { int o = __offset(32); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Bound Bound { get { return GetBound(new Doubility3D.Resource.Schema.Bound()); } }
  public Doubility3D.Resource.Schema.Bound GetBound(Doubility3D.Resource.Schema.Bound obj) { int o = __offset(34); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartMesh(FlatBufferBuilder builder) { builder.StartObject(16); }
  public static void AddVertices(FlatBufferBuilder builder, VectorOffset verticesOffset) { builder.AddOffset(0, verticesOffset.Value, 0); }
  public static void StartVerticesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(12, numElems, 4); }
  public static void AddUv(FlatBufferBuilder builder, VectorOffset uvOffset) { builder.AddOffset(1, uvOffset.Value, 0); }
  public static void StartUvVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(8, numElems, 4); }
  public static void AddUv2(FlatBufferBuilder builder, VectorOffset uv2Offset) { builder.AddOffset(2, uv2Offset.Value, 0); }
  public static void StartUv2Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(8, numElems, 4); }
  public static void AddUv3(FlatBufferBuilder builder, VectorOffset uv3Offset) { builder.AddOffset(3, uv3Offset.Value, 0); }
  public static void StartUv3Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(8, numElems, 4); }
  public static void AddUv4(FlatBufferBuilder builder, VectorOffset uv4Offset) { builder.AddOffset(4, uv4Offset.Value, 0); }
  public static void StartUv4Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(8, numElems, 4); }
  public static void AddNormals(FlatBufferBuilder builder, VectorOffset normalsOffset) { builder.AddOffset(5, normalsOffset.Value, 0); }
  public static void StartNormalsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(12, numElems, 4); }
  public static void AddTangents(FlatBufferBuilder builder, VectorOffset tangentsOffset) { builder.AddOffset(6, tangentsOffset.Value, 0); }
  public static void StartTangentsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(16, numElems, 4); }
  public static void AddColors(FlatBufferBuilder builder, VectorOffset colorsOffset) { builder.AddOffset(7, colorsOffset.Value, 0); }
  public static void StartColorsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(16, numElems, 4); }
  public static void AddColors32(FlatBufferBuilder builder, VectorOffset colors32Offset) { builder.AddOffset(8, colors32Offset.Value, 0); }
  public static void StartColors32Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 1); }
  public static void AddSubmeshes(FlatBufferBuilder builder, VectorOffset submeshesOffset) { builder.AddOffset(9, submeshesOffset.Value, 0); }
  public static VectorOffset CreateSubmeshesVector(FlatBufferBuilder builder, Offset<SubMesh>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSubmeshesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTriangles(FlatBufferBuilder builder, VectorOffset trianglesOffset) { builder.AddOffset(10, trianglesOffset.Value, 0); }
  public static VectorOffset CreateTrianglesVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartTrianglesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddJoints(FlatBufferBuilder builder, VectorOffset jointsOffset) { builder.AddOffset(11, jointsOffset.Value, 0); }
  public static VectorOffset CreateJointsVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartJointsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBindposes(FlatBufferBuilder builder, VectorOffset bindposesOffset) { builder.AddOffset(12, bindposesOffset.Value, 0); }
  public static void StartBindposesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(64, numElems, 4); }
  public static void AddBoneWeights(FlatBufferBuilder builder, VectorOffset boneWeightsOffset) { builder.AddOffset(13, boneWeightsOffset.Value, 0); }
  public static void StartBoneWeightsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(32, numElems, 4); }
  public static void AddBlendShapes(FlatBufferBuilder builder, VectorOffset blendShapesOffset) { builder.AddOffset(14, blendShapesOffset.Value, 0); }
  public static VectorOffset CreateBlendShapesVector(FlatBufferBuilder builder, Offset<BlendShape>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBlendShapesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddBound(FlatBufferBuilder builder, Offset<Doubility3D.Resource.Schema.Bound> boundOffset) { builder.AddStruct(15, boundOffset.Value, 0); }
  public static Offset<Mesh> EndMesh(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    builder.Required(o, 4);  // vertices
    builder.Required(o, 24);  // triangles
    builder.Required(o, 34);  // bound
    return new Offset<Mesh>(o);
  }
};


}
