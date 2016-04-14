// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Skeletons : Table {
  public static Skeletons GetRootAsSkeletons(ByteBuffer _bb) { return GetRootAsSkeletons(_bb, new Skeletons()); }
  public static Skeletons GetRootAsSkeletons(ByteBuffer _bb, Skeletons obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Skeletons __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Joint GetJoints(int j) { return GetJoints(new Joint(), j); }
  public Joint GetJoints(Joint obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int JointsLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Skeletons> CreateSkeletons(FlatBufferBuilder builder,
      VectorOffset jointsOffset = default(VectorOffset)) {
    builder.StartObject(1);
    Skeletons.AddJoints(builder, jointsOffset);
    return Skeletons.EndSkeletons(builder);
  }

  public static void StartSkeletons(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddJoints(FlatBufferBuilder builder, VectorOffset jointsOffset) { builder.AddOffset(0, jointsOffset.Value, 0); }
  public static VectorOffset CreateJointsVector(FlatBufferBuilder builder, Offset<Joint>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartJointsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Skeletons> EndSkeletons(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Skeletons>(o);
  }
};


}
