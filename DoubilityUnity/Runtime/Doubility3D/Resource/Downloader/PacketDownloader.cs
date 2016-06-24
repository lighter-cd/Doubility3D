using System;
using System.Collections;

namespace Doubility3D.Resource.Downloader
{
	// 包名模式(两个包)
	// 所在目录(StreamAssets/DataPath)
	// 
	public class PacketDownloader : IDownloader
	{
		string home;
		internal PacketDownloader (string _home)
		{
			home = _home;
		}
		public IEnumerator ResourceTask (string path, Action<Byte[],string> actOnComplate)
		{
			yield return null;
			actOnComplate (null, "not implement yet!");
		}
		public string Home { get { return home;} }
	}
}

