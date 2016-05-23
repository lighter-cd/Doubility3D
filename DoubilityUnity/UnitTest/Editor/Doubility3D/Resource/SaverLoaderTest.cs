using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FlatBuffers;
using Doubility3D.Resource.Saver;
using Doubility3D.Resource.Serializer;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using UnitTest.Doubility3D;
using System;

namespace UnitTest.Doubility3D.Resource
{
	[TestFixture]
	public class SaverLoaderTest
	{
		[Test]
		public void SaveToFileAndReadBack ()
		{
			System.Random random = new System.Random (System.Environment.TickCount);

			int dataLength = 2048 + random.Next (0, 512);
			byte[] bytes = new byte[dataLength];
			for (int i = 0; i < dataLength; i++) {
				random.NextBytes (bytes);
			}

			// todo:应该使用反射，取得枚举的最大可能值。
			Schema.Context context = (Schema.Context)random.Next ((int)Schema.Context.Skeletons, (int)Schema.Context.AnimationClip);

			const string filePath = TestData.testData_path + "TestFile.doub";

			if (System.IO.File.Exists (filePath)) {
				System.IO.File.Delete (filePath);
			}
            
			ByteBuffer bb = new ByteBuffer (bytes);
			FileSaver.Save (bb, context, filePath);

			Schema.Context out_context = Context.Unknown;
			ByteBuffer bbOut = FileSerializer.LoadFromFile (filePath, out out_context);

			Assert.AreEqual (context, out_context);
			Assert.AreEqual (dataLength, bbOut.Length - bbOut.Position);
			for (int i = 0; i < dataLength; i++) {
				Assert.AreEqual (bytes [i], bbOut.Data [bbOut.Position + i]);
			}

			if (System.IO.File.Exists (filePath)) {
				System.IO.File.Delete (filePath);
			}
		}
	}
}
