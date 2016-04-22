using UnityEngine;
using System.Collections;
using FlatBuffers;

using Doubility3D.Resource;
using Doubility3D.Resource.Loader;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

static public class FileLoader
{
    static public ByteBuffer Load(byte[] bytes, out Context context)
    {
        // 读取文件头
        if (bytes.Length < FileHeader.NumberOfBytes)
        {
            context = Context.Unknown;
            return null;
        }
        context = FileHeader.FromBytes(bytes);
        if (context == Context.Unknown)
        {
            return null;
        }

        ByteBuffer bb = new ByteBuffer(bytes, FileHeader.NumberOfBytes);
        return bb;
    }
    static public ByteBuffer LoadFromFile(string path, out Context context)
    {
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        if (bytes == null)
        {
            context = Context.Unknown;
            return null;
        }
        return Load(bytes, out context);
    }
}
