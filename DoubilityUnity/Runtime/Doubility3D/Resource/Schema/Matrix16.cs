// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Matrix16 : Struct {
  public Matrix16 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float M00 { get { return bb.GetFloat(bb_pos + 0); } }
  public float M01 { get { return bb.GetFloat(bb_pos + 4); } }
  public float M02 { get { return bb.GetFloat(bb_pos + 8); } }
  public float M03 { get { return bb.GetFloat(bb_pos + 12); } }
  public float M10 { get { return bb.GetFloat(bb_pos + 16); } }
  public float M11 { get { return bb.GetFloat(bb_pos + 20); } }
  public float M12 { get { return bb.GetFloat(bb_pos + 24); } }
  public float M13 { get { return bb.GetFloat(bb_pos + 28); } }
  public float M20 { get { return bb.GetFloat(bb_pos + 32); } }
  public float M21 { get { return bb.GetFloat(bb_pos + 36); } }
  public float M22 { get { return bb.GetFloat(bb_pos + 40); } }
  public float M23 { get { return bb.GetFloat(bb_pos + 44); } }
  public float M30 { get { return bb.GetFloat(bb_pos + 48); } }
  public float M31 { get { return bb.GetFloat(bb_pos + 52); } }
  public float M32 { get { return bb.GetFloat(bb_pos + 56); } }
  public float M33 { get { return bb.GetFloat(bb_pos + 60); } }

  public static Offset<Matrix16> CreateMatrix16(FlatBufferBuilder builder, float M00, float M01, float M02, float M03, float M10, float M11, float M12, float M13, float M20, float M21, float M22, float M23, float M30, float M31, float M32, float M33) {
    builder.Prep(4, 64);
    builder.PutFloat(M33);
    builder.PutFloat(M32);
    builder.PutFloat(M31);
    builder.PutFloat(M30);
    builder.PutFloat(M23);
    builder.PutFloat(M22);
    builder.PutFloat(M21);
    builder.PutFloat(M20);
    builder.PutFloat(M13);
    builder.PutFloat(M12);
    builder.PutFloat(M11);
    builder.PutFloat(M10);
    builder.PutFloat(M03);
    builder.PutFloat(M02);
    builder.PutFloat(M01);
    builder.PutFloat(M00);
    return new Offset<Matrix16>(builder.Offset);
  }
};


}
