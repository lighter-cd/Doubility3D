using System;
using System.Collections;
using UnityEngine;

namespace Doubility3D.Resource.Downloader
{
	public class FileDownloader : IDownloader
	{
		private string home;

		internal FileDownloader ()
		{
			home = Environment.GetEnvironmentVariable ("DOUBILITY_HOME",EnvironmentVariableTarget.User);
			if (string.IsNullOrEmpty (home)) {
				home = Application.streamingAssetsPath;
			}
			home = home.Replace ('\\', '/');

			if (home [home.Length - 1] != '/') {
				home += "/";
			}			
		}
		public IEnumerator ResourceTask (string path,Action<Byte[],string> actOnComplate)
		{
			yield return null;
			try{
				byte[] bytes = System.IO.File.ReadAllBytes (home + path);
				actOnComplate(bytes,null);
			}catch(Exception e){
				actOnComplate(null,e.Message);
			}
		}

		public string Home { get { return home;} }
	}
}

