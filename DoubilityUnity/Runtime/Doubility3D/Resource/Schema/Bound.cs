// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Bound : Struct {
  public Bound __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Vec3 Min { get { return GetMin(new Vec3()); } }
  public Vec3 GetMin(Vec3 obj) { return obj.__init(bb_pos + 0, bb); }
  public Vec3 Max { get { return GetMax(new Vec3()); } }
  public Vec3 GetMax(Vec3 obj) { return obj.__init(bb_pos + 12, bb); }

  public static Offset<Bound> CreateBound(FlatBufferBuilder builder, float min_X, float min_Y, float min_Z, float max_X, float max_Y, float max_Z) {
    builder.Prep(4, 24);
    builder.Prep(4, 12);
    builder.PutFloat(max_Z);
    builder.PutFloat(max_Y);
    builder.PutFloat(max_X);
    builder.Prep(4, 12);
    builder.PutFloat(min_Z);
    builder.PutFloat(min_Y);
    builder.PutFloat(min_X);
    return new Offset<Bound>(builder.Offset);
  }
};


}
