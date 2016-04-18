// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class CurveBinding : Table {
  public static CurveBinding GetRootAsCurveBinding(ByteBuffer _bb) { return GetRootAsCurveBinding(_bb, new CurveBinding()); }
  public static CurveBinding GetRootAsCurveBinding(ByteBuffer _bb, CurveBinding obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public CurveBinding __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string PropertyName { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPropertyNameBytes() { return __vector_as_arraysegment(4); }
  public string Path { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetPathBytes() { return __vector_as_arraysegment(6); }
  public string Type { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public ArraySegment<byte>? GetTypeBytes() { return __vector_as_arraysegment(8); }
  public AnimationCurve Curve { get { return GetCurve(new AnimationCurve()); } }
  public AnimationCurve GetCurve(AnimationCurve obj) { int o = __offset(10); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }

  public static Offset<CurveBinding> CreateCurveBinding(FlatBufferBuilder builder,
      StringOffset propertyNameOffset = default(StringOffset),
      StringOffset pathOffset = default(StringOffset),
      StringOffset typeOffset = default(StringOffset),
      Offset<AnimationCurve> curveOffset = default(Offset<AnimationCurve>)) {
    builder.StartObject(4);
    CurveBinding.AddCurve(builder, curveOffset);
    CurveBinding.AddType(builder, typeOffset);
    CurveBinding.AddPath(builder, pathOffset);
    CurveBinding.AddPropertyName(builder, propertyNameOffset);
    return CurveBinding.EndCurveBinding(builder);
  }

  public static void StartCurveBinding(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddPropertyName(FlatBufferBuilder builder, StringOffset propertyNameOffset) { builder.AddOffset(0, propertyNameOffset.Value, 0); }
  public static void AddPath(FlatBufferBuilder builder, StringOffset pathOffset) { builder.AddOffset(1, pathOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, StringOffset typeOffset) { builder.AddOffset(2, typeOffset.Value, 0); }
  public static void AddCurve(FlatBufferBuilder builder, Offset<AnimationCurve> curveOffset) { builder.AddOffset(3, curveOffset.Value, 0); }
  public static Offset<CurveBinding> EndCurveBinding(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<CurveBinding>(o);
  }
};


}
