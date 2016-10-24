using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnitTest.Doubility3D.Resource.Downloader;
using UnitTest.Doubility3D.Resource.Unserializing;
using Doubility3D.Util;
using Doubility3D.Resource.Manager;

namespace UnitTest.Doubility3D.Resource.Manager
{
	class MockResourceManager : IResourceManager{

		Dictionary<string,ResourceRef> dictResources;

		public MockResourceManager(Dictionary<string,ResourceRef> dict){
			dictResources = dict;
		}

		public ResourceRef[] addResources (string[] urls, int[] priorities, bool bAsync, Action<ResourceRef[]> actComplate, Action<Exception> actError){
			ResourceRef[] refs = new ResourceRef[urls.Length];
			bool error = false;
			for (int i = 0; i < urls.Length; i++) {
				if (!dictResources.ContainsKey (urls [i])) {
					refs [i] = new ResourceRef (urls [i], this);
					dictResources.Add (urls [i], refs [i]);
					refs [i].Start ();
				} else {
					refs [i] = dictResources [urls [i]];
					refs [i].AddRefs ();
				}

				if (!string.IsNullOrEmpty (refs [i].Error) && !error) {
					error = true;
				}
			}
			if (!error) {
				actComplate (refs);
			} else {
				actError (new Exception (""));
			}
			return refs;
		}
		public void delResources (string[] urls){
			for (int i = 0; i < urls.Length; i++) {
				if (dictResources.ContainsKey (urls [i])) {
					dictResources [urls [i]].DelRefs ();
					if (dictResources [urls [i]].Refs == 0) {
						dictResources.Remove (urls [i]);
					}
				}
			}
		}
	}


	[TestFixture]
	public class ResourceRefTest
	{
		// 数据：无依赖 有依赖(1层，2层，3层), 依赖无直接引用的，依赖也被直接引用的
		TestParam[] _params  = {
			new TestParam("NDObject01",0,null),
			new TestParam("NDObject02",3,null),
			new TestParam("NDObject03",2,null),
			new TestParam("NDObject04",4,new string[]{"NDObject01"}),
			new TestParam("NDObject05",8,new string[]{"NDObject01","NDObject02"}),
			new TestParam("NDObject06",7,new string[]{"NDObject01","NDObject02","NDObject03"}),
			new TestParam("NDObject07",9,new string[]{"NDObject06"}),
			new TestParam("NDObject08",6,new string[]{"NDObject07"}),
		};


		MockDownloader downloader = new MockDownloader ();
		// 这些文件名解析时将会出错。
		HashSet<string> hashSetErrorUnserializer = new HashSet<string> ();


		Dictionary<string,TestParam> dictParams = new Dictionary<string, TestParam> ();
		Dictionary<string,ResourceRef> dictResources = new Dictionary<string, ResourceRef> ();
		MockResourceManager resourceManager;

		[TestFixtureSetUp]
		public void Init ()
		{
			resourceManager = new MockResourceManager(dictResources);
			for (int i = 0; i < _params.Length; i++) {
				dictParams.Add (_params [i].path, _params [i]);
			}

			MockDownloaderFactory.Initialize (downloader);

			MockUnserializerFactory.Initialize ((bytes) => {
				string path = System.Text.Encoding.Default.GetString (bytes);
				// 文件名被加了一个 .root/的前缀。
				path = System.IO.Path.GetFileName (path);
				if(!hashSetErrorUnserializer.Contains(path)){
					MockResourceObject obj = new MockResourceObject (dictParams [path]);
					return obj;
				}
				return null;
			});
			CoroutineRunner.Instance.ActRunner = (e) => {
				bool completed = e.RunCoroutineWithoutYields (int.MaxValue);
				Assert.IsTrue (completed);
			};
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			MockUnserializerFactory.CleanUp ();
			dictParams.Clear ();
			resourceManager = null;
		}
		[TearDown]
		public void ClearManager ()
		{
			dictResources.Clear ();
		}
		[Test]
		public void NoDependencs ()
		{
			ResourceRef refs = new ResourceRef (_params [0].path, resourceManager);
			dictResources.Add (_params [0].path, refs);
			refs.Start ();

			Assert.AreEqual (1, dictResources.Count);
			Assert.IsTrue(dictResources.ContainsKey(_params [0].path));
			Assert.IsTrue (string.IsNullOrEmpty(refs.Error));
		}
		[Test]
		public void OneDependencs ()
		{
			ResourceRef refs = new ResourceRef (_params [3].path, resourceManager);
			dictResources.Add (_params [3].path, refs);
			refs.Start ();

			Assert.AreEqual (2, dictResources.Count);
			Assert.IsTrue(dictResources.ContainsKey(_params [3].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [0].path));
			Assert.AreEqual(1,dictResources[_params [0].path].Refs);
			Assert.IsTrue (string.IsNullOrEmpty(refs.Error));
		}
		[Test]
		public void TwoDependencs ()
		{
			ResourceRef refs = new ResourceRef (_params [4].path, resourceManager);
			dictResources.Add (_params [4].path, refs);
			refs.Start ();

			Assert.AreEqual (3, dictResources.Count);
			Assert.IsTrue(dictResources.ContainsKey(_params [4].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [0].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [1].path));
			Assert.AreEqual(1,dictResources[_params [0].path].Refs);
			Assert.AreEqual(1,dictResources[_params [1].path].Refs);
			Assert.IsTrue (string.IsNullOrEmpty(refs.Error));
		}
		[Test]
		public void ThreeDependencs ()
		{
			ResourceRef refs = new ResourceRef (_params [5].path, resourceManager);
			dictResources.Add (_params [5].path, refs);
			refs.Start ();

			Assert.AreEqual (4, dictResources.Count);
			Assert.IsTrue(dictResources.ContainsKey(_params [5].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [0].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [1].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [2].path));
			Assert.AreEqual(1,dictResources[_params [0].path].Refs);
			Assert.AreEqual(1,dictResources[_params [1].path].Refs);
			Assert.AreEqual(1,dictResources[_params [2].path].Refs);
			Assert.IsTrue (string.IsNullOrEmpty(refs.Error));
		}

		[Test]
		public void Level2Dependencs ()
		{
			ResourceRef refs = new ResourceRef (_params [6].path, resourceManager);
			dictResources.Add (_params [6].path, refs);
			refs.Start ();

			Assert.AreEqual (5, dictResources.Count);
			Assert.IsTrue(dictResources.ContainsKey(_params [6].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [0].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [1].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [2].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [5].path));
			Assert.AreEqual(1,dictResources[_params [0].path].Refs);
			Assert.AreEqual(1,dictResources[_params [1].path].Refs);
			Assert.AreEqual(1,dictResources[_params [2].path].Refs);
			Assert.AreEqual(1,dictResources[_params [5].path].Refs);
			Assert.IsTrue (string.IsNullOrEmpty(refs.Error));
		}
		[Test]
		public void Level3Dependencs ()
		{
			ResourceRef refs = new ResourceRef (_params [7].path, resourceManager);
			dictResources.Add (_params [7].path, refs);
			refs.Start ();

			Assert.AreEqual (6, dictResources.Count);
			Assert.IsTrue(dictResources.ContainsKey(_params [7].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [0].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [1].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [2].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [5].path));
			Assert.IsTrue(dictResources.ContainsKey(_params [6].path));
			Assert.AreEqual(1,dictResources[_params [0].path].Refs);
			Assert.AreEqual(1,dictResources[_params [1].path].Refs);
			Assert.AreEqual(1,dictResources[_params [2].path].Refs);
			Assert.AreEqual(1,dictResources[_params [5].path].Refs);
			Assert.AreEqual(1,dictResources[_params [6].path].Refs);
			Assert.IsTrue (string.IsNullOrEmpty(refs.Error));
		}
		[Test]
		public void DownloadError ()
		{
			downloader.AddErrorFile (_params [0].path);
			ResourceRef refs = new ResourceRef (_params [0].path, resourceManager);
			dictResources.Add (_params [0].path, refs);
			refs.Start ();
			Assert.IsFalse (string.IsNullOrEmpty(refs.Error));
			downloader.ClearErrorFiles ();
		}
		[Test]
		public void PaserError ()
		{
			hashSetErrorUnserializer.Add(_params [0].path);
			ResourceRef refs = new ResourceRef (_params [0].path, resourceManager);
			dictResources.Add (_params [0].path, refs);
			refs.Start ();
			Assert.IsFalse (string.IsNullOrEmpty(refs.Error));
			hashSetErrorUnserializer.Clear ();
		}
		[Test]
		public void DependencsError ()
		{
			downloader.AddErrorFile (_params [0].path);
			ResourceRef refs = new ResourceRef (_params [4].path, resourceManager);
			dictResources.Add (_params [4].path, refs);
			refs.Start ();
			Assert.IsFalse (string.IsNullOrEmpty(refs.Error));
			downloader.ClearErrorFiles ();			
		}
	}
}