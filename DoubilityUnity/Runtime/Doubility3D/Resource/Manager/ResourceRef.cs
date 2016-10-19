using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;
using RSG;
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
		List<Promise<ResourceRef>> lstPromises = new List<Promise<ResourceRef>> ();

		public delegate void ComplatedHandle(ResourceRef _ref);
		public event ComplatedHandle ComplatedEvent = null;

		public ResourceRef (string _path)
		{
			path = _path;
			State = ResourceState.WaitingInQueue;
			refs = 1;
		}


		public int Refs { get { return refs; } }

		public void AddRefs ()
		{
			refs++;
		}

		public void DelRefs ()
		{
			refs--;
		}

		public int Priority { get; set; }

		public bool Async { get; set; }

		public int Processor { get; set; }



		public string Path { get { return path; } }

		public ResourceObject resourceObject { get; set; }

		public bool IsDone { get { return State <= ResourceState.Complated; } }

		public bool InQueue { get { return State == ResourceState.WaitingInQueue; } }

		public string[] Dependences { get { return dependences; } }

		private ResourceState State { get; set; }
		private string Error { get; set; }


		public void Start ()
		{
			State = ResourceState.Started;
			ResourceRefInterface.actStartCoroutine (ProcessTask (this));
		}

		IEnumerator ProcessTask (ResourceRef refs)
		{

			IDownloader downloader = ResourceRefInterface.funcDownloader();
			if (downloader == null) {
				State = ResourceState.Error; 
				Error = "Create downloader error.";
				OnComplated ();				
				yield break;
			}

			State = ResourceState.Loading;
			string resource_path = ".root/" + path;
			yield return downloader.ResourceTask (resource_path, OnDownloadComplate);

			if (State == ResourceState.Error) {
				OnComplated ();
				yield break;
			}

			if (dependences == null) {
				State = ResourceState.Complated;
				OnComplated ();
				yield break;
			} 

			State = ResourceState.Depending;
			int[] priorities = { Priority + 1 };
			ResourceManager.Instance.addResources (dependences, priorities, Async, OnDependResolved, OnDependError);
			while (!IsDone) {
				yield return null;
			}

			OnComplated ();
			if (ComplatedEvent != null) {
				ComplatedEvent (this);
			}
		}

		private void OnDownloadComplate (Byte[] bytes, string error)
		{
			if (string.IsNullOrEmpty (error)) {
				State = ResourceState.Parsing;
				Parse (bytes);
			} else {
				State = ResourceState.Error; 
				Error = "Download error:" + error;
			}
		}

		void Parse (Byte[] bytes)
		{
			// 开始解析
			resourceObject = ResourceRefInterface.funcUnserializer(bytes);
			if (resourceObject == null) {
				State = ResourceState.Error;
				Error = "Parse error, return null resourceObject";
			} else {
				dependences = resourceObject.DependencePathes;
			}
		}

		private void OnDependResolved (ResourceRef[] resources)
		{
			resourceObject.OnDependencesFinished ();
			State = ResourceState.Complated;
		}

		private void OnDependError (Exception e)
		{
			State = ResourceState.Error;
			Error = "DependError:" + e.Message;
		}
		public void OnRelease(){
			ResourceManager.Instance.delResources (dependences);
			resourceObject.Dispose ();
			resourceObject = null;
		}


		private void ProcessPromise(Promise<ResourceRef> promise){
			if (State != ResourceState.Error) {
				promise.Resolve (this);
			} else {
				promise.Reject (new Exception (Path + " Load Error."));
			}

		}
		private void OnComplated(){
			List<Promise<ResourceRef>>.Enumerator e = lstPromises.GetEnumerator ();
			while (e.MoveNext ()) {
				Promise<ResourceRef> promise = e.Current;
				ProcessPromise (promise);
			}
			lstPromises.Clear ();

			if (ComplatedEvent != null) {
				ComplatedEvent (this);
			}
		}

		public Promise<ResourceRef> AcceptPromise(){
			Promise<ResourceRef> promise = new Promise<ResourceRef> ();
			if (IsDone) {
				ProcessPromise (promise);
			} else {
				lstPromises.Add (promise);
			}
			return promise;
		}
	}
}

