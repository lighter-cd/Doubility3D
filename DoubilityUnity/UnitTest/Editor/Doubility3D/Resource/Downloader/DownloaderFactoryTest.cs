using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;
using Doubility3D.Util;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	[TestFixture]
	public class DownloaderFactoryTest
	{
		TextAsset ReadTestConfig(string file){
			return AssetDatabase.LoadAssetAtPath<TextAsset> (TestData.testConfig_path + file);
		}

		string oldConfigFile;
		Func<string,TextAsset> oldFuncTextAssetReader;

		[TestFixtureSetUp]
		public void Init(){
			oldConfigFile = DownloaderFactory.configFile;
			oldFuncTextAssetReader = DownloaderFactory.funcTextAssetReader;
			DownloaderFactory.funcTextAssetReader = ReadTestConfig;			
		}
		[TestFixtureTearDown]
		public void Clear(){
			DownloaderFactory.configFile = oldConfigFile;
			DownloaderFactory.funcTextAssetReader = oldFuncTextAssetReader;			
		}

		[TearDown] 
		public void Cleanup ()
		{
			DownloaderFactory.Dispose ();
		}

		[Test]
		public void ConfigFileNotExist ()
		{
			DownloaderFactory.configFile = "file_mode_not_exist.json";
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			Assert.IsInstanceOf<NullDownloader> (downloader);
			Assert.AreEqual ((downloader as NullDownloader).Error, ConfigError.NotExist);
		}
		[Test]
		public void ConfigFileIsEmpty ()
		{
			DownloaderFactory.configFile = "file_mode_empty.json";
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			Assert.IsInstanceOf<NullDownloader> (downloader);
			Assert.AreEqual ((downloader as NullDownloader).Error, ConfigError.EmptyFile);
		}
		[Test]
		public void ConfigFileParseError ()
		{
			DownloaderFactory.configFile = "file_mode_error.json";
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			Assert.IsInstanceOf<NullDownloader> (downloader);
			Assert.AreEqual ((downloader as NullDownloader).Error, ConfigError.ErrorJson);
		}

		[Test]
		public void ConfigModeNotValid ()
		{
			DownloaderFactory.configFile = "file_mode_not_valid.json";
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			Assert.IsInstanceOf<NullDownloader> (downloader);
			Assert.AreEqual ((downloader as NullDownloader).Error, ConfigError.ValidMode);
		}


		[Test]
		public void WWWMode ()
		{
			DownloaderFactory.configFile = "file_mode_www.json";
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			Assert.IsInstanceOf<WWWDownloader> (downloader);
			Assert.AreEqual ((downloader as WWWDownloader).Home, "http://127.0.0.1/");
		}
		[Test]
		public void WWWModeNoPostFix ()
		{
			DownloaderFactory.configFile = "file_mode_www_nopostfix.json";
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			Assert.IsInstanceOf<WWWDownloader> (downloader);
			Assert.AreEqual ((downloader as WWWDownloader).Home, "http://127.0.0.1/");
		}
		[Test]
		public void FileMode ()
		{
			DownloaderFactory.configFile = "file_mode_file.json";
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			Assert.IsInstanceOf<FileDownloader> (downloader);
		}
		[Test]
		public void PacketMode ()
		{
			DownloaderFactory.configFile = "file_mode_packet.json";
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			Assert.IsInstanceOf<PacketDownloader> (downloader);
		}
	}
}
