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
		string[] dependences;

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
		public string Error { get; set; }

		public Action<ResourceRef> Action { get; set;}
		public int Processor { get; set;}

		public void Start()
		{
			State = ResourceState.Started;
			new Task (ProcessTask (this));
		}

		IEnumerator ProcessTask(ResourceRef refs){

			IDownloader downloader = DownloaderFactory.Instance.Downloader;
			if (downloader == null) {
				State = ResourceState.Error; 
				Error = "Create downloader error.";
				Action(this);				
				yield break;
			}

			State = ResourceState.Loading;
			string resource_path = ".root/" + path;
			yield return downloader.ResourceTask (resource_path, OnDownloadComplate);

			if (State == ResourceState.Error) {
				Action(this);				
				yield break;
			}

			if (dependences == null) {
				State = ResourceState.Complated;
				Action(this);
				yield break;
			} 

			State = ResourceState.Depending;
			int[] priorities = { Priority + 1};
			ResourceManager.Instance.RegisterDependWaiter (this);
			ResourceManager.Instance.addResources (dependences, priorities, Async, OnDependResolved, OnDependError);
			while (!IsDone) {
				yield return null;
			}

			Action(this);
		}

		private void OnDownloadComplate(Byte[] bytes,string error){
			if (string.IsNullOrEmpty (error) ) {
				State = ResourceState.Parsing;
				Parse (bytes);
			} else {
				State = ResourceState.Error; 
				Error = "Download error:" + error;
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
					Error = "Parse error, return null resourceObject";
				} else {
					dependences = resourceObject.DependencePathes;
				}
			} else {
				State = ResourceState.Error;
				Error = "Create Unserializer error,context = " + context.ToString();
			}
		}

		private void OnDependResolved(ResourceRef[] resources)
		{
			resourceObject.OnDependencesFinished ();
			State = ResourceState.Complated;
		}
		private void OnDependError(Exception e){
			State = ResourceState.Error;
			Error = "DependError:" + e.Message;
		}
	}
}

