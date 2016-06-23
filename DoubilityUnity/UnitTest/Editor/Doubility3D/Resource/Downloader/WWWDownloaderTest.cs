using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	[TestFixture]
	public class WWWDownloaderTest
	{
		TextAsset ReadTestConfig (string file)
		{
			return AssetDatabase.LoadAssetAtPath<TextAsset> (TestData.testConfig_path + file);
		}

		string oldConfigFile;
		Func<string,TextAsset> oldFuncTextAssetReader;

		string fullPath;
		IDownloader downloader;

		[TestFixtureSetUp]
		public void Init ()
		{
			oldConfigFile = DownloaderFactory.configFile;
			oldFuncTextAssetReader = DownloaderFactory.funcTextAssetReader;
			DownloaderFactory.funcTextAssetReader = ReadTestConfig;			
			DownloaderFactory.configFile = "file_mode_www.json";

			fullPath = System.IO.Path.GetFullPath (TestData.testResource_path);
			downloader = DownloaderFactory.Instance.Create ();
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			DownloaderFactory.configFile = oldConfigFile;
			DownloaderFactory.funcTextAssetReader = oldFuncTextAssetReader;
			DownloaderFactory.Dispose ();
		}

		[Test]		
		public void FileReadError ()
		{

		}

		[Test]		
		public void FileDataValid ()
		{

		}
	}
}