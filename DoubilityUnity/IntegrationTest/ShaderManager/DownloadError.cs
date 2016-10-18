using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doubility3D.Resource.Downloader;
using Doubility3D.Resource.Manager;
using Doubility3D.Util;
using LitJson;

namespace Integration.ShaderManagerTest
{
	public class DownloadError : TestCase
	{
		// Use this for initialization
		void Start ()
		{
			string bundle = (testFixture as TestFixtureRoot).BundlePrefix + "ShaderNotExist.assetbundle";
			ShaderManager.Instance.LoadAssetBundle (OnAssetBundle, bundle);
		}
	
		void OnAssetBundle(ShaderLoadResult result,string error){
			Debug.Assert(result == ShaderLoadResult.BundleDownloadError);
			Debug.Assert(!string.IsNullOrEmpty(error));
			IntegrationTest.Pass ();
		}
		void OnDisable(){
			//Debug.Log (name + ".OnDisable");
			ShaderManager.Instance.DisposeBundle ();			
		}
	}
}
