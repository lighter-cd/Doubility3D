using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using FlatBuffers;
using Doubility3D.Resource.Loader;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using System.Collections.Generic;

using UnitTest.Doubility3D;

namespace UnitTest.Doubility3D.Resource.Saver
{
    [TestFixture]

    public class MaterialLoaderTest
    {
        UnityEngine.Material resultMaterial;
        Schema.Material material;
        Dictionary<string, string> dictTextureName = new Dictionary<string, string>();

        Shader GetShader(string name)
        {
            return Shader.Find(name);
        }
		UnityEngine.Texture GetTexture(string nameTexture, string nameProperty)
        {
            dictTextureName.Add(nameProperty, nameTexture);
            return null;
        }

        [SetUp]
        public void Init()
        {
            Context context = Context.Unknown;
            ByteBuffer bb = TestData.LoadResource("Suit_Metal_Dragon_Male.doub", out context);
            Assert.IsNotNull(bb);
            Assert.AreNotEqual(context, Context.Unknown);

            resultMaterial = MaterialLoader.Load(bb, GetShader, GetTexture);
            material = Schema.Material.GetRootAsMaterial(bb);
        }

        [TearDown]
        public void Cleanup()
        {
            UnityEngine.Object.DestroyImmediate(resultMaterial);
            resultMaterial = null;
            material = null;
            dictTextureName.Clear();
        }

        [Test]
        public void EqualSource()
        {
            Assert.AreEqual(material.Name, resultMaterial.name);
            Assert.AreEqual(material.Shader, resultMaterial.shader.name);
            for (int i = 0; i < material.PropertiesLength; i++)
            {
                Schema.ShaderProperty p = material.GetProperties(i);

                Assert.IsTrue(resultMaterial.HasProperty(p.Names));

                switch (p.Type)
                {
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        {
                            Assert.AreEqual(p.ValueType, ShaderPropertyValue.ShaderPropertyFloat);

                            float originValue = resultMaterial.GetFloat(p.Names);
                            ShaderPropertyFloat f = p.GetValue<ShaderPropertyFloat>(new ShaderPropertyFloat());
                            Assert.AreEqual(f.Value, originValue);
                        }
                        break;
                    case ShaderPropertyType.Color:
                        {
                            Assert.AreEqual(p.ValueType, ShaderPropertyValue.ShaderPropertyColor);

                            UnityEngine.Color originValue = resultMaterial.GetColor(p.Names);
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

                            UnityEngine.Vector4 originValue = resultMaterial.GetVector(p.Names);
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
                            //UnityEngine.Texture texture = resultMaterial.GetTexture(p.Names);
                            Vector2 offset = resultMaterial.GetTextureOffset(p.Names);
                            Vector2 scale = resultMaterial.GetTextureScale(p.Names);

                            //这个测试用例不真正装载 texture.
                            //Assert.IsFalse(texture == null);
                            ShaderPropertyTexture t = p.GetValue<ShaderPropertyTexture>(new ShaderPropertyTexture());

                            Assert.IsTrue(dictTextureName.ContainsKey(p.Names));
                            Assert.AreEqual(dictTextureName[p.Names], t.Name);
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


            int count = ShaderUtil.GetPropertyCount(resultMaterial.shader);
            Assert.AreEqual(count, material.PropertiesLength);


            for (int i = 0; i < count; i++)
            {
                string name = ShaderUtil.GetPropertyName(resultMaterial.shader, i);
                Assert.IsTrue(dictProperties.ContainsKey(name));
            }
        }
    }
}
