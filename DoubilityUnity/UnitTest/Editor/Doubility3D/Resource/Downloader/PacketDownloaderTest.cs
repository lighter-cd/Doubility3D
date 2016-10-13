using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Security.AccessControl;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;
using Ionic.Zip;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	[TestFixture]
	public class PacketDownloaderPacketError
	{
		const string packetName = "test_packet";
		string bassPacketPath;

		[TestFixtureSetUp]
		public void Init ()
		{
			// 拷贝底包
			bassPacketPath = System.IO.Path.Combine(Application.streamingAssetsPath,packetName + "_bass.pak");
		}
		[TearDown]
		public void CleanUp(){
			DownloaderFactory.Dispose ();
			try {
				System.IO.File.Delete (bassPacketPath);
			} catch (Exception e) {
				Debug.Log (e.Message);
			}
		}
		[Test]
		public void ZeroBytes ()
		{
			// 创建文件
			FileStream writeStream = new FileStream(bassPacketPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
			writeStream.Dispose();

			Assert.Throws<PacketException> ( new TestDelegate(()=>{
				DownloaderFactory.Instance.CreatePacketDownloader (packetName);
			}) );
		}
		[Test]
		public void NotZip(){
			// 创建文件
			FileStream writeStream = new FileStream(bassPacketPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
			byte[] bytes = RandomData.Build (2048, 512);
			writeStream.Write (bytes, 0, bytes.Length);
			writeStream.Dispose();

			// 写入文件
			bool writed = RandomData.WriteToFile (bytes, bassPacketPath);
			Assert.IsTrue (writed);

			Assert.Throws<PacketException> ( new TestDelegate(()=>{
				DownloaderFactory.Instance.CreatePacketDownloader (packetName);
			}) );
		}
		[Test]
		public void BadZip(){
			MemoryStream ms = new MemoryStream ();

			ZipFile zip = new ZipFile ();
			byte[] bytes = RandomData.Build (2048, 512);
			zip.AddEntry ("OneFileInBadZip.data", bytes);
			zip.Save (ms);
			zip.Dispose ();

			bytes = new byte[ms.Length];
			Array.Copy (ms.GetBuffer (), bytes, ms.Length);
			for (int i = 0; i < 10; i++) {
				bytes [bytes.Length -1 - i] = 0xff;
			}
			FileStream writeStream = new FileStream(bassPacketPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
			writeStream.Write (bytes, 0, bytes.Length);
			writeStream.Dispose();

			Assert.Throws<PacketException> ( new TestDelegate(()=>{
				DownloaderFactory.Instance.CreatePacketDownloader (packetName);
			}) );
		}

		[Test]
		public void BadEntry(){
			const string fileName = "OneFileInBadZip.Data";

			MemoryStream ms = new MemoryStream ();

			ZipFile zip = new ZipFile ();
			byte[] bytes = RandomData.Build (2048, 512);
			ZipEntry e = zip.AddEntry (fileName, bytes);
			zip.Save (ms);
			zip.Dispose ();

			bytes = new byte[ms.Length];
			Array.Copy (ms.GetBuffer (), bytes, ms.Length);
			// make entry bad
			for (int i = 30; i < 51; i++) {
				bytes [i] = 0xff;
			}
			FileStream writeStream = new FileStream(bassPacketPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
			writeStream.Write (bytes, 0, bytes.Length);
			writeStream.Dispose();

			Assert.DoesNotThrow (new TestDelegate (() => {
				DownloaderFactory.Instance.CreatePacketDownloader (packetName);
			}));		


			Assert.Throws<PacketException> (new TestDelegate (() => {
				IDownloader fd = DownloaderFactory.Instance.Downloader;
				Assert.IsNotNull (fd);

				IEnumerator enumerator = fd.ResourceTask (fileName, null);
				bool completed = enumerator.RunCoroutineWithoutYields (int.MaxValue);
				Assert.IsTrue (completed);
			}));
		}

		[Test]
		public void NoVersionNumber(){
			ZipFile zip = new ZipFile ();
			byte[] bytes = RandomData.Build (2048, 512);
			zip.AddEntry ("OneFileInBadZip.data", bytes);
			zip.Save (bassPacketPath);
			zip.Dispose ();

			Assert.Throws<PacketException> ( new TestDelegate(()=>{
				DownloaderFactory.Instance.CreatePacketDownloader (packetName);
			}) );

		}
		[Test]
		public void WrongVersionNumber(){
			ZipFile zip = new ZipFile ();
			byte[] bytes = RandomData.Build (2048, 512);
			zip.AddEntry ("OneFileInBadZip.data", bytes);
			zip.Comment = "NotAInt";
			zip.Save (bassPacketPath);
			zip.Dispose ();

			Assert.Throws<PacketException> ( new TestDelegate(()=>{
				DownloaderFactory.Instance.CreatePacketDownloader (packetName);
			}) );

		}
		[Test]
		public void VersionNumberValid(){
			ZipFile zip = new ZipFile ();
			byte[] bytes = RandomData.Build (2048, 512);
			zip.AddEntry ("OneFileInBadZip.data", bytes);
			zip.Comment = 15.ToString();
			zip.Save (bassPacketPath);
			zip.Dispose ();

			Assert.DoesNotThrow ( new TestDelegate(()=>{
				DownloaderFactory.Instance.CreatePacketDownloader (packetName);
			}) );
		}

		private void MakeEntryBad(byte[] bytes){
			for (int i = 30; i < 51; i++) {
				bytes [i] = 0xff;
			}
		}
	}


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
			bassPacketPath = System.IO.Path.Combine(Application.streamingAssetsPath,packetName + "_bass.pak");
			PreparePacket(dictBassPacket,bassPacketPath,0,0);

			// 向底包中添加
			updatePacketPath = System.IO.Path.Combine(Application.persistentDataPath,packetName + "_update.pak");
			PreparePacket(dictUpdatePacket,updatePacketPath,10,dictBassPacket.Count);

			// 初始化时，数据包必须已经存在。
			DownloaderFactory.Instance.CreatePacketDownloader (packetName);
			downloader = DownloaderFactory.Instance.Downloader;
		}
		void PreparePacket(Dictionary<string,byte[]> dict,string packetPath,int version,int startIndex){
			ZipFile zip = new ZipFile ();

			/// 随机文件数
			int files = RandomData.Random.Next(2,5);
			for (int i = 0; i < files; i++) {
				/// 每个随机文件内容
				string fileName = string.Format (filePrefix, i + 1 + startIndex);
				byte[] bytes = RandomData.Build (2048, 512);
				dict.Add (fileName, bytes);

				zip.AddEntry(fileName,bytes);
			}
			zip.Comment = version.ToString ();
			zip.Save(packetPath);
			zip.Dispose ();
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			DownloaderFactory.Dispose ();
			downloader = null;
			try {
				System.IO.File.Delete (bassPacketPath);
				System.IO.File.Delete (updatePacketPath);
			} catch (Exception e) {
				Debug.Log (e.Message);
			}

			dictBassPacket.Clear ();
			dictUpdatePacket.Clear ();
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