using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System;
using Doubility3D.Util;
using Doubility3D.Resource.Saver;
using FlatBuffers;
using Schema = Doubility3D.Resource.Schema;

namespace Doubility3D.Tools
{
	public class CoreDataBundle
	{
		[MenuItem ("逗逼工具/准备数据/核心数据打包")]
		static void DoIt ()
		{
			{
				string[] files = System.IO.Directory.GetFiles ("Assets/Doubility3D/CoreData", "*.*", System.IO.SearchOption.AllDirectories);
				files = files.Where (s => s.EndsWith (".mat") || s.EndsWith (".exr") || s.EndsWith (".shader")).ToArray ();
				files = Array.ConvertAll<string,string> (files, new Converter<string,string> ((s) => {
					return s.Replace ('\\', '/');
				}));

				AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
				buildMap [0].assetBundleName = "coreData.bundle";
				buildMap [0].assetNames = files;


				if ((EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android) &&
				    (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS) &&
				    (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows) &&
				    (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)) {
					EditorUtility.DisplayDialog ("不得行", EditorUserBuildSettings.activeBuildTarget.ToString () + "是不支持的平台", "晓得不嘛");
				}

				string outputFolder = Application.streamingAssetsPath + "/.coreData/" + TargetPath.GetPath (EditorUserBuildSettings.activeBuildTarget);
				if (!System.IO.Directory.Exists (outputFolder)) {
					System.IO.Directory.CreateDirectory (outputFolder);
				}
				BuildPipeline.BuildAssetBundles (outputFolder, buildMap, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);	
				EditorUtility.DisplayDialog ("搞定了", "核心数据输出完毕", "好了");
			}
		}
	}
}