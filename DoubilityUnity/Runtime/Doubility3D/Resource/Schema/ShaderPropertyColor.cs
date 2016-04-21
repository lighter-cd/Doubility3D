// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class ShaderPropertyColor : Table {
  public static ShaderPropertyColor GetRootAsShaderPropertyColor(ByteBuffer _bb) { return GetRootAsShaderPropertyColor(_bb, new ShaderPropertyColor()); }
  public static ShaderPropertyColor GetRootAsShaderPropertyColor(ByteBuffer _bb, ShaderPropertyColor obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ShaderPropertyColor __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Doubility3D.Resource.Schema.Color Color { get { return GetColor(new Doubility3D.Resource.Schema.Color()); } }
  public Doubility3D.Resource.Schema.Color GetColor(Doubility3D.Resource.Schema.Color obj) { int o = __offset(4); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartShaderPropertyColor(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddColor(FlatBufferBuilder builder, Offset<Doubility3D.Resource.Schema.Color> colorOffset) { builder.AddStruct(0, colorOffset.Value, 0); }
  public static Offset<ShaderPropertyColor> EndShaderPropertyColor(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    builder.Required(o, 4);  // color
    return new Offset<ShaderPropertyColor>(o);
  }
};


}
