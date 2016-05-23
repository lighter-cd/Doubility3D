using System;
using System.Collections.Generic;

using UnityEngine;

using FlatBuffers;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

//using Doubility3D.Util;

namespace Doubility3D.Resource.Serializer
{
	public class TextureSerializer : ISerializer
	{
		public UnityEngine.Object Parse (ByteBuffer bb,out String[] dependences)
		{
			Schema.Texture _texture = Schema.Texture.GetRootAsTexture(bb);

			UnityEngine.Texture2D texture = new Texture2D(_texture.Width,_texture.Height,(UnityEngine.TextureFormat)_texture.Format,_texture.MipmapCount>1);
			//texture.alphaIsTransparency = _texture.AlphaIsTransparency;
			texture.anisoLevel = _texture.AnisoLevel;
			texture.filterMode = (UnityEngine.FilterMode)_texture.FilterMode;
			texture.mipMapBias = _texture.MipMapBias;
			texture.wrapMode = (UnityEngine.TextureWrapMode)_texture.WrapMode;

			ArraySegment<byte> arrayRawData = _texture.GetRawDataBytes().GetValueOrDefault();
			byte[] rawData = new byte[arrayRawData.Count];
			Array.Copy(arrayRawData.Array,arrayRawData.Offset,rawData,0,arrayRawData.Count);
			texture.LoadRawTextureData(rawData);
			texture.Apply(true,true);

			dependences = null;
			return texture;
		}
	}
}

