using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System;
using Doubility3D.Util;
using Doubility3D.Resource.Saver;
using FlatBuffers;
using Schema = Doubility3D.Resource.Schema;

public class TextureExporter {
	[MenuItem("逗逼工具/准备数据/纹理输出")]
	static void DoIt(){
		string[] files = System.IO.Directory.GetFiles("Assets/ArtWork","*.*",System.IO.SearchOption.AllDirectories);
		files = files.Where(s=>s.EndsWith(".tga")||s.EndsWith(".png")||s.EndsWith(".jpg")).ToArray();
		files = Array.ConvertAll<string,string>(files,new Converter<string,string>((s)=>{return s.Replace('\\','/');}));

		if((EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android) &&
			(EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS) &&
			(EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows) &&
			(EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)){
			EditorUtility.DisplayDialog("不得行",EditorUserBuildSettings.activeBuildTarget.ToString() + "是不支持的平台","晓得不嘛");
		}

		string outputFolder = Application.streamingAssetsPath + "/.textureBundle/" + TargetPath.GetPath(EditorUserBuildSettings.activeBuildTarget);
		if(!System.IO.Directory.Exists(outputFolder)){
			System.IO.Directory.CreateDirectory(outputFolder);
		}
		string[]names = files;
		for(int i=0;i<names.Length;i++){
			Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(names[i]);

			string path = names[i].Substring("Assets/ArtWork/".Length);
			path = System.IO.Path.GetDirectoryName(path) + "/" + System.IO.Path.GetFileNameWithoutExtension(path);
			path = Application.streamingAssetsPath + "/.root/" + path + "." + TargetPath.GetPath(EditorUserBuildSettings.activeBuildTarget).ToLower() + ".texture";

			string folder = System.IO.Path.GetDirectoryName(path);
			if(!System.IO.Directory.Exists(folder)){
				System.IO.Directory.CreateDirectory(folder);
			}

			ByteBuffer bb = TextureSaver.Save(texture);
			FileSaver.Save(bb,Schema.Context.Texture,path);
		}

		names = null;
		EditorUtility.DisplayDialog("搞定了","纹理输出完毕","好了");
	}
}
