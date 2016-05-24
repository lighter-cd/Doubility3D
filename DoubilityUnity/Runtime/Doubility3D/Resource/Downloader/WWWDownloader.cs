using System;
using System.Collections;
using UnityEngine;

namespace Doubility3D.Resource.Downloader
{
	public class WWWDownloader : IDownloader
	{
		private string home;

		public WWWDownloader (string _home)
		{
			home = _home;
		}
		public IEnumerator ResourceTask (string path,Action<Byte[],string> actOnComplate)
		{
			Byte[] _bytes = null;
			string _error = null;

			WWW www = new WWW (WWW.EscapeURL (home + path));
			yield return www;
			if (www.isDone) {
				_bytes = www.bytes;
			} else {
				_error = www.error;
			}
			actOnComplate (_bytes, _error);
		}
	}
}

