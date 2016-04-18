// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class AnimationClip : Table {
  public static AnimationClip GetRootAsAnimationClip(ByteBuffer _bb) { return GetRootAsAnimationClip(_bb, new AnimationClip()); }
  public static AnimationClip GetRootAsAnimationClip(ByteBuffer _bb, AnimationClip obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AnimationClip __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public CurveBinding GetBindings(int j) { return GetBindings(new CurveBinding(), j); }
  public CurveBinding GetBindings(CurveBinding obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BindingsLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<AnimationClip> CreateAnimationClip(FlatBufferBuilder builder,
      VectorOffset bindingsOffset = default(VectorOffset)) {
    builder.StartObject(1);
    AnimationClip.AddBindings(builder, bindingsOffset);
    return AnimationClip.EndAnimationClip(builder);
  }

  public static void StartAnimationClip(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddBindings(FlatBufferBuilder builder, VectorOffset bindingsOffset) { builder.AddOffset(0, bindingsOffset.Value, 0); }
  public static VectorOffset CreateBindingsVector(FlatBufferBuilder builder, Offset<CurveBinding>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBindingsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<AnimationClip> EndAnimationClip(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AnimationClip>(o);
  }
};


}
