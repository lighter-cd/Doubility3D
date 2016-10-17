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
		public MockResourceObject (string name)
		{
			unity3dObject = TestResourceManager.Instance.GetObject (name);
			string[] ds = TestResourceManager.Instance.GetDependices (name);
			if (ds != null) {
				dependencesPath.AddRange (ds);
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
	class TestResourceManager : IDisposable {
		Dictionary<string,string[]> dictDependices = new Dictionary<string, string[]>();
		Dictionary<string,UnityEngine.Object> dictObjects = new Dictionary<string, UnityEngine.Object> ();
		Dictionary<string,MockResourceObject> dictResources = new Dictionary<string, MockResourceObject> ();

		static private TestResourceManager _instance = null;
		static public TestResourceManager Instance {
			get { 
				if (_instance == null) {
					_instance = new TestResourceManager ();
				}
				return _instance;
			}
		}
		public string[] GetDependices(string name){
			string[] dependices = null;
			dictDependices.TryGetValue(name,out dependices);
			return dependices;
		}
		public UnityEngine.Object GetObject(string name){
			UnityEngine.Object obj;
			dictObjects.TryGetValue (name, out obj);
			return obj;
		}
		public void AddObject(string name){
			UnityEngine.Object obj = new UnityEngine.Object ();
			obj.name = name;
			dictObjects.Add (name, obj);
		}
		public void AddDependices(string name,string[] dependices){
			dictDependices.Add (name, dependices);
		}
		public void Dispose(){
			dictDependices.Clear ();
			dictResources.Clear ();
			dictObjects.Clear ();
		}
	}

	public class ResourceManagerTest
	{
		Func<IDownloader> funcOldDownloader;
		Func<byte[],ResourceObject> funcOldUnserializer;
		Action<IEnumerator> actOldStartCoroutine;

		MockDownloader downloader = new MockDownloader ();

		[TestFixtureSetUp]
		public void Init ()
		{
			funcOldDownloader = ResourceRefInterface.funcDownloader;
			funcOldUnserializer = ResourceRefInterface.funcUnserializer;
			actOldStartCoroutine = ResourceRefInterface.actStartCoroutine;

			ResourceRefInterface.funcDownloader = () => {
				return downloader;
			};
			ResourceRefInterface.funcUnserializer = (bytes) => {
				MockResourceObject obj = new MockResourceObject (System.Text.Encoding.Default.GetString (bytes));
				return obj;
			};
			ResourceRefInterface.actStartCoroutine = (e) => {
				bool completed = e.RunCoroutineWithoutYields (int.MaxValue);
				Assert.IsTrue (completed);
			};


			// 数据：无依赖 有依赖(1层，2层，3层), 依赖无直接引用的，依赖也被直接引用的
			// 手写 json ?
			for(int i=0;i<10;i++){
				TestResourceManager.Instance.AddObject ("NDObject" + (i + 1).ToString ("D2"));
			}


			int count = 1;
			for(int i=0;i<10;i++){
				string[] dd = new string[5];
				for (int j = 0; j < 5; j++) {
					dd [j] = "BDObject" + (j + count).ToString ("D2");
					count++;
				}
				TestResourceManager.Instance.AddObject ("DDObject" + (i + 1).ToString ("D2"));
				TestResourceManager.Instance.AddDependices ("DDObject" + (i + 1).ToString ("D2"),dd);
			}
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			ResourceRefInterface.funcDownloader = funcOldDownloader;
			ResourceRefInterface.funcUnserializer = funcOldUnserializer;
			ResourceRefInterface.actStartCoroutine = actOldStartCoroutine;
			TestResourceManager.Instance.Dispose ();
		}


		[Test]
		public void AddAndDeleteWithoutDependices ()
		{
			ResourceManager.Instance.addResource ("NDObject01", 0, false, (_ref) => {
			},
				(e) => {
			});
			ResourceManager.Instance.UpdateLoop ();
		}
		[Test]
		public void AddAndDeleteWithDependices ()
		{
		}
		[Test]
		public void ValidPriority(){
		}

	}
}
