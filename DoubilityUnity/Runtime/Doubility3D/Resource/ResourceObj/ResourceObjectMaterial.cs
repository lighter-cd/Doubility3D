using System;
using System.Collections.Generic;
using UnityEngine;
using Doubility3D.Resource.Manager;
using Doubility3D.Util;

namespace Doubility3D.Resource.ResourceObj
{
	public class ResourceObjectMaterial : ResourceObject
	{
		class TextureParam
		{
			public string path;
			public string propertyName;
			public TextureParam(string _path,string _propertyName){
				path = _path;
				propertyName = _propertyName;
			}
		}
		private List<TextureParam> lstTextureParams = new List<TextureParam> (8);
		private List<string> dependencesPath = new List<string>(8);

		static private string platform;
		static ResourceObjectMaterial(){
			platform = PlatformPath.GetPath (Application.platform).ToLower ();
		}

		public ResourceObjectMaterial(string shaderName){
			Shader shader = ResourceObjectInterface.funcAddShader(shaderName);
			if (shader != null) {
				UnityEngine.Material material = new UnityEngine.Material(shader);
				unity3dObject = material;
			}
		}
		public void AddTexture(string path,string propertyName){
			lstTextureParams.Add (new TextureParam (GetTexturePath(path),propertyName));
			dependencesPath.Add(GetTexturePath(path));
		}
		override public string[] DependencePathes {	get{return dependencesPath.ToArray();}}
		override public void OnDependencesFinished(){
			Material material = unity3dObject as Material;
			for (int i = 0; i < lstTextureParams.Count; i++) {
				ResourceRef res = ResourceObjectInterface.funcGetResource (lstTextureParams [i].path);
				if (res != null) {
					material.SetTexture (lstTextureParams [i].propertyName, res.resourceObject.Unity3dObject as Texture);
				}
			}
		}
		override public void Dispose(){
			Material material = unity3dObject as Material;
			ResourceObjectInterface.actDelShader (material.shader);

			// 删除资源物体
			base.Dispose();

			lstTextureParams.Clear ();
			lstTextureParams = null;
		}
		public string GetTexturePath(string path){
			return path + "." + platform + ".texture";
		}
	}
}

