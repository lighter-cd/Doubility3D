// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class File : Table {
  public static File GetRootAsFile(ByteBuffer _bb) { return GetRootAsFile(_bb, new File()); }
  public static File GetRootAsFile(ByteBuffer _bb, File obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public File __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Header Header { get { return GetHeader(new Header()); } }
  public Header GetHeader(Header obj) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public Context ContextType { get { int o = __offset(6); return o != 0 ? (Context)bb.Get(o + bb_pos) : Context.NONE; } }
  public TTable GetContext<TTable>(TTable obj) where TTable : Table { int o = __offset(8); return o != 0 ? __union(obj, o) : null; }

  public static Offset<File> CreateFile(FlatBufferBuilder builder,
      Offset<Header> headerOffset = default(Offset<Header>),
      Context context_type = Context.NONE,
      int contextOffset = 0) {
    builder.StartObject(3);
    File.AddContext(builder, contextOffset);
    File.AddHeader(builder, headerOffset);
    File.AddContextType(builder, context_type);
    return File.EndFile(builder);
  }

  public static void StartFile(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddHeader(FlatBufferBuilder builder, Offset<Header> headerOffset) { builder.AddOffset(0, headerOffset.Value, 0); }
  public static void AddContextType(FlatBufferBuilder builder, Context contextType) { builder.AddByte(1, (byte)contextType, 0); }
  public static void AddContext(FlatBufferBuilder builder, int contextOffset) { builder.AddOffset(2, contextOffset, 0); }
  public static Offset<File> EndFile(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<File>(o);
  }
};


}
