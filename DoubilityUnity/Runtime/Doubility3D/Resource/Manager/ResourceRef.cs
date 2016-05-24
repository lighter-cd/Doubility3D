﻿using System;
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
		Action<ResourceRef> action;

		public ResourceRef(string _path)
		{
			path = _path;
			State = ResourceState.WaitingInQueue;
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

		public void Start(Action<ResourceRef> _action)
		{
			State = ResourceState.Started;

			IDownloader downloader = DownloaderFactory.Instance.Create ();
			if (downloader != null) {
				string resource_path = "Assets/ResData/" + path;
				State = ResourceState.Loading;
				new Task (downloader.ResourceTask (resource_path,OnDownloadComplate));
			}
			action = _action;
		}

		private void OnDownloadComplate(Byte[] bytes,string error){
			if (string.IsNullOrEmpty (error) ) {
				State = ResourceState.Parsing;
				new Task (ResourceTask (bytes));
			} else {
				State = ResourceState.Error; 
				action(this);
			}
		}

		IEnumerator ResourceTask(Byte[] bytes)
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
				} else {
					if (resourceObject.dependencePathes>0) {
						State = ResourceState.Depending;
						ResourceManager.Instance.resourceEvent += OnResourceComplateEvent;

						dependences = new ResourceRef[resourceObject.dependencePathes];
						for (int i = 0; i < resourceObject.dependencePathes; i++) {
							dependences [i] = ResourceManager.Instance.addResource (resourceObject.GetDependencePath(i), Priority, Async); 	
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
			ResourceRef resource = e.Resources;
			if (resource.State != ResourceState.Error)
			{
				int index = Array.IndexOf<ResourceRef> (dependences, resource);
				if (index >= 0) {
					bool ended = true;
					for (int i = 0; i < dependences.Length; i++) {
						if (dependences [i].State != ResourceState.Complated) {
							ended = false;
							break;
						}
					}
					if (ended) {
						resourceObject.OnDependencesFinished ();
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
