using System;
using System.Collections;

namespace Doubility3D.Resource.Downloader
{
	public interface IDownloader : IDisposable
	{
		IEnumerator ResourceTask (string path,Action<Byte[],string> actOnComplate);
	}
}

