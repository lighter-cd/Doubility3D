using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doubility3D.Util;
using Doubility3D.Resource.Downloader;

namespace Doubility3D.Resource.Manager
{
	class ShaderRef {
		public Shader shader;
		public int refs;
	}
	public class ShaderManager
	{
		private string coreDataBundle;
		private ShaderManager ()
		{
			coreDataBundle = "/.coreData/" + PlatformPath.GetPath(Application.platform) + "/coreData.bundle";
		}
		static private ShaderManager _instance = null;

		static public ShaderManager Instance{
			get{ 
				if (_instance == null) {
					_instance = new ShaderManager ();
				}
				return Instance;
			}
		}

		private AssetBundle ab;
		private Dictionary<string,string> dictName2Path = new Dictionary<string, string>();
		private Dictionary<string,ShaderRef> dictShaderRefs = new Dictionary<string, ShaderRef> ();
		private Action<string> actOnComplate;

		public void LoadAssetBundle(Action<string> _actOnComplate){

			actOnComplate = _actOnComplate;
			// 装载数据
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			new Task(downloader.ResourceTask(coreDataBundle,OnDownloadComplate));
		}

		private void OnDownloadComplate(Byte[] bytes,string error){
			if (string.IsNullOrEmpty (error)) {
				new Task (AssetBundleTask (bytes));
			} else {
				actOnComplate (error);
			}
		}

		public Shader AddShader(string name){
			if (ab != null) {
				if (dictShaderRefs.ContainsKey (name)) {
					ShaderRef refs = dictShaderRefs [name];
					refs.refs++;
					return refs.shader;
				} else {
					ShaderRef refs = new ShaderRef ();
					refs.refs = 1;
					refs.shader = ab.LoadAsset<Shader>(dictName2Path[name]);
					dictShaderRefs.Add (name, refs);
					return refs.shader;
				}
			}
			return null;
		}
		public void DelShader(Shader shader){
			if (ab != null) {
				string name = shader.name;
				if (dictShaderRefs.ContainsKey (name)) {
					ShaderRef refs = dictShaderRefs [name];
					refs.refs--;
					if (refs.refs == 0) {
						// 删除资源物体
						#if !UNITY_EDITOR
						UnityEngine.Object.Destroy(refs.shader); 
						#else
						UnityEngine.Object.DestroyImmediate(refs.shader,true);// 这里绝不关联到
						#endif
						dictShaderRefs.Remove (name);
					}
				} else {
					Debug.LogError ("DelShader " + name + ":Shader not found in manager!");
				}
			}
		}

		IEnumerator AssetBundleTask(Byte[] bytes)
		{
			AssetBundleCreateRequest abcq = AssetBundle.LoadFromMemoryAsync (bytes);
			yield return abcq;
			if (abcq.isDone)
			{
				ab = abcq.assetBundle;
			}
			actOnComplate (null);
		}
	}
}

