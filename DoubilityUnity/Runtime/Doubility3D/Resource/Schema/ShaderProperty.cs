// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class ShaderProperty : Table {
  public static ShaderProperty GetRootAsShaderProperty(ByteBuffer _bb) { return GetRootAsShaderProperty(_bb, new ShaderProperty()); }
  public static ShaderProperty GetRootAsShaderProperty(ByteBuffer _bb, ShaderProperty obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ShaderProperty __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Names { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNamesBytes() { return __vector_as_arraysegment(4); }
  public ShaderPropertyType Type { get { int o = __offset(6); return o != 0 ? (ShaderPropertyType)bb.GetSbyte(o + bb_pos) : ShaderPropertyType.Float; } }
  public ShaderPropertyValue ValueType { get { int o = __offset(8); return o != 0 ? (ShaderPropertyValue)bb.Get(o + bb_pos) : ShaderPropertyValue.NONE; } }
  public TTable GetValue<TTable>(TTable obj) where TTable : Table { int o = __offset(10); return o != 0 ? __union(obj, o) : null; }

  public static Offset<ShaderProperty> CreateShaderProperty(FlatBufferBuilder builder,
      StringOffset namesOffset = default(StringOffset),
      ShaderPropertyType type = ShaderPropertyType.Float,
      ShaderPropertyValue value_type = ShaderPropertyValue.NONE,
      int valueOffset = 0) {
    builder.StartObject(4);
    ShaderProperty.AddValue(builder, valueOffset);
    ShaderProperty.AddNames(builder, namesOffset);
    ShaderProperty.AddValueType(builder, value_type);
    ShaderProperty.AddType(builder, type);
    return ShaderProperty.EndShaderProperty(builder);
  }

  public static void StartShaderProperty(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddNames(FlatBufferBuilder builder, StringOffset namesOffset) { builder.AddOffset(0, namesOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, ShaderPropertyType type) { builder.AddSbyte(1, (sbyte)type, 2); }
  public static void AddValueType(FlatBufferBuilder builder, ShaderPropertyValue valueType) { builder.AddByte(2, (byte)valueType, 0); }
  public static void AddValue(FlatBufferBuilder builder, int valueOffset) { builder.AddOffset(3, valueOffset, 0); }
  public static Offset<ShaderProperty> EndShaderProperty(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    builder.Required(o, 4);  // names
    builder.Required(o, 10);  // value
    return new Offset<ShaderProperty>(o);
  }
};


}
