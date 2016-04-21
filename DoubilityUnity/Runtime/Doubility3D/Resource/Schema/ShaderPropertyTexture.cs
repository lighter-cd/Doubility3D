// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class ShaderPropertyTexture : Table {
  public static ShaderPropertyTexture GetRootAsShaderPropertyTexture(ByteBuffer _bb) { return GetRootAsShaderPropertyTexture(_bb, new ShaderPropertyTexture()); }
  public static ShaderPropertyTexture GetRootAsShaderPropertyTexture(ByteBuffer _bb, ShaderPropertyTexture obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ShaderPropertyTexture __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(4); }
  public Doubility3D.Resource.Schema.Vec2 Offset { get { return GetOffset(new Doubility3D.Resource.Schema.Vec2()); } }
  public Doubility3D.Resource.Schema.Vec2 GetOffset(Doubility3D.Resource.Schema.Vec2 obj) { int o = __offset(6); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Doubility3D.Resource.Schema.Vec2 Scale { get { return GetScale(new Doubility3D.Resource.Schema.Vec2()); } }
  public Doubility3D.Resource.Schema.Vec2 GetScale(Doubility3D.Resource.Schema.Vec2 obj) { int o = __offset(8); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartShaderPropertyTexture(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddOffset(FlatBufferBuilder builder, Offset<Doubility3D.Resource.Schema.Vec2> offsetOffset) { builder.AddStruct(1, offsetOffset.Value, 0); }
  public static void AddScale(FlatBufferBuilder builder, Offset<Doubility3D.Resource.Schema.Vec2> scaleOffset) { builder.AddStruct(2, scaleOffset.Value, 0); }
  public static Offset<ShaderPropertyTexture> EndShaderPropertyTexture(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    builder.Required(o, 4);  // name
    builder.Required(o, 6);  // offset
    builder.Required(o, 8);  // scale
    return new Offset<ShaderPropertyTexture>(o);
  }
};


}
