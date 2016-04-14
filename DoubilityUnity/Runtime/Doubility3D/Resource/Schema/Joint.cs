// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Joint : Table {
  public static Joint GetRootAsJoint(ByteBuffer _bb) { return GetRootAsJoint(_bb, new Joint()); }
  public static Joint GetRootAsJoint(ByteBuffer _bb, Joint obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Joint __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Names { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNamesBytes() { return __vector_as_arraysegment(4); }
  public Doubility3D.Resource.Schema.Transform Transform { get { return GetTransform(new Doubility3D.Resource.Schema.Transform()); } }
  public Doubility3D.Resource.Schema.Transform GetTransform(Doubility3D.Resource.Schema.Transform obj) { int o = __offset(6); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public int Parent { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static void StartJoint(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddNames(FlatBufferBuilder builder, StringOffset namesOffset) { builder.AddOffset(0, namesOffset.Value, 0); }
  public static void AddTransform(FlatBufferBuilder builder, Offset<Doubility3D.Resource.Schema.Transform> transformOffset) { builder.AddStruct(1, transformOffset.Value, 0); }
  public static void AddParent(FlatBufferBuilder builder, int parent) { builder.AddInt(2, parent, 0); }
  public static Offset<Joint> EndJoint(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Joint>(o);
  }
};


}
