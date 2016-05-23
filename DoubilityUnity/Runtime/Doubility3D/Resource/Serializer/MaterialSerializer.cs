using System;
using System.Collections.Generic;

using UnityEngine;

using FlatBuffers;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

//using Doubility3D.Util;

namespace Doubility3D.Resource.Serializer
{
    static public class MaterialSerializer
    {
        static Schema.ShaderPropertyFloat fObj = new ShaderPropertyFloat();
        static Schema.ShaderPropertyTexture tObj = new ShaderPropertyTexture();
        static Schema.ShaderPropertyColor cObj = new ShaderPropertyColor();
        static Schema.ShaderPropertyVector vObj = new ShaderPropertyVector();

        public static UnityEngine.Material Load(ByteBuffer bb,Func<string,Shader> funcShader,Func<string,string,UnityEngine.Texture> funcTexture)
        {
            Schema.Material _material = Schema.Material.GetRootAsMaterial(bb);

            UnityEngine.Material material = new UnityEngine.Material(funcShader(_material.Shader));
            material.name = _material.Name;
            
            for (int i = 0; i < _material.PropertiesLength; i++)
            {
                Schema.ShaderProperty p = _material.GetProperties(i);
                switch (p.Type)
                {
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        {
                            Schema.ShaderPropertyFloat f = p.GetValue<Schema.ShaderPropertyFloat>(fObj);
                            material.SetFloat(p.Names, f.Value);
                        }
                        break;
                    case ShaderPropertyType.Color:
                        {
                            Schema.ShaderPropertyColor c = p.GetValue<Schema.ShaderPropertyColor>(cObj);
                            material.SetColor(p.Names,new UnityEngine.Color(c.Color.R,c.Color.G,c.Color.B,c.Color.A));
                        }
                        break;
                    case ShaderPropertyType.Vector:
                        {
                            Schema.ShaderPropertyVector v = p.GetValue<Schema.ShaderPropertyVector>(vObj);
                            material.SetVector(p.Names, new Vector4(v.Vector.X,v.Vector.Y,v.Vector.Z,v.Vector.W));
                        }
                        break;
                    case ShaderPropertyType.TexEnv:
                        {
                            Schema.ShaderPropertyTexture t = p.GetValue<Schema.ShaderPropertyTexture>(tObj);
							UnityEngine.Texture texture = funcTexture(t.Name,p.Names);
                            material.SetTexture(p.Names, texture);
                            material.SetTextureOffset(p.Names, new Vector2(t.Offset.X, t.Offset.Y));
                            material.SetTextureScale(p.Names, new Vector2(t.Scale.X, t.Scale.Y));
                        }
                        break;
                }
            }

            return material;
        }
    }
}
