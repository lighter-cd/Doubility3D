using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using FlatBuffers;
using Doubility3D.Resource.Unserializing;
using Doubility3D.Resource.Schema;
using Doubility3D.Resource.ResourceObj;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using System.Collections.Generic;

using UnitTest.Doubility3D;

namespace UnitTest.Doubility3D.Resource.Saver
{
    [TestFixture]

    public class MaterialLoaderTest
    {
		ResourceObjectMaterial resultMaterial;
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

			MaterialUnserializer unserializer = UnserializerFactory.Instance.Create (context) as MaterialUnserializer;
			resultMaterial = unserializer.Parse(bb) as ResourceObjectMaterial;
            material = Schema.Material.GetRootAsMaterial(bb);
        }

        [TearDown]
        public void Cleanup()
        {
			resultMaterial.Dispose ();
            resultMaterial = null;
            material = null;
            dictTextureName.Clear();
        }

        [Test]
        public void EqualSource()
        {
			UnityEngine.Material realMaterial = resultMaterial.Unity3dObject as UnityEngine.Material;

			Assert.AreEqual(material.Name, realMaterial.name);
            Assert.AreEqual(material.Shader, realMaterial.shader.name);
            for (int i = 0; i < material.PropertiesLength; i++)
            {
                Schema.ShaderProperty p = material.GetProperties(i);

                Assert.IsTrue(realMaterial.HasProperty(p.Names));

                switch (p.Type)
                {
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        {
                            Assert.AreEqual(p.ValueType, ShaderPropertyValue.ShaderPropertyFloat);

                            float originValue = realMaterial.GetFloat(p.Names);
                            ShaderPropertyFloat f = p.GetValue<ShaderPropertyFloat>(new ShaderPropertyFloat());
                            Assert.AreEqual(f.Value, originValue);
                        }
                        break;
                    case ShaderPropertyType.Color:
                        {
                            Assert.AreEqual(p.ValueType, ShaderPropertyValue.ShaderPropertyColor);

                            UnityEngine.Color originValue = realMaterial.GetColor(p.Names);
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

                            UnityEngine.Vector4 originValue = realMaterial.GetVector(p.Names);
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
                            //UnityEngine.Texture texture = realMaterial.GetTexture(p.Names);
                            Vector2 offset = realMaterial.GetTextureOffset(p.Names);
                            Vector2 scale = realMaterial.GetTextureScale(p.Names);

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
			UnityEngine.Material realMaterial = resultMaterial.Unity3dObject as UnityEngine.Material;
			Dictionary<string, Schema.ShaderProperty> dictProperties = new Dictionary<string, ShaderProperty>();
            for (int i = 0; i < material.PropertiesLength; i++)
            {
                Schema.ShaderProperty p = material.GetProperties(i);
                dictProperties.Add(p.Names, p);
            }


			int count = ShaderUtil.GetPropertyCount(realMaterial.shader);
            Assert.AreEqual(count, material.PropertiesLength);


            for (int i = 0; i < count; i++)
            {
				string name = ShaderUtil.GetPropertyName(realMaterial.shader, i);
                Assert.IsTrue(dictProperties.ContainsKey(name));
            }
        }
    }
}
