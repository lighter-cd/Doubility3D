using System;
using UnityEngine;
using Doubility3D.Resource.Manager;

namespace Doubility3D.Resource.ResourceObj
{
	/// <summary>
	/// Resource interface.
	/// 对于包含了其他资源和shader的ResourceObject,需要使用此类来解耦和。
	/// </summary>
	public class ResourceInterface
	{
		public static Func<string,Shader> funcAddShader = ShaderManager.Instance.AddShader;//
		public static Action<Shader> actDelShader = ShaderManager.Instance.DelShader;
		public static Func<string,ResourceRef> funcGetResource = ResourceManager.Instance.getResource;
		public static Action<string> actDelResource = ResourceManager.Instance.delResource;
	}
}
