using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using FlatBuffers;
using Doubility3D.Resource.Saver;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using System.Collections.Generic;
using System;

using UnitTest.Doubility3D;

namespace UnitTest.Doubility3D.Resource.Saver
{
    [TestFixture]
    public class MaterialSaveTest
    {
        AssetBundle ab;
        UnityEngine.Material originMaterial;
        Schema.Material material;

        [SetUp]
        public void Init()
        {
            ab = TestData.LoadBundle("materialtest.bundle");
            originMaterial = TestData.LoadFirstAsset<UnityEngine.Material>(ab);
            Assert.IsNotNull(originMaterial);

            ByteBuffer result = MaterialSaver.Save(originMaterial);
            material = Schema.Material.GetRootAsMaterial(result);
        }

        [TearDown]
        public void Cleanup()
        {
            ab.Unload(true);
            originMaterial = null;
            ab = null;
            material = null;
        }

        [Test]
        public void EqualSource()
        {
            Assert.AreEqual(material.Name, originMaterial.name);
            Assert.AreEqual(material.Shader, originMaterial.shader.name);
            for (int i = 0; i < material.PropertiesLength; i++)
            {
                Schema.ShaderProperty p = material.GetProperties(i);

                Assert.IsTrue(originMaterial.HasProperty(p.Names));

                switch(p.Type){
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        {
                            Assert.AreEqual(p.ValueType, ShaderPropertyValue.ShaderPropertyFloat);
                            
                            float originValue = originMaterial.GetFloat(p.Names);
                            ShaderPropertyFloat f =  p.GetValue<ShaderPropertyFloat>(new ShaderPropertyFloat());
                            Assert.AreEqual(f.Value, originValue);
                        }
                        break;
                    case ShaderPropertyType.Color:
                        {
                            Assert.AreEqual(p.ValueType, ShaderPropertyValue.ShaderPropertyColor);

                            UnityEngine.Color originValue = originMaterial.GetColor(p.Names);
                            ShaderPropertyColor c = p.GetValue<ShaderPropertyColor>(new ShaderPropertyColor());
                            Assert.AreEqual(originValue.a, c.Color.A);
                            Assert.AreEqual(originValue.g, c.Color.G);
                            Assert.AreEqual(originValue.b, c.Color.B);
                            Assert.AreEqual(originValue.r, c.Color.R);
                        }
                        break;
                    case ShaderPropertyType.Vector:
                        {
                            Assert.AreEqual(p.ValueType, ShaderPropertyValue.ShaderPropertyVector);

                            UnityEngine.Vector4 originValue = originMaterial.GetVector(p.Names);
                            ShaderPropertyVector v = p.GetValue<ShaderPropertyVector>(new ShaderPropertyVector());
                            Assert.AreEqual(originValue.x, v.Vector.X);
                            Assert.AreEqual(originValue.y, v.Vector.Y);
                            Assert.AreEqual(originValue.z, v.Vector.Z);
                            Assert.AreEqual(originValue.w, v.Vector.W);
                        }
                        break;
                    case ShaderPropertyType.TexEnv:
                        {
                            Assert.AreEqual(p.ValueType, ShaderPropertyValue.ShaderPropertyTexture);
                            UnityEngine.Texture texture = originMaterial.GetTexture(p.Names);
                            Vector2 offset = originMaterial.GetTextureOffset(p.Names);
                            Vector2 scale = originMaterial.GetTextureScale(p.Names);
                            
                            Assert.IsFalse(texture == null);
                            ShaderPropertyTexture t = p.GetValue<ShaderPropertyTexture>(new ShaderPropertyTexture());

                            Assert.AreEqual(texture.name, t.Name);
                            Assert.AreEqual(offset.x, t.Offset.X);
                            Assert.AreEqual(offset.y, t.Offset.Y);
                            Assert.AreEqual(scale.x, t.Scale.X);
                            Assert.AreEqual(scale.y, t.Scale.Y);
                        }
                        break;
                }
            }
        }
        [Test]
        public void AllPropertiesExist()
        {
            Dictionary<string, Schema.ShaderProperty> dictProperties = new Dictionary<string, ShaderProperty>();
            for (int i = 0; i < material.PropertiesLength; i++)
            {
                Schema.ShaderProperty p = material.GetProperties(i);
                dictProperties.Add(p.Names, p);
            }
            

            int count = ShaderUtil.GetPropertyCount(originMaterial.shader);
            Assert.AreEqual(count, material.PropertiesLength);


            for (int i = 0; i < count; i++)
            {
                string name = ShaderUtil.GetPropertyName(originMaterial.shader, i);
                Assert.IsTrue(dictProperties.ContainsKey(name));
            }
        }
    }
}