using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;
using Doubility3D.Resource.ResourceObj;
using Doubility3D.Resource.Manager;

namespace UnitTest.Doubility3D.Resource.Manager
{
	class TestParam {
		public string path;
		public string[] dependencs;
		public int priority;

		public TestParam(string p, int pri,string[] ds){
			path = p;
			dependencs = ds;
			priority = pri;
		}
	};

	class MockDownloader : IDownloader
	{
		public IEnumerator ResourceTask (string path, Action<Byte[],string> actOnComplate)
		{
			yield return null;
			actOnComplate (System.Text.Encoding.Default.GetBytes (path), null);
		}

		public void Dispose ()
		{
		}
	}

	class MockResourceObject : ResourceObject
	{
		private List<string> dependencesPath = new List<string> (8);
		private bool allDepndencesGotted;
		public MockResourceObject (TestParam param)
		{
			unity3dObject = new GameObject(param.path);
			if (param.dependencs != null) {
				dependencesPath.AddRange (param.dependencs);
			}
		}
		override public string[] DependencePathes {	get{return dependencesPath.ToArray();}}
		override public void OnDependencesFinished(){
			allDepndencesGotted = true;
			List<string>.Enumerator e = dependencesPath.GetEnumerator ();
			while (e.MoveNext ()) {
				ResourceRef _ref = ResourceManager.Instance.getResource (e.Current);
				if (_ref == null) {
					allDepndencesGotted = false;
					break;
				}
			}
		}
		public bool AllDepndencesGotted { get { return allDepndencesGotted; } }
	}


	public class ResourceManagerTest
	{
		Func<IDownloader> funcOldDownloader;
		Func<byte[],ResourceObject> funcOldUnserializer;
		Action<IEnumerator> actOldStartCoroutine;

		MockDownloader downloader = new MockDownloader ();

		// 数据：无依赖 有依赖(1层，2层，3层), 依赖无直接引用的，依赖也被直接引用的
		// 手写 json ?
		/*TestParam[] _params  = {
			new TestParam("NDObject01",0,null),
			new TestParam("NDObject02",3,null),
			new TestParam("NDObject03",2,null),
			new TestParam("NDObject04",4,new string[]{"NDObject01"}),
			new TestParam("NDObject05",8,new string[]{"NDObject01","NDObject02"}),
			new TestParam("NDObject06",7,new string[]{"NDObject01","NDObject02","NDObject03"}),
			new TestParam("NDObject07",9,new string[]{"NDObject04"}),
			new TestParam("NDObject08",6,new string[]{"NDObject07"}),
		};*/
		Dictionary<string,TestParam> dictParams = new Dictionary<string, TestParam>();


		[TestFixtureSetUp]
		public void Init ()
		{
			funcOldDownloader = ResourceRefInterface.funcDownloader;
			funcOldUnserializer = ResourceRefInterface.funcUnserializer;
			actOldStartCoroutine = ResourceRefInterface.actStartCoroutine;
			ResourceManager.UseCoroutineLoop = false;

			ResourceRefInterface.funcDownloader = () => {
				return downloader;
			};
			ResourceRefInterface.funcUnserializer = (bytes) => {
				string path = System.Text.Encoding.Default.GetString (bytes);
				// 文件名被加了一个 .root/的前缀。
				path = System.IO.Path.GetFileName(path);
				MockResourceObject obj = new MockResourceObject (dictParams[path]);
				return obj;
			};
			ResourceRefInterface.actStartCoroutine = (e) => {
				bool completed = e.RunCoroutineWithoutYields (int.MaxValue);
				Assert.IsTrue (completed);
			};

			/*for (int i = 0; i < _params.Length; i++) {
				dictParams.Add (_params [i].path, _params [i]);
			}*/
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			ResourceRefInterface.funcDownloader = funcOldDownloader;
			ResourceRefInterface.funcUnserializer = funcOldUnserializer;
			ResourceRefInterface.actStartCoroutine = actOldStartCoroutine;
		}
		[TearDown]
		public void ClearManager(){
			dictParams.Clear ();
			ResourceManager.Instance.ReleaseAll ();
		}

		[Test]
		public void AddGetDeleteSingle ()
		{
			TestParam _param = new TestParam ("NDObject01", 0, null);
			dictParams.Add (_param.path, _param);

			int finished = 0;
			int error = 0;

			for (int i = 0; i < 10; i++) {
				ResourceManager.Instance.addResource (_param.path, _param.priority, false, (_ref) => {
					finished++;
				},
					(e) => {
						error++;
					});
			}
			while(ResourceManager.Instance.UpdateLoop () > 0);

			ResourceRef refResult = ResourceManager.Instance.getResource (_param.path);
			Assert.IsNotNull (refResult);
			Assert.AreEqual (finished, refResult.Refs); 


			for (int i = 0; i < 10; i++) {
				refResult = ResourceManager.Instance.getResource (_param.path);
				Assert.AreEqual (10-i, refResult.Refs);
				ResourceManager.Instance.delResource (_param.path);
			}
			while(ResourceManager.Instance.UpdateLoop () > 0);
			refResult = ResourceManager.Instance.getResource (_param.path);
			Assert.IsNull (refResult);

		}
		[Test]
		public void AddGetDeleteMuilty ()
		{
			int counts = RandomData.Random.Next (5, 10);
			TestParam[] _params = new TestParam[counts]; 
			for (int i = 0; i < counts; i++) {
				TestParam _param = new TestParam ("NDObject" + (i+1).ToString("D2"), 0, null);
				dictParams.Add (_param.path, _param);
			}



		}
		[Test]
		public void ValidPriority(){
			
			TestParam _param = new TestParam ("NDObject01", 0, null);
		}

	}
}
