﻿using System;
using System.Collections;
using Doubility3D.Resource.Downloader;
using Doubility3D.Resource.ResourceObj;
using Doubility3D.Resource.Unserializing;

namespace Doubility3D.Resource.Manager
{
	static public class ResourceRefInterface
	{
		static public Func<IDownloader> funcDownloader = () => {
			return DownloaderFactory.Instance.Downloader;
		};
		static public Func<byte[],ResourceObject> funcUnserializer = (bytes) => {
			return UnserializerFactory.Instance.Unserialize (bytes);
		};
		public static Action<IEnumerator> actStartCoroutine = (e) => {
			new Task (e);
		};
		public static Action<string[],ResourceRef,Action<ResourceRef[]>,Action<Exception>> actAddDepences = (dependences,_ref, actComplate,actError)=>{
			int[] priorities = { _ref.Priority + 1 };
			ResourceManager.Instance.addResources (dependences, priorities, _ref.Async, actComplate, actError);
		};
		public static Action<string[]> actDelDepences = (dependences) => {
			ResourceManager.Instance.delResources(dependences);
		};
	}
}

