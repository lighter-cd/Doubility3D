using System;
using System.Collections.Generic;
using UnityEngine;

using FlatBuffers;
using Doubility3D.Resource;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

namespace Doubility3D.Resource.Saver
{
	static public class TextureSaver
	{
		const int InitBufferSize = 8192;
		public static ByteBuffer Save (UnityEngine.Texture2D texture)
		{
			FlatBufferBuilder builder = new FlatBufferBuilder(InitBufferSize);

			VectorOffset voRawData = Schema.Texture.CreateRawDataVector(builder,texture.GetRawTextureData());

			Schema.Texture.StartTexture(builder);

			Schema.Texture.AddCube(builder,false);

			Schema.Texture.AddFormat(builder,(Schema.TextureFormat)texture.format);
			Schema.Texture.AddMipmapCount(builder,texture.mipmapCount);
			Schema.Texture.AddAlphaIsTransparency(builder,texture.alphaIsTransparency);

			Schema.Texture.AddWidth(builder,texture.width);
			Schema.Texture.AddHeight(builder,texture.height);
			Schema.Texture.AddDepth(builder,1);
			Schema.Texture.AddAnisoLevel(builder,texture.anisoLevel);
			Schema.Texture.AddFilterMode(builder,(Schema.FilterMode)texture.filterMode);
			Schema.Texture.AddMipMapBias(builder,texture.mipMapBias);
			Schema.Texture.AddWrapMode(builder,(Schema.TextureWrapMode)texture.wrapMode);

			Schema.Texture.AddRawData(builder,voRawData);

			builder.Finish(Schema.Texture.EndTexture(builder).Value);

			return builder.DataBuffer;
		}
	}
}

