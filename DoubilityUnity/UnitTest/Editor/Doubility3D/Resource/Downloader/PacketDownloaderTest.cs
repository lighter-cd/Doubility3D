using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	/// <summary>
	/// Packet downloader test.
	/// 需要两个压缩文件来进行测试
	/// </summary>
	[TestFixture]
	public class PacketDownloaderTest
	{
		const string packetName = "test_packet";
		const string fileNameBass = "test_data_bass.dat";
		const string fileNameUpdate = "test_data_update.dat";

		IDownloader downloader;
		string bassPacketPath;
		string updatePacketPath;
		byte[] bassBytes;
		byte[] updateBytes;

		[TestFixtureSetUp]
		public void Init ()
		{
			downloader = DownloaderFactory.CreatePacketDownloader (packetName);

			// 拷贝底包
			PreparePacket(ref bassPacketPath,ref bassBytes,fileNameBass,"_bass.pak");

			// 向底包中添加
			PreparePacket(ref updatePacketPath,ref updateBytes,fileNameUpdate,"_update.pak");
		}
		void PreparePacket(ref string packetPath,ref byte[] bytes,string filePath,string srcPrefix){
			packetPath = System.IO.Path.Combine(Application.streamingAssetsPath,packetName + ".pak");
			string srcBassPacketPath = System.IO.Path.Combine(TestData.testResource_path,packetName + "_bass.pak");
			System.IO.File.Copy (srcBassPacketPath, bassPacketPath);

			bytes = RandomData.Build (2048, 512);
			bool writed = RandomData.AddToPacket (bytes, fileNameBass,packetPath);
			Assert.IsTrue (writed);
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			downloader = null;
			// 删除两个压缩包
			System.IO.File.Delete(bassPacketPath);
			System.IO.File.Delete(updatePacketPath);
		}


		[Test]		
		public void HomeVar ()
		{
			Assert.IsInstanceOf<PacketDownloader> (downloader);
			Assert.AreEqual ((downloader as PacketDownloader).Home, packetName);
		}
		[Test]		
		public void ReadError ()
		{
			PacketDownloader fd = downloader as PacketDownloader;
			Assert.IsNotNull (fd);

			CoroutineTest.Run (fd.ResourceTask ("NotExistFile.Dat", (bytes, error) => {
				Assert.IsNull (bytes);
				Assert.IsFalse (string.IsNullOrEmpty (error));
			}));
		}
		[Test]		
		public void DataValidInBassPacket ()
		{
			DataValid (fileNameBass, bassBytes);
		}
		[Test]
		public void DataValidInUpdatePacket ()
		{
			DataValid (fileNameUpdate, updateBytes);
		}
		void DataValid(string targetPath,byte[] bytes)
		{
			PacketDownloader fd = downloader as PacketDownloader;
			Assert.IsNotNull (fd);
			CoroutineTest.Run (
				fd.ResourceTask (targetPath, (results, error) => {
					Assert.IsNotNull (results);
					Assert.AreEqual (bytes.Length, results.Length);
					Assert.IsTrue (string.IsNullOrEmpty (error));
					for (int i = 0; i < bytes.Length; i++) {
						Assert.AreEqual (bytes [i], results [i]);
					}
				})
			);
		}
	}
}