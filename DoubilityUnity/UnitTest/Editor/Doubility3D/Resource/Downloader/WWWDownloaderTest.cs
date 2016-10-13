using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	/// <summary>
	/// WWW downloader test.
	/// 使用了 files:/// 模拟了 www ,不需要真正的 http 服务器。
	/// </summary>
	public class WWWDownloaderTest
	{
		string url;
		IDownloader downloader;

		/// <summary>
		/// Init this instance. 没有使用 json文件做配置，直接传入了 url.
		/// </summary>
		[TestFixtureSetUp]
		public void Init ()
		{
			url = System.IO.Path.GetFullPath (TestData.testResource_path);
			url = "file:///" + url.Replace ('\\', '/');
			DownloaderFactory.Instance.CreateWWWDownloader (url);
			downloader = DownloaderFactory.Instance.Downloader;
		}
		[TestFixtureTearDown]
		public void Clear ()
		{
			DownloaderFactory.Dispose();
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

			bool runned = false;
			IEnumerator enumerator = fd.ResourceTask ("NotExistFile.Dat", (bytes, error) => {
				Assert.IsNull (bytes);
				Assert.IsFalse (string.IsNullOrEmpty (error));
				runned = true;
			});
			bool completed = enumerator.RunCoroutineWithoutYields (int.MaxValue);
			Assert.IsTrue (completed);
			Assert.IsTrue (runned);
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

			bool runned = false;
			IEnumerator enumerator = fd.ResourceTask (targetPath + "?" + System.Environment.TickCount.ToString (), (results, error) => {
				Assert.IsNotNull (results);
				Assert.AreEqual (bytes.Length, results.Length);
				Assert.IsTrue (string.IsNullOrEmpty (error));
				for (int i = 0; i < bytes.Length; i++) {
					Assert.AreEqual (bytes [i], results [i]);
				}

				// 删除文件
				System.IO.File.Delete (TestData.testResource_path + targetPath);
				runned = true;
			});
			bool completed = enumerator.RunCoroutineWithoutYields (int.MaxValue);
			Assert.IsTrue (completed);
			Assert.IsTrue (runned);
		}
	}
}