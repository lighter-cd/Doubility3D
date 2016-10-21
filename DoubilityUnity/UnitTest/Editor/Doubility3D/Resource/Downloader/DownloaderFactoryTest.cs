using System;
using System.IO;
using System.Security.AccessControl;
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
		string oldConfigFile;

		[TearDown] 
		public void Cleanup ()
		{
			DownloaderFactory.Dispose ();
		}

		[Test]
		public void DefaultConfigValid(){
			string json = "{\"FileMode\": 1,\"URL\": \"http://127.0.0.1/\"}";
			string dir = Application.streamingAssetsPath + "/";
			string file = dir + "file_mode_test.json";

			if (!System.IO.Directory.Exists (dir)) {
				System.IO.Directory.CreateDirectory (dir);
			}

			// 创建文件
			FileStream writeStream = new FileStream (file, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
			byte[] bytes = System.Text.Encoding.Default.GetBytes ( json );
			writeStream.Write (bytes, 0, bytes.Length);
			writeStream.Dispose ();

			Assert.DoesNotThrow (new TestDelegate (() => {
				DownloaderFactory.Instance.InitializeWithConfig ("file_mode_test.json", null);
			}));
			IDownloader downloader = DownloaderFactory.Instance.Downloader;
			Assert.IsInstanceOf<WWWDownloader> (downloader);
			Assert.AreEqual ((downloader as WWWDownloader).Home, "http://127.0.0.1/");

			System.IO.File.Delete (file);
		}

		[Test]
		public void ConfigFileNotExist ()
		{
			try{
				DownloaderFactory.Instance.InitializeWithConfig ("file_mode_not_exist.json",null);
			}catch(ConfigException e){
				Assert.IsNull (DownloaderFactory.Instance.Downloader);
				Assert.AreEqual (e.Error, ConfigError.NotExist);
			}
		}
		[Test]
		public void ConfigFileIsEmpty ()
		{
			try{
				DownloaderFactory.Instance.InitializeWithConfig ("file_mode_empty.json",(f)=>{
					return "";
				});
			}catch(ConfigException e){
				Assert.IsNull (DownloaderFactory.Instance.Downloader);
				Assert.AreEqual (e.Error, ConfigError.EmptyFile);
			}
		}
		[Test]
		public void ConfigFileParseError ()
		{
			try{
				DownloaderFactory.Instance.InitializeWithConfig ("file_mode_error.json",(f)=>{
					return "I'm not a json";
				});
			}catch(ConfigException e){
				Assert.IsNull (DownloaderFactory.Instance.Downloader);
				Assert.AreEqual (e.Error, ConfigError.ErrorJson);
			}
		}

		[Test]
		public void ConfigModeNotValid ()
		{
			try{
				DownloaderFactory.Instance.InitializeWithConfig ("file_mode_not_valid.json",(f)=>{
					return "{\"FileMode\": 5,\"URL\": \"http://192.168.87.27/data\"}";
				});
			}catch(ConfigException e){
				Assert.IsNull (DownloaderFactory.Instance.Downloader);
				Assert.AreEqual (e.Error, ConfigError.ValidMode);
			}
		}


		[Test]
		public void WWWMode ()
		{
			DownloaderFactory.Instance.InitializeWithConfig ("file_mode_www.json",(f)=>{
				return "{\"FileMode\": 1,\"URL\": \"http://127.0.0.1/\"}";
			});
			IDownloader downloader = DownloaderFactory.Instance.Downloader;
			Assert.IsInstanceOf<WWWDownloader> (downloader);
			Assert.AreEqual ((downloader as WWWDownloader).Home, "http://127.0.0.1/");
		}
		[Test]
		public void WWWModeNoPostFix ()
		{
			DownloaderFactory.Instance.InitializeWithConfig ("file_mode_www_nopostfix.json",(f)=>{
				return "{\"FileMode\": 1,\"URL\": \"http://127.0.0.1\"}";
			});
			IDownloader downloader = DownloaderFactory.Instance.Downloader;
			Assert.IsInstanceOf<WWWDownloader> (downloader);
			Assert.AreEqual ((downloader as WWWDownloader).Home, "http://127.0.0.1/");
		}
		[Test]
		public void FileModeTest ()
		{
			DownloaderFactory.Instance.InitializeWithConfig ("file_mode_file.json",(f)=>{
				return "{\"FileMode\": 0}";
			});
			IDownloader downloader = DownloaderFactory.Instance.Downloader;
			Assert.IsInstanceOf<FileDownloader> (downloader);
		}
		[Test]
		public void FileModeEnvVarTest(){
			bool exist = true;
			string home = Environment.GetEnvironmentVariable ("DOUBILITY_HOME", EnvironmentVariableTarget.User);
			if (string.IsNullOrEmpty (home)) {
				Environment.SetEnvironmentVariable ("DOUBILITY_HOME", "Z:\\ThePath", EnvironmentVariableTarget.User);
				exist = false;
			}
			home = home.Replace ('\\', '/');
			if (home [home.Length - 1] != '/') {
				home += "/";
			}

			DownloaderFactory.Instance.Initialize (DownloadMode.File, "<DOUBILITY_HOME>");
			IDownloader downloader = DownloaderFactory.Instance.Downloader;
			Assert.IsInstanceOf<FileDownloader> (downloader);
			Assert.AreEqual((downloader as FileDownloader).Home,home);

			if (!exist) {
				Environment.SetEnvironmentVariable ("DOUBILITY_HOME", null, EnvironmentVariableTarget.User);
			}
		}
		[Test]
		public void PacketMode ()
		{
			DownloaderFactory.Instance.InitializeWithConfig ("file_mode_packet.json",(f)=>{
				return "{\"FileMode\": 2}";
			});
			IDownloader downloader = DownloaderFactory.Instance.Downloader;
			Assert.IsInstanceOf<PacketDownloader> (downloader);
		}
	}
}
