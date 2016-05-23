using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;
using Doubility3D.Resource.Schema;
using Doubility3D.Resource.Serializer;

namespace Doubility3D.Resource.Manager
{
	public enum ResourceState
	{
		WaitingInQueue,
		Started,
		Loading,
		Parsing,
		Depending,
		Complated,
		Cancled,
		Error,
	}

	public class Resource
	{
		string path;
		int refs;
		Resource[] dependences;

		public Resource(string _path)
		{
			path = _path;
			State = ResourceState.WaitingInQueue;
		}
		public int Refs { get { return refs; } }
		public void AddRefs() { refs++; }
		public void DelRefs() { refs--; }

		public string Path { get { return path; } }

		public UnityEngine.Object Unity3dObject { get; set; }
		public UnityEngine.Object[] Unity3dObjects { get; set; }
		public ResourceState State { get; set; }
		public int Priority { get; set; }
		public bool Async { get; set; }

		public void Start(Action<Resource> action)
		{
			State = ResourceState.Started;

			new Task(ResourceTask(action));
		}
		IEnumerator ResourceTask(Action<Resource> action)
		{
			string resource_path = "Assets/ResData/" + path;
			State = ResourceState.Loading;
			Byte[] bytes = null;

			// 装载数据
			IDownloader downloader = DownloaderFactory.Instance.Create (resource_path);
			yield return downloader.ResourceTask(out bytes);

			// 装载完毕
			downloader.Dispose ();
			downloader = null;

			// 开始解析
			Schema.Context context = Context.Unknown;
			ByteBuffer bb = FileSerializer.Load(bytes, out context);
			ISerializer serializer = SerializerFactory.Instance.Create (context);
			if (serializer != null) {
				String[] dependencePathes = null;
				Unity3dObject = serializer.Parse (bb, out dependencePathes);
				serializer = null;

				if (Unity3dObject == null) {
					State = ResourceState.Error;
				} else {
					if (dependencePathes != null && dependencePathes.Length>0) {
						State = ResourceState.Depending;
						ResourceManager.Instance.resourceEvent += OnResourceComplateEvent;
						dependences = new Resource[dependencePathes.Length];
						for (int i = 0; i < dependencePathes.Length; i++) {
							dependences [i] = ResourceManager.Instance.addResource (dependencePathes [i], Priority, Async); 	
						}
						if (State != ResourceState.Complated && State != ResourceState.Error) {
							yield return null;							
						}
					} else {
						State = ResourceState.Complated;	
					}
				}
			} else {
				State = ResourceState.Error; 	
			}

			action(this);
		}

		private void OnResourceComplateEvent(object sender, ResourceEventArgs e)
		{
			Resource resource = e.Resources;
			if (resource.State != ResourceState.Error)
			{
				int index = Array.IndexOf<Resource> (dependences, resource);
				if (index >= 0) {
					bool ended = true;
					for (int i = 0; i < dependences.Length; i++) {
						if (dependences [i].State != ResourceState.Complated) {
							ended = false;
							break;
						}
					}
					if (ended) {
						State = ResourceState.Complated;
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning("资源 " + resource.Path + "装载出错");
				State = ResourceState.Error;
			}
		}
	}
}

