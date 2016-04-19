using System;
using UnityEngine;
using UnityEditor;

namespace Doubility3D.Util
{
	static public class TargetPath
	{
		static public string GetPath (BuildTarget target)
		{
			switch(target){
			case BuildTarget.Android:
				return PlatformPath.GetPath(RuntimePlatform.Android);
			case BuildTarget.iOS:
				return PlatformPath.GetPath(RuntimePlatform.IPhonePlayer);
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return PlatformPath.GetPath(RuntimePlatform.WindowsPlayer);
			}
			return "Unknown";
		}
	}
}

