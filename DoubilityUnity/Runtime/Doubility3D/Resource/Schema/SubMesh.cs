// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class SubMesh : Table {
  public static SubMesh GetRootAsSubMesh(ByteBuffer _bb) { return GetRootAsSubMesh(_bb, new SubMesh()); }
  public static SubMesh GetRootAsSubMesh(ByteBuffer _bb, SubMesh obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SubMesh __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public uint StartTriangle { get { int o = __offset(4); return o != 0 ? bb.GetUint(o + bb_pos) : (uint)0; } }
  public uint NumOfTriangles { get { int o = __offset(6); return o != 0 ? bb.GetUint(o + bb_pos) : (uint)0; } }
  public MeshTopology MeshTopology { get { int o = __offset(8); return o != 0 ? (MeshTopology)bb.GetSbyte(o + bb_pos) : MeshTopology.Triangles; } }

  public static Offset<SubMesh> CreateSubMesh(FlatBufferBuilder builder,
      uint startTriangle = 0,
      uint numOfTriangles = 0,
      MeshTopology meshTopology = MeshTopology.Triangles) {
    builder.StartObject(3);
    SubMesh.AddNumOfTriangles(builder, numOfTriangles);
    SubMesh.AddStartTriangle(builder, startTriangle);
    SubMesh.AddMeshTopology(builder, meshTopology);
    return SubMesh.EndSubMesh(builder);
  }

  public static void StartSubMesh(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddStartTriangle(FlatBufferBuilder builder, uint startTriangle) { builder.AddUint(0, startTriangle, 0); }
  public static void AddNumOfTriangles(FlatBufferBuilder builder, uint numOfTriangles) { builder.AddUint(1, numOfTriangles, 0); }
  public static void AddMeshTopology(FlatBufferBuilder builder, MeshTopology meshTopology) { builder.AddSbyte(2, (sbyte)meshTopology, 0); }
  public static Offset<SubMesh> EndSubMesh(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SubMesh>(o);
  }
};


}
