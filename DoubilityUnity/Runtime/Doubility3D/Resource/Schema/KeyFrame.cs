// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class KeyFrame : Struct {
  public KeyFrame __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float InTangent { get { return bb.GetFloat(bb_pos + 0); } }
  public float OutTangent { get { return bb.GetFloat(bb_pos + 4); } }
  public int TangentMode { get { return bb.GetInt(bb_pos + 8); } }
  public float Time { get { return bb.GetFloat(bb_pos + 12); } }
  public float Value { get { return bb.GetFloat(bb_pos + 16); } }

  public static Offset<KeyFrame> CreateKeyFrame(FlatBufferBuilder builder, float InTangent, float OutTangent, int TangentMode, float Time, float Value) {
    builder.Prep(4, 20);
    builder.PutFloat(Value);
    builder.PutFloat(Time);
    builder.PutInt(TangentMode);
    builder.PutFloat(OutTangent);
    builder.PutFloat(InTangent);
    return new Offset<KeyFrame>(builder.Offset);
  }
};


}
