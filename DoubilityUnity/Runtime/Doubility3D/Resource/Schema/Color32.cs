// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Color32 : Struct {
  public Color32 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public byte A { get { return bb.Get(bb_pos + 0); } }
  public byte B { get { return bb.Get(bb_pos + 1); } }
  public byte G { get { return bb.Get(bb_pos + 2); } }
  public byte R { get { return bb.Get(bb_pos + 3); } }

  public static Offset<Color32> CreateColor32(FlatBufferBuilder builder, byte A, byte B, byte G, byte R) {
    builder.Prep(1, 4);
    builder.PutByte(R);
    builder.PutByte(G);
    builder.PutByte(B);
    builder.PutByte(A);
    return new Offset<Color32>(builder.Offset);
  }
};


}
