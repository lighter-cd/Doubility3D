using UnityEngine;
using UnityEditor;

using System.IO;
using System.Text;
using System;

using LitJson;
using Doubility3D.Util;
using UnitTest.Doubility3D;

public class AssetBundleForUnitTest : ScriptableObject
{
	public class BuildMaps{
		public AssetBundleBuild[] buildMap;
	}

	[MenuItem("逗逼工具/单元测试/打包测试数据")]
	static void DoIt()
    {
        if (!System.IO.File.Exists(TestData.config_path))
        {
			EditorUtility.DisplayDialog("不得行","找不到"+TestData.config_path,"咋办喃");
			return;
		}

		string output_folder = TestData.testBundle_path + TargetPath.GetPath(EditorUserBuildSettings.activeBuildTarget);
		if(!System.IO.Directory.Exists(output_folder)){
			System.IO.Directory.CreateDirectory(output_folder);
		}

        string json = System.IO.File.ReadAllText(TestData.config_path);
		BuildMaps maps = JsonMapper.ToObject<BuildMaps>(json);

		BuildPipeline.BuildAssetBundles(output_folder,maps.buildMap,BuildAssetBundleOptions.None,EditorUserBuildSettings.activeBuildTarget);
    }
}