// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Transform : Struct {
  public Transform __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Vec3 Pos { get { return GetPos(new Vec3()); } }
  public Vec3 GetPos(Vec3 obj) { return obj.__init(bb_pos + 0, bb); }
  public Quat Rot { get { return GetRot(new Quat()); } }
  public Quat GetRot(Quat obj) { return obj.__init(bb_pos + 12, bb); }
  public Vec3 Scl { get { return GetScl(new Vec3()); } }
  public Vec3 GetScl(Vec3 obj) { return obj.__init(bb_pos + 28, bb); }

  public static Offset<Transform> CreateTransform(FlatBufferBuilder builder, float pos_X, float pos_Y, float pos_Z, float rot_X, float rot_Y, float rot_Z, float rot_W, float scl_X, float scl_Y, float scl_Z) {
    builder.Prep(4, 40);
    builder.Prep(4, 12);
    builder.PutFloat(scl_Z);
    builder.PutFloat(scl_Y);
    builder.PutFloat(scl_X);
    builder.Prep(4, 16);
    builder.PutFloat(rot_W);
    builder.PutFloat(rot_Z);
    builder.PutFloat(rot_Y);
    builder.PutFloat(rot_X);
    builder.Prep(4, 12);
    builder.PutFloat(pos_Z);
    builder.PutFloat(pos_Y);
    builder.PutFloat(pos_X);
    return new Offset<Transform>(builder.Offset);
  }
};


}
