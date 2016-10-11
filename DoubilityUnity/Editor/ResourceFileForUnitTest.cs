using UnityEngine;
using UnityEditor;

using System.IO;
using System.Text;
using System;

using LitJson;
using Doubility3D.Util;
using UnitTest.Doubility3D;

public class ResourceFileForUnitTest : ScriptableObject {

	public class CopyMaps{
		public string[] copyMap;
	}

	[MenuItem("逗逼工具/单元测试/拷贝测试资源")]
	static void DoIt(){
		if (!System.IO.File.Exists(TestData.config_resource))
		{
			EditorUtility.DisplayDialog("不得行","找不到"+TestData.config_resource,"咋办喃");
			return;
		}

		if(!System.IO.Directory.Exists(TestData.testResource_path)){
			System.IO.Directory.CreateDirectory(TestData.testResource_path);
		}

		string json = System.IO.File.ReadAllText(TestData.config_resource);
		CopyMaps maps = JsonMapper.ToObject<CopyMaps>(json);

		string home = Environment.GetEnvironmentVariable ("DOUBILITY_HOME", EnvironmentVariableTarget.User);

		for(int i=0;i<maps.copyMap.Length;i++){
			string source = home + "/.root/" + maps.copyMap[i];
			string dest = TestData.testResource_path + System.IO.Path.GetFileName(maps.copyMap[i]);
			if (System.IO.File.Exists (source)) {
				System.IO.File.Copy (source, dest, true);
			} else {
				EditorUtility.DisplayDialog("搞不定","文件不存在："+source,"咋办");
			}
		}
		EditorUtility.DisplayDialog("搞定了","拷贝完毕","好了");
	}
}
