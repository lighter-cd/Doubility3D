using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Doubility3D
{
	public class ModelGroupExporter : EditorWindow
	{
		const string targetFolder = "Assets/StreamingAssets/.root";
		const string sourceFolder = "Assets/ArtWork";

		ConfigData configData;
		private GUIStyle textFieldStyles;

		private string[] selectTypeString = { "全部导出", "导出文件夹", "导出文件" };
		private int[] selectTypeIndex = { 0, 1, 2 };
		private string[] filterTypeString = { "无", "符合条件", "排除条件" };
		private int[] filterTypeIndex = { 0, 1, 2 };

		[MenuItem ("逗逼雷踢/转换模型组")]

		static void Init ()
		{
			// Get existing open window or if none, make a new one:
			ModelGroupExporter window = (ModelGroupExporter)EditorWindow.GetWindow (typeof(ModelGroupExporter));
			window.titleContent = new GUIContent ("资源转换器");
		}

		private void OnGUI ()
		{
			if (textFieldStyles == null) {
				textFieldStyles = new GUIStyle (EditorStyles.popup);
				textFieldStyles.fontSize = 12;
			}
			if (configData == null) {
				configData = ConfigDataFile.Load ();
			}


			#region 界面布局
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.Space ();
			EditorGUILayout.BeginHorizontal ();
			configData.assetRoot = EditorGUILayout.TextField ("资源根目录", configData.assetRoot);
			if (GUILayout.Button ("...", GUILayout.Width (25))) {
				string folder = EditorUtility.OpenFolderPanel ("选择源目录", string.Empty, string.Empty);
				if (folder != string.Empty) {
					configData.assetRoot = folder;
				}
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();
			configData.selectType = EditorGUILayout.IntPopup ("导出方式", configData.selectType, selectTypeString, selectTypeIndex, textFieldStyles);
			if (configData.selectType != 0) {
				EditorGUILayout.BeginHorizontal ();
				configData.pathSelect = EditorGUILayout.TextField ("源文件", configData.pathSelect);
				if (GUILayout.Button ("...", GUILayout.Width (25))) {
					if (configData.selectType == 1) {
						string folder = EditorUtility.OpenFolderPanel ("选择源目录", configData.assetRoot, string.Empty);
						if (folder != string.Empty) {
							configData.pathSelect = folder;
						}
					} else if (configData.selectType == 2) {
						string[] filter = { "原始模型", "fbx", "预置模型", "prefab", "材质文件", "mat", "All files", "*" };
						string file = EditorUtility.OpenFilePanelWithFilters ("选择文件", configData.assetRoot, filter);
						if (file != string.Empty) {
							configData.pathSelect = file;
						}
					}
				}
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.Separator ();
			configData.filterType = EditorGUILayout.IntPopup ("过滤方式", configData.filterType, filterTypeString, filterTypeIndex, textFieldStyles);
			if (configData.filterType != 0) {
				configData.regularExp = EditorGUILayout.Toggle ("使用正则表达式", configData.regularExp);
				configData.filter = EditorGUILayout.TextField ("过滤条件", configData.filter);
			}

			EditorGUILayout.Separator ();
			if (GUILayout.Button ("开始转换")) {
				if (configData.assetRoot == string.Empty) {
					EditorUtility.DisplayDialog ("错误", "选择根文件夹", "准备好了告诉我");
					return;
				}
				if ((configData.selectType > 0) && (configData.pathSelect == string.Empty)) {
					EditorUtility.DisplayDialog ("错误", "选择需要导出的文件或者文件夹", "准备好了告诉我");
					return;
				}
				ConvertIt ();
			}

			EditorGUILayout.EndVertical ();
			#endregion
		}

		public string getPath (string src)
		{
			src = src.Replace ("\\", "/");
			src = src.Replace (configData.assetRoot + "/", "");
			return src;
		}

		void ConvertIt ()
		{
			string[] files = null;
			if (configData.selectType == 0) {
				Debug.Log (configData.assetRoot + "是文件夹");
				files = Directory.GetFiles (configData.assetRoot, "*.fbx", SearchOption.AllDirectories);
			} else if (configData.selectType == 1) {
				Debug.Log (configData.pathSelect + "是文件夹");
				files = Directory.GetFiles (configData.pathSelect, "*.fbx", SearchOption.AllDirectories);
			} else if (configData.selectType == 2) {
				Debug.Log (configData.pathSelect + "是文件");
				files = new string[] { configData.pathSelect };
			}

			files = Array.ConvertAll<string, string> (files, new Converter<string, string> (getPath));
			Array.Sort<string> (files, new Comparison<string> ((s1, s2) => {
				return string.Compare (s1, s2);
			}));

			if (files != null) {
				// 以目录为单位进行pass
				Dictionary<string, List<string>> dictFiles = new Dictionary<string, List<string>> ();
				foreach (string file in files) {
					string key = System.IO.Path.GetDirectoryName (file);
					if (!dictFiles.ContainsKey (key)) {
						dictFiles [key] = new List<string> ();
					}
					dictFiles [key].Add (file);
				}

				foreach (var pair in dictFiles) {
					string targetDir = targetFolder + "/" + pair.Key;
					if (!System.IO.Directory.Exists (targetDir)) {
						System.IO.Directory.CreateDirectory (targetDir);
					}

					List<string> thisFiles = pair.Value;
					foreach (string file in thisFiles) {
						// 过滤
						if (configData.filterType != 0) {
							if (filter (file))
								continue;
						}

						// todo:转换文件 
						UnityModelConvert.Convert (sourceFolder + "/" + file, targetDir);
					}
				}
				Debug.Log ("全部转换完毕");
				ConfigDataFile.Save (configData);
			}
		}

		bool filter (string file)
		{
			if (configData.regularExp) {
				Match mc = Regex.Match (file, configData.filter);
				if (mc != null && mc.Success) {
					if (configData.filterType == 1) {
						return false;
					} else if (configData.filterType == 2) {
						return true;
					}
				}
			} else {
				bool subString = file.IndexOf (configData.filter) >= 0;
				if (subString) {
					if (configData.filterType == 1) {
						return false;
					} else if (configData.filterType == 2) {
						return true;
					}
				}
			}        
			return false;
		}
	}
}

