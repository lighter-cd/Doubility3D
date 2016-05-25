using System;
using UnityEngine;

namespace Doubility3D.Resource.ResourceObj
{
	public class ResourceObject
	{
		protected UnityEngine.Object unity3dObject = null;

		public UnityEngine.Object Unity3dObject { get{ return unity3dObject;} }
		virtual public int dependencePathes  { get { return 0; }}
		virtual public string[] DependencePathes  {  get { return null; } }
		virtual public void OnDependencesFinished(){}
		/// <summary>
		/// Releases all resource used by the <see cref="Doubility3D.Resource.ResourceObj.ResourceObject"/> object.
		/// </summary>
		/// 1.释放自己的unity3dObject,不释放自己的资源引用计数
		/// 2.通过资源管理器释放依赖资源的引用计数,不释放依赖资源的unity3dObject
		/// </remarks>
		virtual public void Dispose(){
			// 删除资源物体
			#if !UNITY_EDITOR
			UnityEngine.Object.Destroy(resource.Unity3dObject); 
			#else
			UnityEngine.Object.DestroyImmediate(unity3dObject,true);// 这里绝不关联到
			#endif
		}
	}
}

