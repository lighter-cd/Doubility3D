using System;
using LitJson;
using UnityEngine;
using Doubility3D.Resource.Manager;

namespace Doubility3D.Resource.Downloader
{
	public enum DownloadMode {
		File = 0,
		WWW,
		Packet
	}
	public class DownloadConfig {
		public DownloadMode FileMode;
		public string URL;
	}

	public class DownloaderFactory
	{
		DownloadConfig config;
		private DownloaderFactory ()
		{
			TextAsset asset = Resources.Load<TextAsset> ("file_mode");
			if (asset != null) {
				config = JsonMapper.ToObject<DownloadConfig> (asset.text);
			} else {
				throw new Exception ("Cannot load file_mode.json from Resources folder.");
			}
		}

		static private DownloaderFactory _instance = null;

		static public DownloaderFactory Instance{
			get{ 
				if (_instance == null) {
					_instance = new DownloaderFactory ();
				}
				return _instance;
			}
		}

		public IDownloader Create(){
			switch (config.FileMode) {
			case DownloadMode.File:
				return new FileDownloader ();
			case DownloadMode.WWW:
				return new WWWDownloader (config.URL);
			case DownloadMode.Packet:
				return null;
			}
			return null;
		}
	}
}

