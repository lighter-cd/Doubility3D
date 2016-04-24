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
	public class TextureSaverTest
	{
		AssetBundle ab;
		UnityEngine.Texture2D originTexture;
		Schema.Texture texture;

		[SetUp]
		public void Init()
		{
			ab = TestData.LoadBundle("texturetest.bundle");
			originTexture = TestData.LoadFirstAsset<UnityEngine.Texture2D>(ab);
			Assert.IsNotNull(originTexture);

			ByteBuffer result = TextureSaver.Save(originTexture);
			texture = Schema.Texture.GetRootAsTexture(result);
		}

		[TearDown]
		public void Cleanup()
		{
			ab.Unload(true);
			originTexture = null;
			ab = null;
			texture = null;
		}

		[Test]
		public void EqualSource()
		{
			Assert.AreEqual(texture.Cube,false);
			Assert.AreEqual(texture.Format,(Schema.TextureFormat)originTexture.format);
			Assert.AreEqual(texture.MipmapCount,originTexture.mipmapCount);
			Assert.AreEqual(texture.AlphaIsTransparency,originTexture.alphaIsTransparency);
			Assert.AreEqual(texture.Width,originTexture.width);
			Assert.AreEqual(texture.Height,originTexture.height);
			Assert.AreEqual(texture.Depth,1);
			Assert.AreEqual(texture.AnisoLevel,originTexture.anisoLevel);
			Assert.AreEqual(texture.FilterMode,(Schema.FilterMode)originTexture.filterMode);
			Assert.AreEqual(texture.MipMapBias,originTexture.mipMapBias);
			Assert.AreEqual(texture.WrapMode,(Schema.TextureWrapMode)originTexture.wrapMode);

			byte[] originRawData = originTexture.GetRawTextureData();

			ArraySegment<byte> arrayRawData = texture.GetRawDataBytes().GetValueOrDefault();
			byte[] rawData = new byte[arrayRawData.Count];
			Array.Copy(arrayRawData.Array,arrayRawData.Offset,rawData,0,arrayRawData.Count);

			Assert.AreEqual(originRawData.Length,texture.RawDataLength);
			Assert.AreEqual(originRawData.Length,arrayRawData.Count);

			for(int i=0;i<originRawData.Length;i++){
				Assert.AreEqual(originRawData[i],rawData[i]);
			}
		}
	}
}

