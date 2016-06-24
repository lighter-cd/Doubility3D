using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;
using Doubility3D.Resource.Manager;

namespace UnitTest.Doubility3D.Resource.Manager
{
	public class ShaderManagerTest
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
			oldHome = Environment.GetEnvironmentVariable ("DOUBILITY_HOME", EnvironmentVariableTarget.Machine);
			oldConfigFile = DownloaderFactory.configFile;
			oldFuncTextAssetReader = DownloaderFactory.funcTextAssetReader;
			DownloaderFactory.funcTextAssetReader = ReadTestConfig;			
			DownloaderFactory.configFile = "file_mode_file.json";

			fullPath = System.IO.Path.GetFullPath (TestData.testResource_path);

			Environment.SetEnvironmentVariable ("DOUBILITY_HOME", fullPath, EnvironmentVariableTarget.Machine);
			downloader = DownloaderFactory.Instance.Create ();
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			Environment.SetEnvironmentVariable ("DOUBILITY_HOME", oldHome, EnvironmentVariableTarget.Machine);
			DownloaderFactory.configFile = oldConfigFile;
			DownloaderFactory.funcTextAssetReader = oldFuncTextAssetReader;
			DownloaderFactory.Dispose ();
		}

		[Test]
		public void DownloadError ()
		{
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.BundleDownloadError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, TestData.testBundle_path + "ShaderNotExist.assetbundle");
			TaskManagerTest.Instance.Run ();
		}

		[Test]
		public void BundleLoadError ()
		{
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.BundleLoadError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, TestData.testBundle_path + "ShaderErrorBundle.assetbundle");
			TaskManagerTest.Instance.Run ();
		}

		[Test]
		public void DictLoadError ()
		{
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.DictionaryLoadError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, TestData.testBundle_path + "ShaderNoDict.assetbundle");
			TaskManagerTest.Instance.Run ();
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void DictJsonError ()
		{
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.DictionaryJsonError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, TestData.testBundle_path + "ShaderDictError.assetbundle");
			TaskManagerTest.Instance.Run ();
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void ContentCheckError ()
		{
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.ContentCheckError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, TestData.testBundle_path + "ShaderContentError.assetbundle");
			TaskManagerTest.Instance.Run ();
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void AddAndDelete ()
		{
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.Ok);
					RunAddAndDelete();
				}, TestData.testBundle_path + "ShaderOK.assetbundle");
			TaskManagerTest.Instance.Run ();
			ShaderManager.Instance.DisposeBundle ();
		}
		void RunAddAndDelete(){
			string[] lst = ShaderManager.Instance.GetShaderList ();

			for (int i = 0; i < lst.Length; i++) {
				int times = 10 + RandomData.Random.Next (0, 5);

				Shader shader = null;
				for (int j = 0; j < times; j++) {
					shader = ShaderManager.Instance.AddShader (lst [i]);
					Assert.IsNotNull (shader);
					int refs = ShaderManager.Instance.GetShaderRef (lst [i]);
					Assert.Equals (refs, j + 1);
				}

				for (int j = times; j >= 0; j--) {
					ShaderManager.Instance.DelShader (shader);
					int refs = ShaderManager.Instance.GetShaderRef (lst [i]);
					Assert.Equals (refs, j);
				}
			}
		}
	}
}
