using UnityEngine;
using System.Collections;
using FlatBuffers;

using Doubility3D.Resource;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

static public class FileLoader {
    static public UnityEngine.Object Load(byte[] bytes)
    {
        // 读取文件头

        // 读取文件内容
        return null;
    }
    static Schema.Header LoadHeader(ByteBuffer buf)
    {
        return null;
    }

    static UnityEngine.GameObject LoadSkeleton(ByteBuffer buf)
    {
        return null;
    }

    static UnityEngine.Mesh LoadMesh(ByteBuffer buf,ref string[] joints)
    {
        
        return null;
    }

    static UnityEngine.Material LoadMaterial(ByteBuffer buf)
    {
        return null;
    }

    static UnityEngine.AnimationClip LoadAnimationClip(ByteBuffer buf)
    {
        return null;
    }
}
