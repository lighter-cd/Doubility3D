using System;
using System.Collections;

namespace Doubility3D.Resource.Downloader
{
	public class NullDownloader : IDownloader
	{
		string info;
		string file;
		ConfigError error;

		public NullDownloader (ConfigError _error,string _file,string _info)
		{
			error = _error;
			file = _file;
			info = _info;
		}

		public IEnumerator ResourceTask (string path, Action<Byte[],string> actOnComplate)
		{
			yield return null;
			actOnComplate (null, "Error from DownloaderFactory config file [" + file + "] :error = " + error.ToString() + ",info = " + info);
		}

		public ConfigError Error { get { return error; } }
	}
}

