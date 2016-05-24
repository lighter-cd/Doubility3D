using System;
using System.Collections.Generic;
using UnityEngine;
using Doubility3D.Resource.Downloader;

namespace Doubility3D.Resource.Manager
{
	public class ResourceEventArgs : EventArgs
	{
		ResourceRef resource;

		public ResourceEventArgs(ResourceRef _resource)
		{
			resource = _resource;
		}

		public ResourceRef Resources { get { return resource; } }
	}
	public class ObjectEventArgs : EventArgs
	{
		public ObjectEventArgs(){

		}
		public UnityEngine.Object Target { get; set; }
		public int Zone{get;set;}
	}

	public class ResourceComparer : Comparer<ResourceRef>
	{
		public override int Compare(ResourceRef x, ResourceRef y)
		{
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
		}

		static private ResourceManager _instance = null;
		static public ResourceMode resourceMode = ResourceMode.FromPacket;

		static public ResourceManager Instance{
			get{ 
				if (_instance == null) {
					DownloaderFactory.resourceMode = resourceMode;
					_instance = new ResourceManager ();
				}
				return Instance;
			}
		}

		bool needRemoveUnused = false;
		ResourceRef current = null;
		PriorityQueue<ResourceRef> queueResource = new PriorityQueue<ResourceRef>(new ResourceComparer());
		Dictionary<string, ResourceRef> dictResources = new Dictionary<string, ResourceRef>();
		public event EventHandler<ResourceEventArgs> resourceEvent;

		public ResourceRef addResource(string url, int priority, bool bAsync)
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
				queueResource.Push(resource);
			}
			else
			{
				resource = dictResources[url];
			}
			resource.AddRefs();
			return resource;
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
					if (resource.resourceObject != null)
					{
						resource.resourceObject.Dispose ();
						resource.resourceObject = null;
						needRemoveUnused = true;
					}
					//Debug.Log("资源 url = " + url + "计数器为0被删除");
					dictResources.Remove(url);
				}
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

		private void DispatchResourceEvent(ResourceRef resource)
		{
			//Debug.Log("资源 " + resource.Path + " 装载完毕");
			if (resource.Async)
			{
				current = null;
			}

			if (resource.Refs == 0)
			{
				if (resource.resourceObject != null)
				{
					resource.resourceObject.Dispose ();
					resource.resourceObject = null;
					needRemoveUnused = true;
				}
				//Debug.Log("资源 url = " + resource.Path + "计数器已经为0装载完毕即被删除");
				return;
			}

			//复制一个委托的引用到临时字段temp，这样确保线程安全
			EventHandler<ResourceEventArgs> temp = this.resourceEvent;

			//任何注册到事件里面的方法，通知它们
			if (temp != null)
			{
				temp(this, new ResourceEventArgs(resource));
			}
		}

		public void Update()
		{
			// 一次只装载一个资源
			if (current == null || current.State == ResourceState.Depending)
			{
				while (queueResource.Count > 0)
				{
					ResourceRef resource = queueResource.Pop();

					// 尝试取到一个引用计数不为0的资源
					while (resource.Refs == 0 && queueResource.Count > 0)
					{
						resource = queueResource.Pop();
					}

					// 取到了一个引用计数不为0的资源
					if (resource.Refs > 0)
					{
						resource.Start(DispatchResourceEvent);
						//Debug.Log("资源 "+resource.Path+ " 开始装载");

						// 异步资源一帧只加载一个。同步资源加载完为止。
						if (resource.Async)
						{
							current = resource;
							break;
						}
					}
				}
			}
			else
			{
				// 如果资源出错了，就把当前的资源放过。
				if (current.State == ResourceState.Error) {
					current = null;
				} 

				if (current.Refs == 0 && current.State == ResourceState.WaitingInQueue)
				{
					Debug.LogError("无法处理的当前资源？");
				}
			}
			if (needRemoveUnused)
			{
				Resources.UnloadUnusedAssets();
				needRemoveUnused = false;
				//Debug.Log("释放一下无用资源");
			}
		}
	}
}

