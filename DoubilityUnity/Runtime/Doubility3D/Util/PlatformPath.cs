using System;
using UnityEngine;

namespace Doubility3D.Util
{
	static public class PlatformPath
	{
		static public string GetPath (RuntimePlatform platform)
		{
			switch (platform){
			case RuntimePlatform.Android:
				return RuntimePlatform.Android.ToString();
			case RuntimePlatform.IPhonePlayer:
				return "iOS";
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:	
				return "Windows";
			}
			return "Unknown";
		}
	}
}

