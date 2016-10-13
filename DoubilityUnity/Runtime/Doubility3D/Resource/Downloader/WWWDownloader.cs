using System;
using System.Collections;
using UnityEngine;

namespace Doubility3D.Resource.Downloader
{
	public class WWWDownloader : IDownloader
	{
		private string home;

		internal WWWDownloader (string _home)
		{
			home = _home;
			if (home [home.Length - 1] != '/') {
				home += "/";
			}
		}

		public IEnumerator ResourceTask (string path, Action<Byte[],string> actOnComplate)
		{
			Byte[] _bytes = null;

			WWW www = new WWW (home + path);

			while (!www.isDone) {
				yield return www;
			}
			if (string.IsNullOrEmpty (www.error)) {
				_bytes = www.bytes;
			} 
			actOnComplate (_bytes, www.error);
		}

		public string Home { get { return home;} }
		public void Dispose(){
		}
	}
}

