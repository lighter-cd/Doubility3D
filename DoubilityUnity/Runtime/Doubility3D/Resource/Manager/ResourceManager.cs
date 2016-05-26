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
				
			}
			return x.Priority - y.Priority;
		}
	}

	public enum ResourceMode
	{
		FromPacket,
		FromFile,
		FromWWW,
	}

	public class ResourceManager
	{
		private ResourceManager ()
		{
			new Task (Update());
		}

		static private ResourceManager _instance = null;
		static public ResourceMode resourceMode = ResourceMode.FromPacket;

		static public ResourceManager Instance{
			get{ 
				if (_instance == null) {
					DownloaderFactory.resourceMode = resourceMode;
					_instance = new ResourceManager ();
				}
				return _instance;
			}
		}

		bool needRemoveUnused = false;
		ResourceRef current = null;
		Stack<ResourceRef> stackWaiter = new Stack<ResourceRef> ();
		PriorityQueue<ResourceRef> queueResources = new PriorityQueue<ResourceRef>(new ResourceComparer());
		Dictionary<string, ResourceRef> dictResources = new Dictionary<string, ResourceRef>();

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
				dictResources[url] = resource;
				queueResources.Push(resource);
			}
			else
			{
				resource = dictResources[url];
				int oldPriority = resource.Priority;
				bool oldAsync = resource.Async;
				resource.Priority = Math.Max (priority, resource.Priority);
				resource.Async = resource.Async && bAsync;	// 一直传异步参数才能保持资源异步，一旦传入过同步参数，就一直是同步的了。
				resource.AddRefs();
				if((oldPriority != resource.Priority) || (oldAsync != resource.Async)){
					queueResources.ReSort ();
				}
			}

			Promise<ResourceRef> promise = new Promise<ResourceRef> ();
			if (resource.IsDone) {
				promise.Resolve (resource);

			} else {
				resource.Action = (refs)=>{
					if(refs.State != ResourceState.Error){
						promise.Resolve (refs);
					}else{
						promise.Reject(new Exception(refs.Path + " Load Error."));
					}
					refs.Action = null;
					OnResourceComplate(refs);
				};
			}

			return promise;
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
				// 一次只装载一个资源
				if (current == null) {
					while (queueResources.Count > 0) {
						ResourceRef resource = queueResources.Pop ();

						// 尝试取到一个引用计数不为0的资源
						while (resource.Refs == 0 && queueResources.Count > 0) {
							resource = queueResources.Pop ();
						}

						// 取到了一个引用计数不为0的资源
						if (resource.Refs > 0) {
							resource.Start ();
							//Debug.Log("资源 "+resource.Path+ " 开始装载");

							// 异步资源一帧只加载一个。同步资源加载完为止。
							if (resource.Async) {
								current = resource;
								break;
							}
						}
					}
				}


				if (stackWaiter.Count > 0) {
					ResourceRef top = stackWaiter.Peek ();
					if (top.IsDone) {
						stackWaiter.Pop ();
					}
				}

				if (needRemoveUnused) {
					Resources.UnloadUnusedAssets ();
					needRemoveUnused = false;
					//Debug.Log("释放一下无用资源");
				}

				yield return null;
			}
		}
		void OnResourceComplate(ResourceRef resource){
			//Debug.Log("资源 " + resource.Path + " 装载完毕");
			if (resource.Async && (resource == current))
			{
				current = null;
			}

			if (resource.Refs == 0)
			{
				ReleaseResource (resource);
				//Debug.Log("资源 url = " + resource.Path + "计数器已经为0装载完毕即被删除");
				return;
			}			
		}

		public void RegisterDependWaiter(ResourceRef refs){
			if (refs == current) {
				current = null;
			}
			stackWaiter.Push (refs);
		}

		private void ReleaseResource(ResourceRef resource){
			if (resource.IsDone && resource.resourceObject != null)
			{
				resource.resourceObject.Dispose ();
				resource.resourceObject = null;
				needRemoveUnused = true;
			}
		}
	}
}

