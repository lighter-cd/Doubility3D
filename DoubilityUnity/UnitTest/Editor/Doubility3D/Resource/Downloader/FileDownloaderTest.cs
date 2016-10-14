using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	[TestFixture]
	public class FileDownloaderTest
	{
		TextAsset ReadTestConfig (string file)
		{
			return AssetDatabase.LoadAssetAtPath<TextAsset> (TestData.testConfig_path + file);
		}

		string oldConfigFile;
		Func<string,TextAsset> oldFuncTextAssetReader;
		string oldHome;

		string fullPath;
		IDownloader downloader;

		[TestFixtureSetUp]
		public void Init ()
		{
			oldHome = Environment.GetEnvironmentVariable ("DOUBILITY_HOME", EnvironmentVariableTarget.User);
			fullPath = System.IO.Path.GetFullPath (TestData.testResource_path);

			Assert.DoesNotThrow (new TestDelegate (() => {
				Environment.SetEnvironmentVariable ("DOUBILITY_HOME", fullPath, EnvironmentVariableTarget.User);
				DownloaderFactory.Instance.Initialize (DownloadMode.File,"");
			}));
			downloader = DownloaderFactory.Instance.Downloader;
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			Environment.SetEnvironmentVariable ("DOUBILITY_HOME", oldHome, EnvironmentVariableTarget.User);
			DownloaderFactory.Dispose ();
			downloader = null;
		}

		[Test]		
		public void HomeVar ()
		{
			Assert.IsInstanceOf<FileDownloader> (downloader);

			string fullUnixPath = fullPath.Replace ('\\', '/');
			Assert.AreEqual ((downloader as FileDownloader).Home, fullUnixPath);

			string home = (downloader as FileDownloader).Home;
			Assert.IsTrue (home [home.Length - 1] == '/');
		}

		[Test]		
		public void ReadError ()
		{
			FileDownloader fd = downloader as FileDownloader;
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
			const string targetPath = "FileDownloader.Dat";
			bool writed = RandomData.WriteToFile (bytes, TestData.testResource_path + targetPath);
			Assert.IsTrue (writed);

			FileDownloader fd = downloader as FileDownloader;
			Assert.IsNotNull (fd);
			bool runned = false;
			IEnumerator enumerator = fd.ResourceTask (targetPath, (results, error) => {
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