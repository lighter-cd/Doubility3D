using System;
using LitJson;
using UnityEngine;
using Doubility3D.Resource.Manager;

namespace Doubility3D.Resource.Downloader
{
	public enum DownloadMode
	{
		File = 0,
		WWW,
		Packet
	}

	public class DownloadConfig
	{
		public DownloadMode FileMode;
		public string URL;
	}

	public enum ConfigError
	{
		NoError,
		NotExist,
		EmptyFile,
		ErrorJson,
		ValidMode,
	}

	public class DownloaderFactory
	{
		DownloadConfig config;
		static public string configFile = "file_mode";
		static public Func<string,TextAsset> funcTextAssetReader = Resources.Load<TextAsset>;

		ConfigError error = ConfigError.NoError;
		string errorMsg;

		private DownloaderFactory ()
		{
			TextAsset asset = funcTextAssetReader (configFile);
			if (asset != null) {
				if (!string.IsNullOrEmpty (asset.text)) {
					try {
						config = JsonMapper.ToObject<DownloadConfig> (asset.text);
					} catch (Exception e) {
						error = ConfigError.ErrorJson;
						errorMsg = e.Message;
					}
				} else {
					error = ConfigError.EmptyFile;
				}
			} else {
				error = ConfigError.NotExist;
			}
		}

		static private DownloaderFactory _instance = null;

		static public DownloaderFactory Instance {
			get { 
				if (_instance == null) {
					_instance = new DownloaderFactory ();
				}
				return _instance;
			}
		}

		static public void Dispose ()
		{
			_instance = null;
		}

		public IDownloader Create ()
		{
			if (config != null) {
				switch (config.FileMode) {
				case DownloadMode.File:
					return new FileDownloader ();
				case DownloadMode.WWW:
					return new WWWDownloader (config.URL);
				case DownloadMode.Packet:
					return new PacketDownloader (config.URL);
				default:
					return new NullDownloader (ConfigError.ValidMode, configFile, config.FileMode.ToString ());
				}
			}
			return new NullDownloader (error, configFile, errorMsg);
		}

		static public IDownloader CreateWWWDownloader (string url)
		{
			return new WWWDownloader (url);
		}
		static public IDownloader CreatePacketDownloader (string name)
		{
			return new PacketDownloader (name);
		}
	}
}

