using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using FlatBuffers;
using Doubility3D.Resource.Loader;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using System.Collections.Generic;
using System;

using UnitTest.Doubility3D;

namespace UnitTest.Doubility3D.Resource.Saver
{
	[TestFixture]
	public class TextureLoaderTest
	{
		UnityEngine.Texture2D resultTexture;
		Schema.Texture texture;

		[SetUp]
		public void Init()
		{
			Context context = Context.Unknown;
			ByteBuffer bb = TestData.LoadResource("suit_metal_dragon_male."+PlatformPath.GetPath(Application.platform).ToLower()+".texture", out context);
			Assert.IsNotNull(bb);
			Assert.AreNotEqual(context, Context.Unknown);

			resultTexture = TextureLoader.Load(bb);
			Assert.IsNotNull(resultTexture);
			texture = Schema.Texture.GetRootAsTexture(bb);
		}

		[TearDown]
		public void Cleanup()
		{
			UnityEngine.Object.DestroyImmediate(resultTexture);
			resultTexture = null;
			texture = null;
		}

		[Test]
		public void EqualSource()
		{
			Assert.AreEqual(texture.Cube,false);
			Assert.AreEqual(texture.Format,(Schema.TextureFormat)resultTexture.format);
			Assert.AreEqual(texture.MipmapCount,resultTexture.mipmapCount);
			Assert.AreEqual(texture.AlphaIsTransparency,resultTexture.alphaIsTransparency);
			Assert.AreEqual(texture.Width,resultTexture.width);
			Assert.AreEqual(texture.Height,resultTexture.height);
			Assert.AreEqual(texture.Depth,1);
			Assert.AreEqual(texture.AnisoLevel,resultTexture.anisoLevel);
			Assert.AreEqual(texture.FilterMode,(Schema.FilterMode)resultTexture.filterMode);
			Assert.AreEqual(texture.MipMapBias,resultTexture.mipMapBias);
			Assert.AreEqual(texture.WrapMode,(Schema.TextureWrapMode)resultTexture.wrapMode);

			byte[] resultRawData = resultTexture.GetRawTextureData();
			ArraySegment<byte> arrayRawData = texture.GetRawDataBytes().GetValueOrDefault();
			byte[] rawData = new byte[arrayRawData.Count];
			Array.Copy(arrayRawData.Array,arrayRawData.Offset,rawData,0,arrayRawData.Count);

			Assert.AreEqual(resultRawData.Length,texture.RawDataLength);
			Assert.AreEqual(resultRawData.Length,arrayRawData.Count);

			for(int i=0;i<resultRawData.Length;i++){
				Assert.AreEqual(resultRawData[i],rawData[i]);
			}
		}
	}
}

