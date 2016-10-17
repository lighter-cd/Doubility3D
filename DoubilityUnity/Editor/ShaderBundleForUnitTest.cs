using UnityEngine;
using UnityEditor;

using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

using LitJson;
using Doubility3D.Util;
using UnitTest.Doubility3D;

public class ShaderBundleForUnitTest
{
	[MenuItem("逗逼工具/单元测试/打包ShaderManager测试数据")]
	static void DoIt ()
	{
		BuildShaderBundle ("ShaderNoDict",(dict)=>{return null;});
		BuildShaderBundle ("ShaderDictError", (dict)=>{return "I'm not a json";});
		BuildShaderBundle ("ShaderContentError", (dict)=>{
			dict.Add("I'm not in assetbundle","who care shader's name");
			return JsonMapper.ToJson (dict);
		});
		BuildShaderBundle ("ShaderOk", (dict)=>{
			return JsonMapper.ToJson (dict);
		});
	}
	static void BuildShaderBundle (string bundleName,Func<Dictionary<string,string>,string> funcShaderDict)
	{
		string[] _files = System.IO.Directory.GetFiles ("Assets/Doubility3D/CoreData", "*.shader", System.IO.SearchOption.AllDirectories);
		_files = Array.ConvertAll<string,string> (_files, new Converter<string,string> ((s) => {
			return s.Replace ('\\', '/');
		}));

		AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
		buildMap [0].assetBundleName = bundleName + ".assetbundle";
		buildMap [0].assetNames = new string[1];
		buildMap [0].assetNames[0] = _files [0];


		Dictionary<string,string> dictShaderName2Path = new Dictionary<string, string> ();
		Shader shader = AssetDatabase.LoadAssetAtPath<Shader> (buildMap [0].assetNames[0]);
		dictShaderName2Path.Add (shader.name, _files [0]);

		string output_folder = TestData.testBundle_path;
		string jsonString = funcShaderDict (dictShaderName2Path) ;
		if(jsonString != null){
			System.IO.File.WriteAllText ("Assets/Doubility3D/UnitTest/" + bundleName + ".json", jsonString);

			Array.Resize<string> (ref buildMap [0].assetNames, buildMap [0].assetNames.Length + 1);
			buildMap [0].assetNames [buildMap [0].assetNames.Length - 1] = "Assets/Doubility3D/UnitTest/" + bundleName + ".json";
			AssetDatabase.Refresh ();
		}

		BuildPipeline.BuildAssetBundles (output_folder, buildMap, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
		AssetDatabase.Refresh ();
		if (jsonString != null) {
			System.IO.File.Delete ("Assets/Doubility3D/UnitTest/" + bundleName + ".json");
			AssetDatabase.Refresh ();
		}
	}
}

