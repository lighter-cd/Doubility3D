using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using RSG;
using UnityEngine;
using Doubility3D.Resource.Downloader;

namespace Doubility3D.Resource.Manager
{
	public class ResourceComparer : Comparer<ResourceRef>
	{
		public override int Compare(ResourceRef x, ResourceRef y)
		{
			// 同步的排在前面
			if (x.Async != y.Async) {
				if (!x.Async && y.Async) {
					return 1;
				} else if (x.Async && !y.Async) {
					return -1;
				}
			}
			return x.Priority - y.Priority;
		}
	}

	public class ResourceManager
	{
		static private ResourceManager _instance = null;
		static public int NumberOfProcessor = 4;
		static public bool UseCoroutineLoop = true;

		static public ResourceManager Instance{
			get{ 
				if (_instance == null) {
					_instance = new ResourceManager ();
				}
				return _instance;
			}
		}

		IResourceScheduler syncProcessor;	// for sync ResourceRef
		IResourceScheduler asyncProcessor;	// for async ResourceRef
		bool needRemoveUnused = false;
		PriorityQueue<ResourceRef> queueResources = new PriorityQueue<ResourceRef>(new ResourceComparer());
		Dictionary<string, ResourceRef> dictResources = new Dictionary<string, ResourceRef>();

		private ResourceManager ()
		{
			if (UseCoroutineLoop) {
				new Task (Update ());
			}
			syncProcessor = new ResourceSchedulerLimitless ();
			asyncProcessor = new ResourceSchedulerLimited (NumberOfProcessor);
		}


		private IPromise<ResourceRef> _addResource(string url, int priority, bool bAsync)
		{
			ResourceRef resource = null;
			Debug.Assert(url != null, "addResource url 不能为 null");
			Debug.Assert(url != string.Empty, "addResource url 不能为 string.Empty");
			if (!dictResources.ContainsKey(url))
			{
				resource = new ResourceRef(url);
				resource.Priority = priority;
				resource.Async = bAsync;
				resource.ComplatedEvent += OnResourceComplate;
				dictResources[url] = resource;
				queueResources.Push(resource);
			}
			else
			{
				resource = dictResources[url];
				resource.AddRefs();

				if (resource.InQueue) {
					int oldPriority = resource.Priority;
					bool oldAsync = resource.Async;
					resource.Priority = Math.Max (priority, resource.Priority);
					resource.Async = resource.Async && bAsync;	// 一直传异步参数才能保持资源异步，一旦传入过同步参数，就一直是同步的了。
					if ((oldPriority != resource.Priority) || (oldAsync != resource.Async)) {
						queueResources.ReSort ();
					}
				}
			}

			return resource.AcceptPromise ();
		}

		public ResourceRef addResource(string url, int priority, bool bAsync,Action<ResourceRef> actComplate,Action<Exception> actError){
			_addResource(url,priority,bAsync).Catch(actError).Then(actComplate).Done();
			return getResource(url);
		}

		public ResourceRef[] addResources(string[] urls, int[] priorities, bool bAsync,Action<ResourceRef[]> actComplate,Action<Exception> actError)
		{
			IPromise<ResourceRef>[] promises = new IPromise<ResourceRef>[urls.Length];
			ResourceRef[] refs = new ResourceRef[urls.Length];
			for (int i = 0; i < urls.Length; i++) {
				int priority = (priorities!=null)?(priorities.Length>=urls.Length?priorities[i]:priorities[0]):0;
				promises[i] = _addResource (urls[i], priority, bAsync);
				refs[i] = getResource(urls[i]);
			}

			Func<IEnumerable<ResourceRef>,IEnumerable<IPromise<ResourceRef>>> funcResolved = resources=>{
				actComplate (resources.ToArray());
				return promises;
			};
			Promise<ResourceRef>.All(promises).Catch(actError).ThenAll(funcResolved).Done();
			return refs;
		}

		public void delResource(string url)
		{
			Debug.Assert(url != null, "delResource url 不能为 null");
			Debug.Assert(url != string.Empty, "delResource url 不能为 string.Empty");
			Debug.Assert(dictResources.ContainsKey(url), "资源 url = " + url + "不存在");
			if (dictResources.ContainsKey(url))
			{
				ResourceRef resource = dictResources[url];
				resource.DelRefs();

				if (resource.Refs == 0)
				{
					ReleaseResource (resource);
					//Debug.Log("资源 url = " + url + "计数器为0被删除");
					dictResources.Remove(url);
				}
			}
		}

		public void delResources(string[] urls)
		{
			for (int i = 0; i < urls.Length; i++) {
				delResource(urls[i]);
			}
		}

		public ResourceRef getResource(string url)
		{
			if (dictResources.ContainsKey(url))
			{
				return dictResources[url];
			}
			return null;
		}

		private IEnumerator Update()
		{
			while (true) {
				UpdateLoop ();
				yield return null;
			}
		}
		public int UpdateLoop(){
			syncProcessor.ProcessQueue (queueResources);
			asyncProcessor.ProcessQueue (queueResources);
			if (needRemoveUnused) {
				Resources.UnloadUnusedAssets ();	//释放一下无用资源
				needRemoveUnused = false;
			}
			return queueResources.Count;
		}
		void OnResourceComplate(ResourceRef resource){
			resource.ComplatedEvent -= OnResourceComplate;

			if (resource.Async) {
				asyncProcessor.OnResourceComplate (resource);
			} else {
				syncProcessor.OnResourceComplate (resource);
			}

			if (resource.Refs == 0)
			{
				ReleaseResource (resource);
				return;
			}			
		}

		private void ReleaseResource(ResourceRef resource){
			if (resource.IsDone && resource.resourceObject != null)
			{
				resource.resourceObject.Dispose ();
				resource.resourceObject = null;
				needRemoveUnused = true;
			}
		}

		public void ReleaseAll(){

			Dictionary<string, ResourceRef>.Enumerator e = dictResources.GetEnumerator ();
			while(e.MoveNext()){
				ResourceRef resource = e.Current.Value;
				resource.DelRefs();
				ReleaseResource (resource);
			}
			dictResources.Clear ();
			queueResources.Clear ();
		}
	}
}

