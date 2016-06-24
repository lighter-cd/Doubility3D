using System;
using UnityEngine;

namespace UnitTest.Doubility3D
{
	public class RandomData
	{
		static System.Random random;
		static RandomData(){
			random = new System.Random (System.Environment.TickCount);
		}

		public static byte[] Build (int lengthBase, int lengthMaxAdder)
		{
			int dataLength = lengthBase + random.Next (0, lengthMaxAdder);
			byte[] bytes = new byte[dataLength];
			for (int i = 0; i < dataLength; i++) {
				random.NextBytes (bytes);
			}
			return bytes;
		}

		public static bool WriteToFile (byte[] bytes, string path)
		{
			try {
				if (System.IO.File.Exists (path)) {
					System.IO.File.Delete (path);
				}
				// 写入文件
				System.IO.File.WriteAllBytes (path, bytes);
				return true;
			} catch (Exception e) {
				Debug.LogError (e.Message);
				return false;
			}
		}
		public static bool AddToPacket(byte[] bytes, string pathFile, string pathPacket)
		{
			return false;
		}
		public static System.Random Random {get{ return random;}}
	}
}

