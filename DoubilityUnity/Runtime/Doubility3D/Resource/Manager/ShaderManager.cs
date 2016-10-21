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

	/// <summary>
	/// Shader manager.
	/// 依赖于 DownloaderFactory
	/// 依赖于 CoroutineRunner
	/// </summary>
	public class ShaderManager
	{
		public static string shaderDictPath = ShaderDictionary.Path;

		private string coreDataBundle;

		private ShaderManager ()
		{
			coreDataBundle = ".coreData/" + PlatformPath.GetPath (Application.platform) + "/coredata.bundle";
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

			CoroutineRunner.Instance.Run (AssetBundleTask ());
		}

		public void DisposeBundle ()
		{
			Dictionary<string,ShaderRef>.Enumerator e = dictShaderRefs.GetEnumerator ();
			while (e.MoveNext ()) {
				ShaderRef refs = e.Current.Value;
				#if !UNITY_EDITOR
				UnityEngine.Object.Destroy(refs.shader); 
				#else
				UnityEngine.Object.DestroyImmediate (refs.shader, true);// 这里绝不关联到
				#endif
			}
			dictShaderRefs.Clear ();
			dictName2Path.Clear ();
			if (ab != null) {
				ab.Unload (true);
			}
		}

		public string CoreDataBundle { get { return coreDataBundle; } }

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

		IEnumerator AssetBundleTask ()
		{
			Byte[] bytes = null;
			string err = "";

			IDownloader downloader = DownloaderFactory.Instance.Downloader;
			yield return downloader.ResourceTask (coreDataBundle, (bs, e) => {
				bytes = bs;
				err = e;
			});


			if (!string.IsNullOrEmpty (err)) {
				actOnComplate (ShaderLoadResult.BundleDownloadError, err);
				yield break;
			}
			yield return null;

			ShaderLoadResult error = ShaderLoadResult.Ok;
			string info = "";

			/*AssetBundleCreateRequest abcq = AssetBundle.LoadFromMemoryAsync (bytes);
			while (!abcq.isDone) {
				yield return abcq;
			}
			ab = abcq.assetBundle;*/
			ab = AssetBundle.LoadFromMemory (bytes);


			if (ab != null) {
				/*AssetBundleRequest abr = ab.LoadAssetAsync (shaderDictPath);
				while (!abr.isDone) {
					yield return abr;
				}
				UnityEngine.Object asset = abr.asset;*/
				UnityEngine.Object asset = ab.LoadAsset (shaderDictPath);

				if (asset != null && asset is TextAsset) {
					TextAsset textAsset = asset as TextAsset;
					JsonData data = null;
					try {
						data = JsonMapper.ToObject (textAsset.text);
					} catch (Exception e) {
						error = ShaderLoadResult.DictionaryJsonError;
						info = e.Message + " in " + textAsset.text;
					}

					if (data != null && !BuildDictName2Path (data)) {
						error = ShaderLoadResult.ContentCheckError;
						info = textAsset.text;
					}

				} else {
					error = ShaderLoadResult.DictionaryLoadError;
					info = shaderDictPath;
				}

			} else {
				error = ShaderLoadResult.BundleLoadError;
				info = coreDataBundle;
			}
			actOnComplate (error, info);
		}

		private bool BuildDictName2Path (JsonData data)
		{
			string[] names = ab.GetAllAssetNames ();
			IEnumerator e = data.Keys.GetEnumerator ();
			while (e.MoveNext ()) {
				string key = e.Current as string;
				string value = (string)data [key];

				// the shader in dict must exist in ab
				int index = Array.IndexOf<string> (names, value.ToLower ());
				if (index < 0) {
					return false;
				}

				dictName2Path.Add (key, value);
			}
			return true;
		}

		public string[] GetShaderList ()
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

