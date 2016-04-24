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

        public static ByteBuffer Save(UnityEngine.Material material)
        {
			FlatBufferBuilder builder = new FlatBufferBuilder(InitBufferSize);

			int count = ShaderUtil.GetPropertyCount(material.shader);
			List<Offset<ShaderProperty>> listOffShaderProperties = new List<Offset<ShaderProperty>>();

			for(int i=0;i<count;i++){
				string name = ShaderUtil.GetPropertyName(material.shader,i);
				ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(material.shader,i);
				Schema.ShaderPropertyValue valueType = ShaderPropertyValue.NONE;
                int valueOffset = -1;
				switch(type){
				case ShaderUtil.ShaderPropertyType.Color:
					{
						UnityEngine.Color c = material.GetColor(name);
                        ShaderPropertyColor.StartShaderPropertyColor(builder);
                        ShaderPropertyColor.AddColor(builder, Schema.Color.CreateColor(builder, c.a, c.b, c.g, c.r));
                        Offset<ShaderPropertyColor> offset = ShaderPropertyColor.EndShaderPropertyColor(builder);
                        valueType = ShaderPropertyValue.ShaderPropertyColor;
                        valueOffset = offset.Value;
					}
					break;
				case ShaderUtil.ShaderPropertyType.Vector:
					{
						Vector4 v = material.GetVector(name);
                        ShaderPropertyVector.StartShaderPropertyVector(builder);
                        ShaderPropertyVector.AddVector(builder, Schema.Vec4.CreateVec4(builder, v.x, v.y, v.z, v.w));
                        Offset<ShaderPropertyVector> offset = ShaderPropertyVector.EndShaderPropertyVector(builder);
                        valueType = ShaderPropertyValue.ShaderPropertyVector;
                        valueOffset = offset.Value;
                    }
					break;
				case ShaderUtil.ShaderPropertyType.Range:
				case ShaderUtil.ShaderPropertyType.Float:
					{
						float f = material.GetFloat(name);
                        ShaderPropertyFloat.StartShaderPropertyFloat(builder);
                        ShaderPropertyFloat.AddValue(builder, f);
                        Offset<ShaderPropertyFloat> offset = ShaderPropertyFloat.EndShaderPropertyFloat(builder);
                        valueType = ShaderPropertyValue.ShaderPropertyFloat;
                        valueOffset = offset.Value;
					}
					break;
				case ShaderUtil.ShaderPropertyType.TexEnv:
					{
						UnityEngine.Texture t = material.GetTexture(name);
                        string textureName = "$NULL_TEXTURE";
                        if (t != null)
                        {
							textureName = AssetDatabase.GetAssetPath(t.GetInstanceID());
							if(string.IsNullOrEmpty(textureName)){
								textureName = t.name;
							}else{
								textureName = textureName.Substring("Assets/ArtWork/".Length);
								textureName = System.IO.Path.GetDirectoryName(textureName) + "/" + System.IO.Path.GetFileNameWithoutExtension(textureName);
							}
                        }
                        Vector2 toffset = material.GetTextureOffset(name);
                        Vector2 tscale = material.GetTextureScale(name);

                        StringOffset pathOffset = builder.CreateString(textureName);
                        ShaderPropertyTexture.StartShaderPropertyTexture(builder);
                        ShaderPropertyTexture.AddName(builder, pathOffset);
                        ShaderPropertyTexture.AddOffset(builder, Vec2.CreateVec2(builder, toffset.x, toffset.y));
                        ShaderPropertyTexture.AddScale(builder, Vec2.CreateVec2(builder, tscale.x, tscale.y));
                        Offset<ShaderPropertyTexture> offset = ShaderPropertyTexture.EndShaderPropertyTexture(builder);
                        valueType = ShaderPropertyValue.ShaderPropertyTexture;
                        valueOffset = offset.Value;
					}
					break;
				}

                if (valueOffset >= 0)
                {
                    listOffShaderProperties.Add(
                        ShaderProperty.CreateShaderProperty(
                            builder, builder.CreateString(name), (Schema.ShaderPropertyType)type, valueType, valueOffset
                            ));
                }
			}
			StringOffset offMaterialName = builder.CreateString(material.name);
			StringOffset offShader = builder.CreateString(material.shader.name);
			Offset<Schema.Material> offMaterial = Schema.Material.CreateMaterial(
                builder, offMaterialName, offShader, Schema.Material.CreatePropertiesVector(builder, listOffShaderProperties.ToArray()));

			builder.Finish(offMaterial.Value);
            return builder.DataBuffer;
        }
    }
}
