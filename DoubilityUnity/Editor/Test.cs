using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using Doubility3D.Resource.Unserializing;
using UnitTest.Doubility3D.Resource.Downloader;
using UnitTest.Doubility3D.Resource.Unserializing;

namespace Doubility3D
{

	class MockDownloader : Doubility3D.Resource.Downloader.IDownloader{
		public IEnumerator ResourceTask (string path,Action<Byte[],string> actOnComplate){
			yield return null;
		}
		public void Dispose(){
		}
	};

	static public class Test
	{
		[MenuItem ("逗逼工具/搞一哈!")]
		static void DoIt()
		{
			/*if(Selection.activeObject is Material)
			{
				Material mat = Selection.activeObject as Material;
				int count = ShaderUtil.GetPropertyCount(mat.shader);
				for(int i=0;i<count;i++)
				{
					string name = ShaderUtil.GetPropertyName(mat.shader,i);
					ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(mat.shader,i);
					UnityEngine.Debug.Log("name:"+name+",type:"+type);
				}
			}*/
			Doubility3D.Resource.Downloader.DownloaderFactory  df = Doubility3D.Resource.Downloader.DownloaderFactory.Instance;
			MockDownloaderFactory.Initialize (new MockDownloader ());
			Doubility3D.Resource.Downloader.IDownloader idlr = df.Downloader;
			Debug.Log (idlr.GetType ().FullName);

			MockUnserializerFactory.Initialize ((bytes) => {
				Debug.Log ("Unserialize");
				return null;
			});
			UnserializerFactory.Instance.Unserialize (null);
		}
	}
}

