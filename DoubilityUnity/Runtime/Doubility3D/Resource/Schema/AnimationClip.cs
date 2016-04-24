// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class AnimationClip : Table {
  public static AnimationClip GetRootAsAnimationClip(ByteBuffer _bb) { return GetRootAsAnimationClip(_bb, new AnimationClip()); }
  public static AnimationClip GetRootAsAnimationClip(ByteBuffer _bb, AnimationClip obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AnimationClip __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float FrameRate { get { int o = __offset(4); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public WrapMode WrapMode { get { int o = __offset(6); return o != 0 ? (WrapMode)bb.GetSbyte(o + bb_pos) : WrapMode.Default; } }
  public CurveBinding GetBindings(int j) { return GetBindings(new CurveBinding(), j); }
  public CurveBinding GetBindings(CurveBinding obj, int j) { int o = __offset(8); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BindingsLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<AnimationClip> CreateAnimationClip(FlatBufferBuilder builder,
      float frameRate = 0,
      WrapMode wrapMode = WrapMode.Default,
      VectorOffset bindingsOffset = default(VectorOffset)) {
    builder.StartObject(3);
    AnimationClip.AddBindings(builder, bindingsOffset);
    AnimationClip.AddFrameRate(builder, frameRate);
    AnimationClip.AddWrapMode(builder, wrapMode);
    return AnimationClip.EndAnimationClip(builder);
  }

  public static void StartAnimationClip(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddFrameRate(FlatBufferBuilder builder, float frameRate) { builder.AddFloat(0, frameRate, 0); }
  public static void AddWrapMode(FlatBufferBuilder builder, WrapMode wrapMode) { builder.AddSbyte(1, (sbyte)wrapMode, 0); }
  public static void AddBindings(FlatBufferBuilder builder, VectorOffset bindingsOffset) { builder.AddOffset(2, bindingsOffset.Value, 0); }
  public static VectorOffset CreateBindingsVector(FlatBufferBuilder builder, Offset<CurveBinding>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBindingsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<AnimationClip> EndAnimationClip(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AnimationClip>(o);
  }
};


}
