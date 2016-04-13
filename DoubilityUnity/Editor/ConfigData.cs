using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LitJson;

namespace Doubility3D
{
    class ConfigData
    {
        public string assetRoot = string.Empty;
        public int selectType = 0;
        public string bsonSelect = string.Empty;
        public int filterType = 0;
        public bool regularExp = false;
        public string filter = string.Empty;
    }
	static class ConfigDataFile
	{
		const string fileName = "ResourceExporter.json";
		static public ConfigData Load()
		{
			if (System.IO.File.Exists(fileName))
			{
				string jsonString = System.IO.File.ReadAllText(fileName);
				JsonReader reader = new JsonReader(jsonString);
				ConfigData data = JsonMapper.ToObject<ConfigData>(reader);
				return data;
			}
			return new ConfigData();
		}
		static public void Save(ConfigData data)
		{
			string jsonString = JsonMapper.ToJson(data);
			System.IO.File.WriteAllText(fileName,jsonString);
		}		
	}
}
