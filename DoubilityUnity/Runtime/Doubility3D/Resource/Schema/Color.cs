// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Color : Struct {
  public Color __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float A { get { return bb.GetFloat(bb_pos + 0); } }
  public float B { get { return bb.GetFloat(bb_pos + 4); } }
  public float G { get { return bb.GetFloat(bb_pos + 8); } }
  public float R { get { return bb.GetFloat(bb_pos + 12); } }

  public static Offset<Color> CreateColor(FlatBufferBuilder builder, float A, float B, float G, float R) {
    builder.Prep(4, 16);
    builder.PutFloat(R);
    builder.PutFloat(G);
    builder.PutFloat(B);
    builder.PutFloat(A);
    return new Offset<Color>(builder.Offset);
  }
};


}
