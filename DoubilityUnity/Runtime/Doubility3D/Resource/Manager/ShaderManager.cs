using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Doubility3D.Util;
using Doubility3D.Resource.Downloader;

namespace Doubility3D.Resource.Manager
{
	class ShaderRef
	{
		public Shader shader;
		public int refs;
	}

	public enum ShaderLoadResult
	{
		Ok = 0,
		BundleDownloadError,
		BundleLoadError,
		DictionaryLoadError,
		DictionaryJsonError,
		ContentCheckError,
	}

	public class ShaderManager
	{
		private string coreDataBundle;

		private ShaderManager ()
		{
			coreDataBundle = ".coreData/" + PlatformPath.GetPath (Application.platform) + "/coreData.bundle";
		}

		static private ShaderManager _instance = null;

		static public ShaderManager Instance {
			get { 
				if (_instance == null) {
					_instance = new ShaderManager ();
				}
				return _instance;
			}
		}

		private AssetBundle ab;
		private Dictionary<string,string> dictName2Path = new Dictionary<string, string> ();
		private Dictionary<string,ShaderRef> dictShaderRefs = new Dictionary<string, ShaderRef> ();
		private Action<ShaderLoadResult,string> actOnComplate;

		public void LoadAssetBundle (Action<ShaderLoadResult,string> _actOnComplate, string bundlePath = null)
		{
			if (bundlePath != null) {
				coreDataBundle = bundlePath;
			}

			actOnComplate = _actOnComplate;
			// 装载数据
			IDownloader downloader = DownloaderFactory.Instance.Create ();
			new Task (downloader.ResourceTask (coreDataBundle, OnDownloadComplate));
		}

		public void DisposeBundle(){
			if (ab != null) {
				ab.Unload (true);
			}
		}

		public string CoreDataBundle { get { return coreDataBundle; } }

		private void OnDownloadComplate (Byte[] bytes, string error)
		{
			if (string.IsNullOrEmpty (error)) {
				new Task (AssetBundleTask (bytes));
			} else {
				actOnComplate (ShaderLoadResult.BundleDownloadError,error);
			}
		}

		public Shader AddShader (string name)
		{
			if (ab != null) {
				if (dictShaderRefs.ContainsKey (name)) {
					ShaderRef refs = dictShaderRefs [name];
					refs.refs++;
					return refs.shader;
				} else {
					ShaderRef refs = new ShaderRef ();
					refs.refs = 1;
					refs.shader = ab.LoadAsset<Shader> (dictName2Path [name]);
					dictShaderRefs.Add (name, refs);
					return refs.shader;
				}
			}
			return null;
		}

		public void DelShader (Shader shader)
		{
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
						UnityEngine.Object.DestroyImmediate (refs.shader, true);// 这里绝不关联到
						#endif
						dictShaderRefs.Remove (name);
					}
				} else {
					Debug.LogError ("DelShader " + name + ":Shader not found in manager!");
				}
			}
		}

		public int GetShaderRef (string name)
		{
			if (ab != null) {
				if (dictShaderRefs.ContainsKey (name)) {
					ShaderRef refs = dictShaderRefs [name];
					return refs.refs;
				} 
			}
			return 0;			
		}

		IEnumerator AssetBundleTask (Byte[] bytes)
		{
			AssetBundleCreateRequest abcq = AssetBundle.LoadFromMemoryAsync (bytes);
			yield return abcq;

			ShaderLoadResult error = ShaderLoadResult.Ok;
			string info = "";
			if (abcq.isDone) {
				if (abcq.assetBundle != null) {
					ab = abcq.assetBundle;
					AssetBundleRequest abr = ab.LoadAssetAsync (ShaderDictionary.Path);
					yield return abr;

					if (abr.isDone) {
						if (abr.asset != null && abr.asset is TextAsset) {
							TextAsset asset = abr.asset as TextAsset;
							if (!BuildDictName2Path (asset)){
								error = ShaderLoadResult.ContentCheckError;
							}
						} else {
							error = ShaderLoadResult.DictionaryLoadError;
							info = ShaderDictionary.Path;
						}
					}
				} else {
					error = ShaderLoadResult.BundleLoadError;
					info = coreDataBundle;
				}
			}
			actOnComplate (error,info);
		}

		private bool BuildDictName2Path (TextAsset asset)
		{
			string[] names = ab.GetAllAssetNames ();

			JsonData data = JsonMapper.ToObject (asset.text);
			IEnumerator e = data.Keys.GetEnumerator ();
			while (e.MoveNext ()) {
				string key = e.Current as string;
				string value = (string)data [key];

				int index = Array.IndexOf<string> (names, value);
				if (index < 0) {
					return false;
				}

				dictName2Path.Add (key, value);
			}
			return true;
		}

		public string[] GetShaderList()
		{
			string[] shaderList = new string[dictName2Path.Count];
			Dictionary<string,string>.Enumerator e = dictName2Path.GetEnumerator ();
			int count = 0;
			while (e.MoveNext ()) {
				shaderList [count] = e.Current.Key;
				count++;
			}
			return shaderList;
		}
	}
}

