// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class ShaderPropertyVector : Table {
  public static ShaderPropertyVector GetRootAsShaderPropertyVector(ByteBuffer _bb) { return GetRootAsShaderPropertyVector(_bb, new ShaderPropertyVector()); }
  public static ShaderPropertyVector GetRootAsShaderPropertyVector(ByteBuffer _bb, ShaderPropertyVector obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ShaderPropertyVector __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Doubility3D.Resource.Schema.Vec4 Vector { get { return GetVector(new Doubility3D.Resource.Schema.Vec4()); } }
  public Doubility3D.Resource.Schema.Vec4 GetVector(Doubility3D.Resource.Schema.Vec4 obj) { int o = __offset(4); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartShaderPropertyVector(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddVector(FlatBufferBuilder builder, Offset<Doubility3D.Resource.Schema.Vec4> vectorOffset) { builder.AddStruct(0, vectorOffset.Value, 0); }
  public static Offset<ShaderPropertyVector> EndShaderPropertyVector(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    builder.Required(o, 4);  // vector
    return new Offset<ShaderPropertyVector>(o);
  }
};


}
