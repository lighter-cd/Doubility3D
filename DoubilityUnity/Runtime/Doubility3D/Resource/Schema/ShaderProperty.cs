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
  public byte GetValue(int j) { int o = __offset(8); return o != 0 ? bb.Get(__vector(o) + j * 1) : (byte)0; }
  public int ValueLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetValueBytes() { return __vector_as_arraysegment(8); }

  public static Offset<ShaderProperty> CreateShaderProperty(FlatBufferBuilder builder,
      StringOffset namesOffset = default(StringOffset),
      ShaderPropertyType type = ShaderPropertyType.Float,
      VectorOffset valueOffset = default(VectorOffset)) {
    builder.StartObject(3);
    ShaderProperty.AddValue(builder, valueOffset);
    ShaderProperty.AddNames(builder, namesOffset);
    ShaderProperty.AddType(builder, type);
    return ShaderProperty.EndShaderProperty(builder);
  }

  public static void StartShaderProperty(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddNames(FlatBufferBuilder builder, StringOffset namesOffset) { builder.AddOffset(0, namesOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, ShaderPropertyType type) { builder.AddSbyte(1, (sbyte)type, 2); }
  public static void AddValue(FlatBufferBuilder builder, VectorOffset valueOffset) { builder.AddOffset(2, valueOffset.Value, 0); }
  public static VectorOffset CreateValueVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
  public static void StartValueVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static Offset<ShaderProperty> EndShaderProperty(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ShaderProperty>(o);
  }
};


}
