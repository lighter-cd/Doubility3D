using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;
using Doubility3D.Resource.Manager;
using LitJson;

namespace UnitTest.Doubility3D.Resource.Manager
{
	public class ShaderManagerTest
	{
		string oldConfigFile;
		string oldHome;

		string fullPath;

		Action<IEnumerator> actOldStartCoroutine = null;

		[TestFixtureSetUp]
		public void Init ()
		{
			oldHome = Environment.GetEnvironmentVariable ("DOUBILITY_HOME", EnvironmentVariableTarget.User);
			fullPath = System.IO.Path.GetFullPath (TestData.testBundle_path);

			Environment.SetEnvironmentVariable ("DOUBILITY_HOME", fullPath, EnvironmentVariableTarget.User);
			DownloaderFactory.Instance.Initialize (DownloadMode.File, null);

			actOldStartCoroutine = ShaderManager.actStartCoroutine;
			ShaderManager.actStartCoroutine = StartCoroutine;
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			ShaderManager.actStartCoroutine = actOldStartCoroutine;
			Environment.SetEnvironmentVariable ("DOUBILITY_HOME", oldHome, EnvironmentVariableTarget.User);
			DownloaderFactory.Dispose ();
		}

		void StartCoroutine(IEnumerator e){
			bool completed = e.RunCoroutineWithoutYields (int.MaxValue);
			Assert.IsTrue (completed);
		}

		[Test]
		public void DownloadError ()
		{
			bool bRunned = false;
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.BundleDownloadError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
					bRunned = true;
				}, "ShaderNotExist.assetbundle");
			Assert.IsTrue (bRunned);
		}

		[Test]
		public void BundleLoadError ()
		{
			// 随机产生bytes数组
			// 写入文件作为测试数据
			byte[] bytes = RandomData.Build (2048, 512);

			// 写入文件
			const string targetPath = "ShaderErrorBundle.assetbundle";
			bool writed = RandomData.WriteToFile (bytes, fullPath + targetPath);
			Assert.IsTrue (writed);


			bool bRunned = false;
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.BundleLoadError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
					bRunned = true;
				}, "ShaderErrorBundle.assetbundle");
			Assert.IsTrue (bRunned);

			System.IO.File.Delete (fullPath + targetPath);
		}

		[Test]
		public void DictLoadError ()
		{
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.DictionaryLoadError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, "ShaderNoDict.assetbundle");
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void DictJsonError ()
		{
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.DictionaryJsonError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, "ShaderDictError.assetbundle");
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void ContentCheckError ()
		{
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.ContentCheckError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, "ShaderContentError.assetbundle");
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void AddAndDelete ()
		{
			bool bRunned = false;
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.Ok);
					RunAddAndDelete();
					bRunned = true;
				}, "ShaderOK.bundle");
			ShaderManager.Instance.DisposeBundle ();
			Assert.IsTrue (bRunned);
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

		static void DoIt (string bundleName)
		{
			string[] _files = System.IO.Directory.GetFiles ("Assets/Doubility3D/CoreData", "*.shader", System.IO.SearchOption.AllDirectories);

			AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
			buildMap [0].assetBundleName = bundleName;
			buildMap [0].assetNames = new string[1];
			buildMap [0].assetNames[0] = _files [0].Replace ('\\', '/');


			Dictionary<string,string> dictShaderName2Path = new Dictionary<string, string> ();
			Shader shader = AssetDatabase.LoadAssetAtPath<Shader> (buildMap [0].assetNames[0]);
			dictShaderName2Path.Add (shader.name, _files [0]);


			/*string jsonString = JsonMapper.ToJson (dictShaderName2Path);
			System.IO.File.WriteAllText (ShaderDictionary.Path, jsonString);

			Array.Resize<string> (ref buildMap [0].assetNames, buildMap [0].assetNames.Length + 1);
			buildMap [0].assetNames [buildMap [0].assetNames.Length - 1] = ShaderDictionary.Path;


			BuildPipeline.BuildAssetBundles (outputFolder, buildMap, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);	*/
		}
	}
}
