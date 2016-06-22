using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Doubility3D.Util;
using FlatBuffers;
using Doubility3D.Resource.Schema;
using Doubility3D.Resource.Unserializing;
using Schema = Doubility3D.Resource.Schema;


namespace UnitTest.Doubility3D
{
    static public class TestData
    {
        /// <summary>
        /// 所有测试数据的根目录
        /// </summary>
        public const string testData_path = "Assets/Doubility3D/UnitTest/.TestData/";
        /// <summary>
        /// 测试所需要的 asset bundle 数据目录
        /// </summary>
        public const string testBundle_path = testData_path + "Bundle/";
        /// <summary>
        /// 测试所需要的逗逼文件格式的数据目录
        /// </summary>
        public const string testResource_path = testData_path + "Resource/";
		/// <summary>
		/// 测试所需要的逗逼文件格式的配置文件目录
		/// </summary>
		public const string testConfig_path = "Assets/Doubility3D/UnitTest/Editor/Config/";
        /// <summary>
        /// 生成测试所需要的 asset bundle 的配置文件路径
        /// </summary>
		public const string config_path = "Assets/Doubility3D/UnitTest/assetbundle.json";
		/// <summary>
		/// 拷贝测试所需要的资源 的配置文件路径
		/// </summary>
		public const string config_resource = "Assets/Doubility3D/UnitTest/resource.json";

        /// <summary>
        /// 从 testBundle_path 目录装载指定名称的 asset bundle
        /// </summary>
        /// <param name="bundleName"> asset bundle 文件名 不需要指定目录 </param>
        /// <returns></returns>
        static public AssetBundle LoadBundle(string bundleName)
        {
            string testData_folder = testBundle_path + PlatformPath.GetPath(Application.platform);
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
        static public ByteBuffer LoadResource(string resourceName,out Context context)
        {
			return FileUnserializer.LoadFromFile(testResource_path + resourceName, out context);
        }
    }
}
