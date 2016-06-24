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

		string url;
		IDownloader downloader;

		[TestFixtureSetUp]
		public void Init ()
		{
			url = System.IO.Path.GetFullPath (TestData.testResource_path);
			url = "file:///" + url.Replace ('\\', '/');
			downloader = DownloaderFactory.CreateWWWDownloader (url);
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			downloader = null;
		}

		[Test]		
		public void HomeVar ()
		{
			Assert.IsInstanceOf<WWWDownloader> (downloader);

			Assert.AreEqual ((downloader as WWWDownloader).Home, url);

			string home = (downloader as WWWDownloader).Home;
			Assert.IsTrue (home [home.Length - 1] == '/');
		}

		[Test]		
		public void ReadError ()
		{
			WWWDownloader fd = downloader as WWWDownloader;
			Assert.IsNotNull (fd);

			CoroutineTest.Run (fd.ResourceTask ("NotExistFile.Dat", (bytes, error) => {
				Assert.IsNull (bytes);
				Assert.IsFalse (string.IsNullOrEmpty (error));
			}));
		}

		[Test]		
		public void DataValid ()
		{
			// 随机产生bytes数组
			// 写入文件作为测试数据
			byte[] bytes = RandomData.Build (2048, 512);

			// 写入文件
			const string targetPath = "WWWDownloader.Dat";
			bool writed = RandomData.WriteToFile (bytes, TestData.testResource_path + targetPath);
			Assert.IsTrue (writed);

			WWWDownloader fd = downloader as WWWDownloader;
			Assert.IsNotNull (fd);
			CoroutineTest.Run (
				fd.ResourceTask (targetPath + "?" + System.Environment.TickCount.ToString(), (results, error) => {
					Assert.IsNotNull (results);
					Assert.AreEqual (bytes.Length, results.Length);
					Assert.IsTrue (string.IsNullOrEmpty (error));
					for (int i = 0; i < bytes.Length; i++) {
						Assert.AreEqual (bytes [i], results [i]);
					}

					// 删除文件
					System.IO.File.Delete (TestData.testResource_path + targetPath);
				})
			);
		}
	}
}