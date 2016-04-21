// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class ShaderPropertyFloat : Table {
  public static ShaderPropertyFloat GetRootAsShaderPropertyFloat(ByteBuffer _bb) { return GetRootAsShaderPropertyFloat(_bb, new ShaderPropertyFloat()); }
  public static ShaderPropertyFloat GetRootAsShaderPropertyFloat(ByteBuffer _bb, ShaderPropertyFloat obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ShaderPropertyFloat __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float Value { get { int o = __offset(4); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<ShaderPropertyFloat> CreateShaderPropertyFloat(FlatBufferBuilder builder,
      float value = 0) {
    builder.StartObject(1);
    ShaderPropertyFloat.AddValue(builder, value);
    return ShaderPropertyFloat.EndShaderPropertyFloat(builder);
  }

  public static void StartShaderPropertyFloat(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddValue(FlatBufferBuilder builder, float value) { builder.AddFloat(0, value, 0); }
  public static Offset<ShaderPropertyFloat> EndShaderPropertyFloat(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ShaderPropertyFloat>(o);
  }
};


}
