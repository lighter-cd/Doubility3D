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
		private IDownloader downloader;

		public string UrlFixer = null;

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

		public void InitializeWithConfig(string configFile,Func<string,string> funcTextAssetReader = null)
		{
			if (funcTextAssetReader == null) {
				funcTextAssetReader = (configPath) => {
					try{
						return System.IO.File.ReadAllText(Application.streamingAssetsPath + "/" + configPath);
					}catch(Exception e){
						throw(new ConfigException (ConfigError.NotExist,e));
					}
				};
			}

			string json = funcTextAssetReader (configFile);
			DownloadConfig config;

			if (!string.IsNullOrEmpty (json)) {
				try {
					config = JsonMapper.ToObject<DownloadConfig> (json);
				} catch (Exception e) {
					throw(new ConfigException (ConfigError.ErrorJson, e));
				}
			} else {
				throw(new ConfigException (ConfigError.EmptyFile));
			}
			Initialize(config.FileMode,config.URL);
		}

		public void Initialize (DownloadMode fileMode,string url)
		{

			if (downloader == null) {
				switch (fileMode) {
				case DownloadMode.File:
					downloader = new FileDownloader (PostfixUrl(url));
					break;
				case DownloadMode.WWW:
					downloader = new WWWDownloader (PostfixUrl(url));
					break;
				case DownloadMode.Packet:
					downloader = new PacketDownloader (PrefixUrl(url));
					break;
				default:
					throw(new ConfigException (ConfigError.ValidMode));
				}
			}
		}

		private string PostfixUrl(string url){
			if (!string.IsNullOrEmpty (UrlFixer)) {
				string fixer = UrlFixer;
				if (fixer [fixer.Length - 1] != '/') {
					fixer += "/";
				}
				url += fixer;
			}
			return url;
		}
		private string PrefixUrl(string url){
			if (!string.IsNullOrEmpty (UrlFixer)) {
				string fixer = UrlFixer;
				if (fixer [fixer.Length - 1] != '/') {
					fixer += "/";
				}
				url = fixer + url;
			}
			return url;
		}

		public IDownloader Downloader {get{ return downloader;}}
	}
}

