using System;
using System.Collections;

namespace Doubility3D.Resource.Manager
{
	public interface IDownloader
	{
		IEnumerator ResourceTask (out Byte[] bytes);
		void Dispose();
	}
}

