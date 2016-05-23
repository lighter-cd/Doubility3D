using System;
using FlatBuffers;

namespace Doubility3D.Resource.Serializer
{
	public interface ISerializer
	{
		UnityEngine.Object Parse (ByteBuffer bb,out String[] dependences);
	}
}

