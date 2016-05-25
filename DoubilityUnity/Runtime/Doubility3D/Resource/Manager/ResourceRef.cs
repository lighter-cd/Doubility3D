using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;
using Doubility3D.Resource.Schema;
using Doubility3D.Resource.Unserializing;
using Doubility3D.Resource.ResourceObj;
using Doubility3D.Resource.Downloader;

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

	public class ResourceRef
	{
		string path;
		int refs;
		ResourceRef[] dependences;

		public ResourceRef(string _path)
		{
			path = _path;
			State = ResourceState.WaitingInQueue;
			refs = 1;
		}
		public int Refs { get { return refs; } }
		public void AddRefs() { refs++; }
		public void DelRefs() { refs--; }

		public string Path { get { return path; } }

		public ResourceObject resourceObject { get; set; }
		public ResourceState State { get; set; }
		public int Priority { get; set; }
		public bool Async { get; set; }
		public bool IsDone { get { return State == ResourceState.Complated; } }

		public Action<ResourceRef> Action { get; set;}

		public void Start()
		{
			State = ResourceState.Started;

			IDownloader downloader = DownloaderFactory.Instance.Create ();
			if (downloader != null) {
				string resource_path = "Assets/ResData/" + path;
				State = ResourceState.Loading;
				new Task (downloader.ResourceTask (resource_path,OnDownloadComplate));
			}
		}

		private void OnDownloadComplate(Byte[] bytes,string error){
			if (string.IsNullOrEmpty (error) ) {
				State = ResourceState.Parsing;
				Parse (bytes);
			} else {
				State = ResourceState.Error; 
				Action(this);
			}
		}

		void Parse(Byte[] bytes)
		{
			// 开始解析
			Schema.Context context = Context.Unknown;
			ByteBuffer bb = FileUnserializer.Load(bytes, out context);
			IUnserializer serializer = UnserializerFactory.Instance.Create (context);
			if (serializer != null) {
				resourceObject = serializer.Parse (bb);
				serializer = null;

				if (resourceObject == null) {
					State = ResourceState.Error;
					Action(this);
				} else {
					if (resourceObject.dependencePathes>0) {
						State = ResourceState.Depending;

						int[] priorities = {Priority};
						ResourceManager.Instance.addResources(resourceObject.DependencePathes,priorities,Async,OnDependResolved,OnDependError);

					} else {
						State = ResourceState.Complated;
						Action(this);
					}
				}
			} else {
				State = ResourceState.Error;
				Action(this);
			}
		}

		private void OnDependResolved(ResourceRef[] resources)
		{
			resourceObject.OnDependencesFinished ();
			State = ResourceState.Complated;
			Action(this);
		}
		private void OnDependError(Exception e){
			UnityEngine.Debug.LogError(e.Message);
			State = ResourceState.Error;
			Action(this);
		}
	}
}

