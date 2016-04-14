// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class BoneWeight : Struct {
  public BoneWeight __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int BoneIndex0 { get { return bb.GetInt(bb_pos + 0); } }
  public int BoneIndex1 { get { return bb.GetInt(bb_pos + 4); } }
  public int BoneIndex2 { get { return bb.GetInt(bb_pos + 8); } }
  public int BoneIndex3 { get { return bb.GetInt(bb_pos + 12); } }
  public float Weight0 { get { return bb.GetFloat(bb_pos + 16); } }
  public float Weight1 { get { return bb.GetFloat(bb_pos + 20); } }
  public float Weight2 { get { return bb.GetFloat(bb_pos + 24); } }
  public float Weight3 { get { return bb.GetFloat(bb_pos + 28); } }

  public static Offset<BoneWeight> CreateBoneWeight(FlatBufferBuilder builder, int BoneIndex0, int BoneIndex1, int BoneIndex2, int BoneIndex3, float Weight0, float Weight1, float Weight2, float Weight3) {
    builder.Prep(4, 32);
    builder.PutFloat(Weight3);
    builder.PutFloat(Weight2);
    builder.PutFloat(Weight1);
    builder.PutFloat(Weight0);
    builder.PutInt(BoneIndex3);
    builder.PutInt(BoneIndex2);
    builder.PutInt(BoneIndex1);
    builder.PutInt(BoneIndex0);
    return new Offset<BoneWeight>(builder.Offset);
  }
};


}
