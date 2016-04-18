using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using FlatBuffers;
using Doubility3D.Resource;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

namespace Doubility3D.Resource.Saver
{
    static public class MaterialSaver
    {
		const int InitBufferSize = 8192;

        public static void Save(UnityEngine.Material material, string dstFolder)
        {
			FlatBufferBuilder builder = new FlatBufferBuilder(InitBufferSize);

			int count = ShaderUtil.GetPropertyCount(material.shader);
			Offset<ShaderProperty>[] offShaderProperties = new Offset<ShaderProperty>[count];

			for(int i=0;i<count;i++){
				string name = ShaderUtil.GetPropertyName(material.shader,i);
				ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(material.shader,i);
				byte[] bytes = null;
				switch(type){
				case ShaderUtil.ShaderPropertyType.Color:
					{
						UnityEngine.Color c = material.GetColor(name);
						bytes = new byte[16];
						ByteBuffer byteBuf = new ByteBuffer(bytes);
						byteBuf.PutFloat(0,c.a);	
						byteBuf.PutFloat(4,c.b);
						byteBuf.PutFloat(8,c.g);
						byteBuf.PutFloat(12,c.r);	
					}
					break;
				case ShaderUtil.ShaderPropertyType.Vector:
					{
						Vector4 v = material.GetVector(name);
						bytes = new byte[16];
						ByteBuffer byteBuf = new ByteBuffer(bytes);
						byteBuf.PutFloat(0,v.x);	
						byteBuf.PutFloat(4,v.y);
						byteBuf.PutFloat(8,v.z);
						byteBuf.PutFloat(12,v.w);	
					}
					break;
				case ShaderUtil.ShaderPropertyType.Range:
				case ShaderUtil.ShaderPropertyType.Float:
					{
						float f = material.GetFloat(name);
						bytes = System.BitConverter.GetBytes(f);
					}
					break;
				case ShaderUtil.ShaderPropertyType.TexEnv:
					{
						Texture t = material.GetTexture(name);
						string path = AssetDatabase.GetAssetPath(t.GetInstanceID());
						bytes = System.Text.Encoding.Default.GetBytes(path);
					}
					break;
				}

				StringOffset offName = builder.CreateString(name);
				VectorOffset vecValue = ShaderProperty.CreateValueVector(builder,bytes);
				offShaderProperties[i] = ShaderProperty.CreateShaderProperty(builder,offName,(Schema.ShaderPropertyType)type,vecValue);
			}
			StringOffset offMaterialName = builder.CreateString(material.name);
			StringOffset offShader = builder.CreateString(material.shader.name);
			Offset<Schema.Material> offMaterial = Schema.Material.CreateMaterial(
				builder,offMaterialName,offShader,Schema.Material.CreatePropertiesVector(builder,offShaderProperties));

			builder.Finish(offMaterial.Value);

			FileSaver.Save(builder.DataBuffer,Context.Material,dstFolder + "/" + material.name + ".db3d");
        }
    }
}
