using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;
using Ionic.Zip;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	/// <summary>
	/// Packet downloader test.
	/// todo:文件名也随机
	/// </summary>
	[TestFixture]
	public class PacketDownloaderTest
	{
		const string packetName = "test_packet";
		const string filePrefix = "file_{0}.dat";

		IDownloader downloader;
		string bassPacketPath;
		string updatePacketPath;
		Dictionary<string,byte[]> dictBassPacket = new Dictionary<string, byte[]>();
		Dictionary<string,byte[]> dictUpdatePacket = new Dictionary<string, byte[]>();

		[TestFixtureSetUp]
		public void Init ()
		{
			// 拷贝底包
			bassPacketPath = PreparePacket(dictBassPacket,"_bass",0,0);

			// 向底包中添加
			updatePacketPath = PreparePacket(dictUpdatePacket,"_update",10,dictBassPacket.Count);

			// 初始化时，数据包必须已经存在。
			downloader = DownloaderFactory.CreatePacketDownloader (packetName);
		}
		string PreparePacket(Dictionary<string,byte[]> dict,string packetPostfix,int version,int startIndex){

			string packetPath = System.IO.Path.Combine(Application.streamingAssetsPath,packetName + packetPostfix +".pak");
			ZipFile zip = new ZipFile ();

			/// 随机文件数
			int files = RandomData.Random.Next(2,5);
			for (int i = 0; i < files; i++) {
				/// 每个随机文件内容
				string fileName = string.Format (filePrefix, i + 1 + startIndex);
				byte[] bytes = RandomData.Build (2048, 512);
				dict.Add (fileName, bytes);

				ZipEntry e= zip.AddEntry(fileName,bytes);
			}
			zip.Comment = version.ToString ();
			zip.Save(packetPath);

			return packetPath;
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
		public void PacketMustHasVersionNumber(){
		}
		[Test]		
		public void ReadError ()
		{
			PacketDownloader fd = downloader as PacketDownloader;
			Assert.IsNotNull (fd);

			IEnumerator enumerator = fd.ResourceTask ("NotExistFile.Dat", (bytes, error) => {
				Assert.IsNull (bytes);
				Assert.IsFalse (string.IsNullOrEmpty (error));
			});
			bool completed = enumerator.RunCoroutineWithoutYields (int.MaxValue);
			Assert.IsTrue (completed);
		}
		[Test]		
		public void DataValidInBassPacket ()
		{
			DataValid (dictBassPacket,"_bass");
		}
		[Test]
		public void DataValidInUpdatePacket ()
		{
			DataValid (dictUpdatePacket,"_update");
		}
		[Test]
		public void DataValidAnyWhere(){
		}
		[Test]
		public void DataNewestInUpdatePacket(){
		}

		void DataValid(Dictionary<string,byte[]> dict,string packetPostfix)
		{
			PacketDownloader fd = downloader as PacketDownloader;
			Assert.IsNotNull (fd);

			Dictionary<string,byte[]>.Enumerator e = dict.GetEnumerator ();
			while (e.MoveNext ()) {
				string targetPath = e.Current.Key;
				byte[] bytes = e.Current.Value;

				IEnumerator enumerator = fd.ResourceTask (targetPath, (results, error) => {
					Assert.IsNotNull (results);
					Assert.AreEqual (bytes.Length, results.Length);
					Assert.IsTrue (string.IsNullOrEmpty (error));
					for (int i = 0; i < bytes.Length; i++) {
						Assert.AreEqual (bytes [i], results [i]);
					}
				});
				bool completed = enumerator.RunCoroutineWithoutYields (int.MaxValue);
				Assert.IsTrue (completed);				
			}
		}
	}
}