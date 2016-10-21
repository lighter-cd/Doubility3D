using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Util;
using Doubility3D.Resource.Downloader;
using Doubility3D.Resource.ResourceObj;
using Doubility3D.Resource.Manager;
using UnitTest.Doubility3D.Resource.Downloader;
using UnitTest.Doubility3D.Resource.Unserializing;

namespace UnitTest.Doubility3D.Resource.Manager
{
	class TestParam
	{
		public string path;
		public string[] dependencs;
		public int priority;

		public TestParam (string p, int pri, string[] ds)
		{
			path = p;
			dependencs = ds;
			priority = pri;
		}
	};

	class TestCounter
	{
		public int counter;
		public int finished;
		public int error;

		public TestCounter (int cnt)
		{
			counter = cnt;
		}
	};

	class MockDownloader : IDownloader
	{
		// 这些文件名读取时将会出错。
		HashSet<string> hashSetErrorFiles = new HashSet<string> ();

		public IEnumerator ResourceTask (string path, Action<Byte[],string> actOnComplate)
		{
			yield return null;
			if (!hashSetErrorFiles.Contains (path)) {
				actOnComplate (System.Text.Encoding.Default.GetBytes (path), null);
			} else {
				actOnComplate (null, "File read error:" + path);
			}
		}

		public void Dispose ()
		{
		}

		public void AddErrorFiles(string[] files){
			for (int i = 0; i < files.Length; i++) {
				hashSetErrorFiles.Add (files [i]);
			}
		}
		public void ClearErrorFiles(){
			hashSetErrorFiles.Clear ();
		}
	}

	class MockResourceObject : ResourceObject
	{
		private List<string> dependencesPath = new List<string> (8);
		private bool allDepndencesGotted;

		public MockResourceObject (TestParam param)
		{
			unity3dObject = new GameObject (param.path);
			if (param.dependencs != null) {
				dependencesPath.AddRange (param.dependencs);
			}
		}

		override public string[] DependencePathes {	get { return dependencesPath.ToArray (); } }

		override public void OnDependencesFinished ()
		{
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
		MockDownloader downloader = new MockDownloader ();
		// 这些文件名解析时将会出错。
		HashSet<string> hashSetErrorUnserializer = new HashSet<string> ();


		Dictionary<string,TestParam> dictParams = new Dictionary<string, TestParam> ();


		[TestFixtureSetUp]
		public void Init ()
		{
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

			ResourceManager.UseCoroutineLoop = false;

			/*for (int i = 0; i < _params.Length; i++) {
				dictParams.Add (_params [i].path, _params [i]);
			}*/
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			MockUnserializerFactory.CleanUp ();
		}

		[TearDown]
		public void ClearManager ()
		{
			dictParams.Clear ();
			ResourceManager.Instance.ReleaseAll ();
		}

		[Test]
		public void AddGetDeleteSingle ()
		{
			TestParam _param = new TestParam ("NDObject01", 0, null);
			dictParams.Add (_param.path, _param);
			TestCounter counter = new TestCounter (10);

			for (int i = 0; i < 10; i++) {
				ResourceManager.Instance.addResource (_param.path, _param.priority, false, (_ref) => {
					counter.finished++;
				},
					(e) => {
						counter.error++;
					});
			}
			while (ResourceManager.Instance.UpdateLoop () > 0)
				;

			ResourceRef refResult = ResourceManager.Instance.getResource (_param.path);
			Assert.IsNotNull (refResult);
			Assert.AreEqual (counter.finished, refResult.Refs); 


			for (int i = 0; i < 10; i++) {
				refResult = ResourceManager.Instance.getResource (_param.path);
				Assert.AreEqual (10 - i, refResult.Refs);
				ResourceManager.Instance.delResource (_param.path);
			}
			while (ResourceManager.Instance.UpdateLoop () > 0)
				;
			refResult = ResourceManager.Instance.getResource (_param.path);
			Assert.IsNull (refResult);

		}

		[Test]
		public void AddGetDeleteMuiltyAsync (){
			AddGetDeleteMuilty (true);
		}
		[Test]
		public void AddGetDeleteMuiltySync (){
			AddGetDeleteMuilty (false);
		}

		public void AddGetDeleteMuilty (bool bAsync)
		{
			Dictionary<string,TestCounter> dictCounter = new Dictionary<string, TestCounter> ();
			List<TestParam> lstParams = new List<TestParam> ();		
			int resources = RandomData.Random.Next (5, 10);
			for (int i = 0; i < resources; i++) {
				int count = RandomData.Random.Next (2, 5);
				TestParam _param = new TestParam ("NDObject" + (i + 1).ToString ("D2"), 0, null);
				dictParams.Add (_param.path, _param);
				dictCounter.Add (_param.path, new TestCounter (count));
				for (int j = 0; j < count; j++) {
					lstParams.Add (_param);
				}
			}

			TestParam[] arrayParams = ShufftList (lstParams);
			for (int i = 0; i < arrayParams.Length; i++) {
				string path = arrayParams [i].path;
				ResourceManager.Instance.addResource (arrayParams [i].path, arrayParams [i].priority, bAsync, (_ref) => {
					dictCounter [_ref.Path].finished++;
				},
					(e) => {
						dictCounter [path].error++;
					});				
			}
			while (ResourceManager.Instance.UpdateLoop () > 0)
				;

			Dictionary<string,TestParam>.Enumerator eDictParam = dictParams.GetEnumerator ();
			while (eDictParam.MoveNext ()) {
				ResourceRef refResult = ResourceManager.Instance.getResource (eDictParam.Current.Key);
				Assert.IsNotNull (refResult);
				Assert.AreEqual (dictCounter [eDictParam.Current.Key].finished, refResult.Refs); 
			}

			arrayParams = ShufftList (lstParams);
			for (int i = 0; i < arrayParams.Length; i++) {
				
				ResourceRef refResult = ResourceManager.Instance.getResource (arrayParams [i].path);
				Assert.AreEqual (dictCounter [arrayParams [i].path].finished, refResult.Refs);

				dictCounter [arrayParams [i].path].finished--;
				ResourceManager.Instance.delResource (arrayParams [i].path);
			}
			while (ResourceManager.Instance.UpdateLoop () > 0)
				;

			eDictParam = dictParams.GetEnumerator ();
			while (eDictParam.MoveNext ()) {
				ResourceRef refResult = ResourceManager.Instance.getResource (eDictParam.Current.Key);
				Assert.IsNull (refResult);
				Assert.AreEqual (0, dictCounter [eDictParam.Current.Key].finished);
			}
		}

		[Test]
		public void AddGetDeleteArrayAsync (){
			AddGetDeleteArray (true);
		}
		[Test]
		public void AddGetDeleteArraySync (){
			AddGetDeleteArray (false);
		}


		private void AddGetDeleteArray (bool bAsync)
		{
			List<string> pathes = new List<string> ();
			int resources = RandomData.Random.Next (5, 10);
			for (int i = 0; i < resources; i++) {
				TestParam _param = new TestParam ("NDObject" + (i + 1).ToString ("D2"), 0, null);
				dictParams.Add (_param.path, _param);
				pathes.Add (_param.path);
			}
			string[] arrayPathes = pathes.ToArray ();	// todo:输入参数最好不区分是 List 或者 Array

			int finished = 0;
			int error = 0;
			ResourceManager.Instance.addResources (arrayPathes, new int[]{ 0 }, bAsync, (_ref) => {
				finished++;
			},
				(e) => {
					error++;
				});	
			while (ResourceManager.Instance.UpdateLoop () > 0)
				;

			Dictionary<string,TestParam>.Enumerator eDictParam = dictParams.GetEnumerator ();
			while (eDictParam.MoveNext ()) {
				ResourceRef refResult = ResourceManager.Instance.getResource (eDictParam.Current.Key);
				Assert.IsNotNull (refResult);
				Assert.AreEqual (1, refResult.Refs); 
			}

			ResourceManager.Instance.delResources (arrayPathes);
			while (ResourceManager.Instance.UpdateLoop () > 0)
				;

			eDictParam = dictParams.GetEnumerator ();
			while (eDictParam.MoveNext ()) {
				ResourceRef refResult = ResourceManager.Instance.getResource (eDictParam.Current.Key);
				Assert.IsNull (refResult);
			}
		}

		[Test]
		public void ValidPriorityNoChanged (){
			ValidPriority (null, null);
		}
		[Test]
		public void ValidPriorityChanged (){
			ValidPriority ((TestParam[] ps)=>{
				Array.Sort<TestParam> (ps, new Comparison<TestParam>((t1,t2)=>{return t1.priority - t2.priority;}));
			}, (ResourceRef[] refs)=>{
				refs[0].Priority = int.MaxValue - 50;
			});
		}

		private void ValidPriority (Action<TestParam[]> actSort,Action<ResourceRef[]> actChangePri)
		{
			List<TestParam> lstParams = new List<TestParam> ();		
			int resources = RandomData.Random.Next (5, 10);
			for (int i = 0; i < resources; i++) {
		
				int pri = RandomData.Random.Next (0,100);		
				TestParam _param = new TestParam ("NDObject" + (i + 1).ToString ("D2"), pri, null);
				dictParams.Add (_param.path, _param);
				lstParams.Add (_param);
			}

			int error = 0;
			TestParam[] arrayParams = lstParams.ToArray ();	// todo:输入参数最好不区分是 List 或者 Array
			if (actSort != null) {
				actSort (arrayParams);
			}
			List<ResourceRef> refs = new List<ResourceRef>();

			Action<ResourceRef> actOnComplated = (_ref) => {
				refs.Add(_ref);
			};
			Action<Exception> actOnException = (e)=>{
				error++;
			};		

			for (int i = 0; i < arrayParams.Length; i++) {
				//Debug.Log("R:" + arrayParams [i].path + ":" + arrayParams [i].priority);
				ResourceManager.Instance.addResource (arrayParams [i].path, arrayParams [i].priority, true, actOnComplated,actOnException);				
			}	
			if (actChangePri != null) {
				//actChangePri (refs);
				ResourceManager.Instance.addResource (arrayParams [0].path, int.MaxValue - 50, true, actOnComplated,actOnException);	
			}
			while (ResourceManager.Instance.UpdateLoop () > 0)
				;

			ResourceRef[] arrayRefs = refs.ToArray ();
			for (int i = 0; i < arrayRefs.Length-1; i++) {
				Assert.GreaterOrEqual (arrayRefs [i].Priority, arrayRefs [i + 1].Priority);
			}

			Dictionary<string,TestParam>.Enumerator eDictParam = dictParams.GetEnumerator ();
			while (eDictParam.MoveNext ()) {
				ResourceRef refResult = ResourceManager.Instance.getResource (eDictParam.Current.Key);
				Assert.IsNotNull (refResult);
			}

			for (int i = 0; i < arrayParams.Length; i++) {
				ResourceManager.Instance.delResource (arrayParams [i].path);
			}
			if (actChangePri != null) {
				ResourceManager.Instance.delResource (arrayParams [0].path);
			}
			while (ResourceManager.Instance.UpdateLoop () > 0)
				;
		}

		private TestParam[] ShufftList (List<TestParam> _lstParams)
		{
			List<TestParam> lstParams = new List<TestParam> (_lstParams.Count);
			lstParams.AddRange (_lstParams);
			TestParam[] arrayParams = new TestParam[lstParams.Count];
			for (int i = 0; i < arrayParams.Length; i++) {
				int index = RandomData.Random.Next (0, lstParams.Count);
				arrayParams [i] = lstParams [index];
				lstParams.RemoveAt (index);
			}
			return arrayParams;
		}
		[Test]
		public void SingleWithDownloadError ()
		{
		}
		[Test]
		public void SingleWithPaserError ()
		{
		}
		[Test]
		public void MuiltyWithAllError ()
		{
		}
		[Test]
		public void MuiltyWithSomeError ()
		{
		}
		[Test]
		public void ArrayWithAllError ()
		{
		}
		[Test]
		public void ArrayWithSomeError ()
		{
		}
		[Test]
		public void LimitedProcessor ()
		{
		}
	}
}
