// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class AnimationCurve : Table {
  public static AnimationCurve GetRootAsAnimationCurve(ByteBuffer _bb) { return GetRootAsAnimationCurve(_bb, new AnimationCurve()); }
  public static AnimationCurve GetRootAsAnimationCurve(ByteBuffer _bb, AnimationCurve obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AnimationCurve __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public KeyFrame GetKeyFrames(int j) { return GetKeyFrames(new KeyFrame(), j); }
  public KeyFrame GetKeyFrames(KeyFrame obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__vector(o) + j * 20, bb) : null; }
  public int KeyFramesLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public WrapMode PostWrapMode { get { int o = __offset(6); return o != 0 ? (WrapMode)bb.GetSbyte(o + bb_pos) : WrapMode.Default; } }
  public WrapMode PreWrapMode { get { int o = __offset(8); return o != 0 ? (WrapMode)bb.GetSbyte(o + bb_pos) : WrapMode.Default; } }

  public static Offset<AnimationCurve> CreateAnimationCurve(FlatBufferBuilder builder,
      VectorOffset keyFramesOffset = default(VectorOffset),
      WrapMode postWrapMode = WrapMode.Default,
      WrapMode preWrapMode = WrapMode.Default) {
    builder.StartObject(3);
    AnimationCurve.AddKeyFrames(builder, keyFramesOffset);
    AnimationCurve.AddPreWrapMode(builder, preWrapMode);
    AnimationCurve.AddPostWrapMode(builder, postWrapMode);
    return AnimationCurve.EndAnimationCurve(builder);
  }

  public static void StartAnimationCurve(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddKeyFrames(FlatBufferBuilder builder, VectorOffset keyFramesOffset) { builder.AddOffset(0, keyFramesOffset.Value, 0); }
  public static void StartKeyFramesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(20, numElems, 4); }
  public static void AddPostWrapMode(FlatBufferBuilder builder, WrapMode postWrapMode) { builder.AddSbyte(1, (sbyte)postWrapMode, 0); }
  public static void AddPreWrapMode(FlatBufferBuilder builder, WrapMode preWrapMode) { builder.AddSbyte(2, (sbyte)preWrapMode, 0); }
  public static Offset<AnimationCurve> EndAnimationCurve(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AnimationCurve>(o);
  }
};


}
