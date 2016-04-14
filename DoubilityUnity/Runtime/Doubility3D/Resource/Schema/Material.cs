// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Material : Table {
  public static Material GetRootAsMaterial(ByteBuffer _bb) { return GetRootAsMaterial(_bb, new Material()); }
  public static Material GetRootAsMaterial(ByteBuffer _bb, Material obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Material __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetNameBytes() { return __vector_as_arraysegment(4); }
  public string Shader { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetShaderBytes() { return __vector_as_arraysegment(6); }
  public ShaderProperty GetProperties(int j) { return GetProperties(new ShaderProperty(), j); }
  public ShaderProperty GetProperties(ShaderProperty obj, int j) { int o = __offset(8); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int PropertiesLength { get { int o = __offset(8); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<Material> CreateMaterial(FlatBufferBuilder builder,
      StringOffset nameOffset = default(StringOffset),
      StringOffset shaderOffset = default(StringOffset),
      VectorOffset propertiesOffset = default(VectorOffset)) {
    builder.StartObject(3);
    Material.AddProperties(builder, propertiesOffset);
    Material.AddShader(builder, shaderOffset);
    Material.AddName(builder, nameOffset);
    return Material.EndMaterial(builder);
  }

  public static void StartMaterial(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddShader(FlatBufferBuilder builder, StringOffset shaderOffset) { builder.AddOffset(1, shaderOffset.Value, 0); }
  public static void AddProperties(FlatBufferBuilder builder, VectorOffset propertiesOffset) { builder.AddOffset(2, propertiesOffset.Value, 0); }
  public static VectorOffset CreatePropertiesVector(FlatBufferBuilder builder, Offset<ShaderProperty>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartPropertiesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Material> EndMaterial(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Material>(o);
  }
};


}
