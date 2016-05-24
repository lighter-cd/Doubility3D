using System;
using System.Collections;

namespace Doubility3D.Resource.Downloader
{
	public interface IDownloader
	{
		IEnumerator ResourceTask (string path,Action<Byte[],string> actOnComplate);
	}
}

