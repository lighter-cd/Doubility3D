using System;
using System.Collections;
using UnityEngine;

namespace Doubility3D.Resource.Downloader
{
	public class FileDownloader : IDownloader
	{
		private static string home;

		static FileDownloader(){
			home = Environment.GetEnvironmentVariable ("DOUBILITY_HOME",EnvironmentVariableTarget.Machine);
			if (string.IsNullOrEmpty (home)) {
				home = Application.streamingAssetsPath;
			}
			home = home.Replace ('\\', '/');
		}

		public FileDownloader ()
		{
			
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
	}
}

