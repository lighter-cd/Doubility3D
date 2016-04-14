// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Header : Table {
  public static Header GetRootAsHeader(ByteBuffer _bb) { return GetRootAsHeader(_bb, new Header()); }
  public static Header GetRootAsHeader(ByteBuffer _bb, Header obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Header __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Magic { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Version { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<Header> CreateHeader(FlatBufferBuilder builder,
      int magic = 0,
      int version = 0) {
    builder.StartObject(2);
    Header.AddVersion(builder, version);
    Header.AddMagic(builder, magic);
    return Header.EndHeader(builder);
  }

  public static void StartHeader(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddMagic(FlatBufferBuilder builder, int magic) { builder.AddInt(0, magic, 0); }
  public static void AddVersion(FlatBufferBuilder builder, int version) { builder.AddInt(1, version, 0); }
  public static Offset<Header> EndHeader(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Header>(o);
  }
};


}
