using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Doubility3D.Util;

namespace UnitTest.Doubility3D
{
    static public class TestData
    {
        public const string testData_path = "Assets/Doubility3D/UnitTest/.TestData/";

        static public AssetBundle LoadBundle(string bundleName)
        {
            string testData_folder = testData_path + PlatformPath.GetPath(Application.platform);
            AssetBundle ab = AssetBundle.LoadFromFile(testData_folder + "/" + bundleName);
            return ab;
        }
        static public T LoadFirstAsset<T>(AssetBundle ab) where T : UnityEngine.Object
        {
            string[] path = ab.GetAllAssetNames();
            if(path.Length > 0){
                return ab.LoadAsset<T>(path[0]);
            }
            return null;
        }
    }
}
