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

	public class ConfigException : Exception
	{
		ConfigError error;
		public ConfigException(ConfigError _error) : base(_error.ToString()){
			error = _error;
		}
		public ConfigException(ConfigError _error, Exception e) : base (e.GetType().FullName + ":" + e.Message,e){
			error = _error;
		}
		public ConfigError Error { get { return error; } }
	}

	public class DownloaderFactory
	{
		DownloadConfig config;
		static public string configFile = "file_mode";
		static public Func<string,TextAsset> funcTextAssetReader = Resources.Load<TextAsset>;

		IDownloader downloader;

		private DownloaderFactory ()
		{
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
			if (_instance != null && _instance.Downloader != null) {
				_instance.Downloader.Dispose ();
			}
			_instance = null;
		}

		public void Initialize ()
		{
			TextAsset asset = funcTextAssetReader (configFile);
			if (asset != null) {
				if (!string.IsNullOrEmpty (asset.text)) {
					try {
						config = JsonMapper.ToObject<DownloadConfig> (asset.text);
					} catch (Exception e) {
						throw(new ConfigException ( ConfigError.ErrorJson, e));
					}
				} else {
					throw(new ConfigException ( ConfigError.EmptyFile));
				}
			} else {
				throw(new ConfigException ( ConfigError.NotExist));
			}

			if (config != null && downloader == null) {
				switch (config.FileMode) {
				case DownloadMode.File:
					downloader = new FileDownloader ();
					break;
				case DownloadMode.WWW:
					downloader = new WWWDownloader (config.URL);
					break;
				case DownloadMode.Packet:
					downloader = new PacketDownloader (config.URL);
					break;
				default:
					throw(new ConfigException (ConfigError.ValidMode));
				}
			}
		}

		public IDownloader Downloader {get{ return downloader;}}

		public void CreateWWWDownloader (string url)
		{
			downloader = new WWWDownloader (url);
		}
		public void CreatePacketDownloader (string name)
		{
			downloader =  new PacketDownloader (name);
		}
	}
}

