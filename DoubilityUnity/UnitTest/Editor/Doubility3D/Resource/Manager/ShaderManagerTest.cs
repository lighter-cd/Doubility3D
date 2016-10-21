using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;
using Doubility3D.Resource.Manager;
using Doubility3D.Util;
using LitJson;

namespace UnitTest.Doubility3D.Resource.Manager
{
	public class ShaderManagerTest
	{
		string oldShaderDict;
		string fullPath;
		string where;


		[TestFixtureSetUp]
		public void Init ()
		{
			fullPath = System.IO.Path.GetFullPath (TestData.testBundle_path).Replace ('\\', '/');
			where = "Assets/Doubility3D/UnitTest/";
			DownloaderFactory.Instance.Initialize (DownloadMode.File, fullPath);
			CoroutineRunner.Instance.ActRunner = StartCoroutine;
			oldShaderDict = ShaderManager.shaderDictPath;
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			ShaderManager.Instance.DisposeBundle ();
			ShaderManager.shaderDictPath = oldShaderDict;
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
			ShaderManager.Instance.DisposeBundle ();
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
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void DictLoadError ()
		{
			ShaderManager.shaderDictPath = where + "ShaderNoDict.json";
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.DictionaryLoadError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, "shadernodict.assetbundle");
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void DictJsonError ()
		{
			ShaderManager.shaderDictPath = where + "ShaderDictError.json";
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.DictionaryJsonError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, "shaderdicterror.assetbundle");
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void ContentCheckError ()
		{
			ShaderManager.shaderDictPath = where + "ShaderContentError.json";
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.ContentCheckError);
					Assert.IsFalse(string.IsNullOrEmpty(error));
				}, "shadercontenterror.assetbundle");
			ShaderManager.Instance.DisposeBundle ();
		}

		[Test]
		public void AddAndDelete ()
		{
			ShaderManager.shaderDictPath = where + "ShaderOk.json";
			bool bRunned = false;
			ShaderManager.Instance.LoadAssetBundle (
				(result, error) => {
					Assert.IsTrue(result == ShaderLoadResult.Ok);
					RunAddAndDelete();
					bRunned = true;
				}, "shaderok.assetbundle");
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
					Assert.AreEqual (refs, j + 1);
				}

				for (int j = times-1; j >= 0; j--) {
					ShaderManager.Instance.DelShader (shader);
					int refs = ShaderManager.Instance.GetShaderRef (lst [i]);
					Assert.AreEqual (refs, j);
				}
			}
		}
	}
}
