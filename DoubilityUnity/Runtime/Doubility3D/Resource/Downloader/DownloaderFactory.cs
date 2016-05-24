using System;
using Doubility3D.Resource.Manager;

namespace Doubility3D.Resource.Downloader
{
	public class DownloaderFactory
	{
		private DownloaderFactory ()
		{
		}

		static private DownloaderFactory _instance = null;

		static public DownloaderFactory Instance{
			get{ 
				if (_instance == null) {
					_instance = new DownloaderFactory ();
				}
				return Instance;
			}
		}

		static public ResourceMode resourceMode = ResourceMode.FromPacket;

		public IDownloader Create(){
			return null;
		}
	}
}

