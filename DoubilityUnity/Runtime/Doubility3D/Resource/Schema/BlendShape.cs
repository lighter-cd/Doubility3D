// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class BlendShape : Table {
  public static BlendShape GetRootAsBlendShape(ByteBuffer _bb) { return GetRootAsBlendShape(_bb, new BlendShape()); }
  public static BlendShape GetRootAsBlendShape(ByteBuffer _bb, BlendShape obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public BlendShape __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(4); }
  public BlendShapeFrame GetFrames(int j) { return GetFrames(new BlendShapeFrame(), j); }
  public BlendShapeFrame GetFrames(BlendShapeFrame obj, int j) { int o = __offset(6); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int FramesLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<BlendShape> CreateBlendShape(FlatBufferBuilder builder,
      StringOffset nameOffset = default(StringOffset),
      VectorOffset framesOffset = default(VectorOffset)) {
    builder.StartObject(2);
    BlendShape.AddFrames(builder, framesOffset);
    BlendShape.AddName(builder, nameOffset);
    return BlendShape.EndBlendShape(builder);
  }

  public static void StartBlendShape(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddFrames(FlatBufferBuilder builder, VectorOffset framesOffset) { builder.AddOffset(1, framesOffset.Value, 0); }
  public static VectorOffset CreateFramesVector(FlatBufferBuilder builder, Offset<BlendShapeFrame>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartFramesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<BlendShape> EndBlendShape(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<BlendShape>(o);
  }
};


}
