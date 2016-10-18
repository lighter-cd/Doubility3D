using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doubility3D.Resource.Downloader;
using Doubility3D.Resource.Manager;
using Doubility3D.Util;

namespace Integration.ShaderManagerTest
{
	public class TestFixtureRoot : TestFixture
	{
		string oldShaderDict;
		string fullPath;

		protected override void TestFixtureSetup ()
		{
			DownloaderFactory.Instance.InitializeWithConfig("file_mode.json");
			oldShaderDict = ShaderManager.shaderDictPath;
		}

		protected override void TestFixtureTeardown ()
		{
			ShaderManager.Instance.DisposeBundle ();
			ShaderManager.shaderDictPath = oldShaderDict;
			DownloaderFactory.Dispose ();
		}

		public string Where { get { return "Assets/Doubility3D/UnitTest/"; } }
		public string BundlePrefix { get { return ".UnitTest/.coreData/" + PlatformPath.GetPath (Application.platform) + "/";} }
	}
}
