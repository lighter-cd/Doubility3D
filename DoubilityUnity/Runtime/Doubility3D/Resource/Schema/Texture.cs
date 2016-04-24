// automatically generated, do not modify

namespace Doubility3D.Resource.Schema
{

using System;
using FlatBuffers;

public sealed class Texture : Table {
  public static Texture GetRootAsTexture(ByteBuffer _bb) { return GetRootAsTexture(_bb, new Texture()); }
  public static Texture GetRootAsTexture(ByteBuffer _bb, Texture obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Texture __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public bool Cube { get { int o = __offset(4); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public TextureFormat Format { get { int o = __offset(6); return o != 0 ? (TextureFormat)bb.GetInt(o + bb_pos) : TextureFormat.RGBA32; } }
  public int MipmapCount { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool AlphaIsTransparency { get { int o = __offset(10); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int Width { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Height { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Depth { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int AnisoLevel { get { int o = __offset(18); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public FilterMode FilterMode { get { int o = __offset(20); return o != 0 ? (FilterMode)bb.GetInt(o + bb_pos) : FilterMode.Bilinear; } }
  public float MipMapBias { get { int o = __offset(22); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public TextureWrapMode WrapMode { get { int o = __offset(24); return o != 0 ? (TextureWrapMode)bb.GetInt(o + bb_pos) : TextureWrapMode.Repeat; } }
  public byte GetRawData(int j) { int o = __offset(26); return o != 0 ? bb.Get(__vector(o) + j * 1) : (byte)0; }
  public int RawDataLength { get { int o = __offset(26); return o != 0 ? __vector_len(o) : 0; } }
  public ArraySegment<byte>? GetRawDataBytes() { return __vector_as_arraysegment(26); }

  public static Offset<Texture> CreateTexture(FlatBufferBuilder builder,
      bool cube = false,
      TextureFormat format = TextureFormat.RGBA32,
      int mipmapCount = 0,
      bool alphaIsTransparency = false,
      int width = 0,
      int height = 0,
      int depth = 0,
      int anisoLevel = 0,
      FilterMode filterMode = FilterMode.Bilinear,
      float mipMapBias = 0,
      TextureWrapMode wrapMode = TextureWrapMode.Repeat,
      VectorOffset rawDataOffset = default(VectorOffset)) {
    builder.StartObject(12);
    Texture.AddRawData(builder, rawDataOffset);
    Texture.AddWrapMode(builder, wrapMode);
    Texture.AddMipMapBias(builder, mipMapBias);
    Texture.AddFilterMode(builder, filterMode);
    Texture.AddAnisoLevel(builder, anisoLevel);
    Texture.AddDepth(builder, depth);
    Texture.AddHeight(builder, height);
    Texture.AddWidth(builder, width);
    Texture.AddMipmapCount(builder, mipmapCount);
    Texture.AddFormat(builder, format);
    Texture.AddAlphaIsTransparency(builder, alphaIsTransparency);
    Texture.AddCube(builder, cube);
    return Texture.EndTexture(builder);
  }

  public static void StartTexture(FlatBufferBuilder builder) { builder.StartObject(12); }
  public static void AddCube(FlatBufferBuilder builder, bool cube) { builder.AddBool(0, cube, false); }
  public static void AddFormat(FlatBufferBuilder builder, TextureFormat format) { builder.AddInt(1, (int)format, 4); }
  public static void AddMipmapCount(FlatBufferBuilder builder, int mipmapCount) { builder.AddInt(2, mipmapCount, 0); }
  public static void AddAlphaIsTransparency(FlatBufferBuilder builder, bool alphaIsTransparency) { builder.AddBool(3, alphaIsTransparency, false); }
  public static void AddWidth(FlatBufferBuilder builder, int width) { builder.AddInt(4, width, 0); }
  public static void AddHeight(FlatBufferBuilder builder, int height) { builder.AddInt(5, height, 0); }
  public static void AddDepth(FlatBufferBuilder builder, int depth) { builder.AddInt(6, depth, 0); }
  public static void AddAnisoLevel(FlatBufferBuilder builder, int anisoLevel) { builder.AddInt(7, anisoLevel, 0); }
  public static void AddFilterMode(FlatBufferBuilder builder, FilterMode filterMode) { builder.AddInt(8, (int)filterMode, 1); }
  public static void AddMipMapBias(FlatBufferBuilder builder, float mipMapBias) { builder.AddFloat(9, mipMapBias, 0); }
  public static void AddWrapMode(FlatBufferBuilder builder, TextureWrapMode wrapMode) { builder.AddInt(10, (int)wrapMode, 0); }
  public static void AddRawData(FlatBufferBuilder builder, VectorOffset rawDataOffset) { builder.AddOffset(11, rawDataOffset.Value, 0); }
  public static VectorOffset CreateRawDataVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
  public static void StartRawDataVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static Offset<Texture> EndTexture(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Texture>(o);
  }
};


}
