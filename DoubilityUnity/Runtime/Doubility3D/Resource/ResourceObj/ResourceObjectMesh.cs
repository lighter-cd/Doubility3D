using System;
using UnityEngine;

namespace Doubility3D.Resource.ResourceObj
{
	public class ResourceObjectMesh : ResourceObject
	{
		private string[] _joints;
		public string[] joints{ get { return _joints; } }
		public ResourceObjectMesh(Mesh mesh,string[] js){
			unity3dObject = mesh;
			_joints = js;
		}
	}
}

