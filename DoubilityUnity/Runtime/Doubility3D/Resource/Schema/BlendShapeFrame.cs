// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class BlendShapeFrame : Table {
  public static BlendShapeFrame GetRootAsBlendShapeFrame(ByteBuffer _bb) { return GetRootAsBlendShapeFrame(_bb, new BlendShapeFrame()); }
  public static BlendShapeFrame GetRootAsBlendShapeFrame(ByteBuffer _bb, BlendShapeFrame obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BlendShapeFrame __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float Weight { get { int o = __offset(4); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public Doubility3D.Resource.Schema.Vec3 GetDeltaVertices(int j) { return GetDeltaVertices(new Doubility3D.Resource.Schema.Vec3(), j); }
  public Doubility3D.Resource.Schema.Vec3 GetDeltaVertices(Doubility3D.Resource.Schema.Vec3 obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__vector(o) + j * 12, bb) : null; }
  public int DeltaVerticesLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Vec3 GetDeltaNormals(int j) { return GetDeltaNormals(new Doubility3D.Resource.Schema.Vec3(), j); }
  public Doubility3D.Resource.Schema.Vec3 GetDeltaNormals(Doubility3D.Resource.Schema.Vec3 obj, int j) { int o = __offset(8); return o != 0 ? obj.__init(__vector(o) + j * 12, bb) : null; }
  public int DeltaNormalsLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public Doubility3D.Resource.Schema.Vec3 GetDeltaTangents(int j) { return GetDeltaTangents(new Doubility3D.Resource.Schema.Vec3(), j); }
  public Doubility3D.Resource.Schema.Vec3 GetDeltaTangents(Doubility3D.Resource.Schema.Vec3 obj, int j) { int o = __offset(10); return o != 0 ? obj.__init(__vector(o) + j * 12, bb) : null; }
  public int DeltaTangentsLength { get { int o = __offset(10); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<BlendShapeFrame> CreateBlendShapeFrame(FlatBufferBuilder builder,
      float weight = 0,
      VectorOffset deltaVerticesOffset = default(VectorOffset),
      VectorOffset deltaNormalsOffset = default(VectorOffset),
      VectorOffset deltaTangentsOffset = default(VectorOffset)) {
    builder.StartObject(4);
    BlendShapeFrame.AddDeltaTangents(builder, deltaTangentsOffset);
    BlendShapeFrame.AddDeltaNormals(builder, deltaNormalsOffset);
    BlendShapeFrame.AddDeltaVertices(builder, deltaVerticesOffset);
    BlendShapeFrame.AddWeight(builder, weight);
    return BlendShapeFrame.EndBlendShapeFrame(builder);
  }

  public static void StartBlendShapeFrame(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddWeight(FlatBufferBuilder builder, float weight) { builder.AddFloat(0, weight, 0); }
  public static void AddDeltaVertices(FlatBufferBuilder builder, VectorOffset deltaVerticesOffset) { builder.AddOffset(1, deltaVerticesOffset.Value, 0); }
  public static void StartDeltaVerticesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(12, numElems, 4); }
  public static void AddDeltaNormals(FlatBufferBuilder builder, VectorOffset deltaNormalsOffset) { builder.AddOffset(2, deltaNormalsOffset.Value, 0); }
  public static void StartDeltaNormalsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(12, numElems, 4); }
  public static void AddDeltaTangents(FlatBufferBuilder builder, VectorOffset deltaTangentsOffset) { builder.AddOffset(3, deltaTangentsOffset.Value, 0); }
  public static void StartDeltaTangentsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(12, numElems, 4); }
  public static Offset<BlendShapeFrame> EndBlendShapeFrame(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BlendShapeFrame>(o);
  }
};


}
