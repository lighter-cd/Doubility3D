using System;
using System.Collections;

namespace Doubility3D.Resource.Downloader
{
	public class PacketDownloader : IDownloader
	{
		internal PacketDownloader ()
		{
		}
		public IEnumerator ResourceTask (string path, Action<Byte[],string> actOnComplate)
		{
			yield return null;
			actOnComplate (null, "not implement yet!");
		}
	}
}

