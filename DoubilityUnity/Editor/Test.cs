using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Doubility3D
{
	static public class Test
	{
		[MenuItem ("逗逼引擎/搞一哈!")]
		static void DoIt()
		{
			if(Selection.activeObject is Material)
			{
				Material mat = Selection.activeObject as Material;
				int count = ShaderUtil.GetPropertyCount(mat.shader);
				for(int i=0;i<count;i++)
				{
					string name = ShaderUtil.GetPropertyName(mat.shader,i);
					ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(mat.shader,i);
					UnityEngine.Debug.Log("name:"+name+",type:"+type);
				}
			}
		}
	}
}

