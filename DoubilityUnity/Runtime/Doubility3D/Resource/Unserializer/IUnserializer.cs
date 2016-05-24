using System;
using FlatBuffers;
using Doubility3D.Resource.ResourceObj;

namespace Doubility3D.Resource.Unserializing
{
	public interface IUnserializer
	{
		ResourceObject Parse (ByteBuffer bb);
	}
}

