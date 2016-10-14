using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Downloader;
using Ionic.Zip;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	[TestFixture]
	public class PacketDownloaderWhenError
	{
		const string packetName = "test_packet";
		string bassPacketPath;

		[TestFixtureSetUp]
		public void Init ()
		{
			// 拷贝底包
			bassPacketPath = System.IO.Path.Combine (Application.streamingAssetsPath, packetName + "_bass.pak").Replace ("\\", "/");
		}

		[TearDown]
		public void CleanUp ()
		{
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
			FileStream writeStream = new FileStream (bassPacketPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
			writeStream.Dispose ();

			Assert.Throws<PacketException> (new TestDelegate (() => {
				DownloaderFactory.Instance.Initialize (DownloadMode.Packet, packetName);
			}));
		}

		[Test]
		public void NotZip ()
		{
			// 创建文件
			FileStream writeStream = new FileStream (bassPacketPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
			byte[] bytes = RandomData.Build (2048, 512);
			writeStream.Write (bytes, 0, bytes.Length);
			writeStream.Dispose ();

			// 写入文件
			bool writed = RandomData.WriteToFile (bytes, bassPacketPath);
			Assert.IsTrue (writed);

			Assert.Throws<PacketException> (new TestDelegate (() => {
				DownloaderFactory.Instance.Initialize (DownloadMode.Packet, packetName);
			}));
		}

		[Test]
		public void BadZip ()
		{
			MemoryStream ms = new MemoryStream ();

			ZipFile zip = new ZipFile ();
			byte[] bytes = RandomData.Build (2048, 512);
			zip.AddEntry ("OneFileInBadZip.data", bytes);
			zip.Save (ms);
			zip.Dispose ();

			bytes = new byte[ms.Length];
			Array.Copy (ms.GetBuffer (), bytes, ms.Length);
			for (int i = 0; i < 10; i++) {
				bytes [bytes.Length - 1 - i] = 0xff;
			}
			FileStream writeStream = new FileStream (bassPacketPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
			writeStream.Write (bytes, 0, bytes.Length);
			writeStream.Dispose ();

			Assert.Throws<PacketException> (new TestDelegate (() => {
				DownloaderFactory.Instance.Initialize (DownloadMode.Packet, packetName);
			}));
		}

		[Test]
		public void BadEntry ()
		{
			const string fileName = "OneFileInBadZip.Data";

			MemoryStream ms = new MemoryStream ();

			ZipFile zip = new ZipFile ();
			byte[] bytes = RandomData.Build (2048, 512);
			zip.AddEntry (fileName, bytes);
			zip.Save (ms);
			zip.Dispose ();

			bytes = new byte[ms.Length];
			Array.Copy (ms.GetBuffer (), bytes, ms.Length);
			// make entry bad
			for (int i = 30; i < 51; i++) {
				bytes [i] = 0xff;
			}
			FileStream writeStream = new FileStream (bassPacketPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
			writeStream.Write (bytes, 0, bytes.Length);
			writeStream.Dispose ();

			Assert.DoesNotThrow (new TestDelegate (() => {
				DownloaderFactory.Instance.Initialize (DownloadMode.Packet, packetName);
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
		public void NoVersionNumber ()
		{
			ZipFile zip = new ZipFile ();
			byte[] bytes = RandomData.Build (2048, 512);
			zip.AddEntry ("OneFileInBadZip.data", bytes);
			zip.Save (bassPacketPath);
			zip.Dispose ();

			Assert.Throws<PacketException> (new TestDelegate (() => {
				DownloaderFactory.Instance.Initialize (DownloadMode.Packet, packetName);
			}));

		}

		[Test]
		public void WrongVersionNumber ()
		{
			ZipFile zip = new ZipFile ();
			byte[] bytes = RandomData.Build (2048, 512);
			zip.AddEntry ("OneFileInBadZip.data", bytes);
			zip.Comment = "NotAInt";
			zip.Save (bassPacketPath);
			zip.Dispose ();

			Assert.Throws<PacketException> (new TestDelegate (() => {
				DownloaderFactory.Instance.Initialize (DownloadMode.Packet, packetName);
			}));

		}

		private void MakeEntryBad (byte[] bytes)
		{
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
	public class PacketDownloaderNoDuplicate
	{
		const string packetName = "test_packet";
		const string filePrefix = "file_{0}.dat";

		IDownloader downloader;

		string[] packetPath;
		Dictionary<string,byte[]>[] dictPacket;
		int[] versions;

		[TestFixtureSetUp]
		public void Init ()
		{
			int packets = RandomData.Random.Next (3, 6);
			packetPath = new string[packets];
			dictPacket = new Dictionary<string, byte[]>[packets];
			versions = new int[packets];

			int fileOffset = 0;
			for (int i = 0; i < packets; i++) {
				dictPacket [i] = new Dictionary<string, byte[]> ();

				string where = (i == 0) ? Application.streamingAssetsPath : Application.persistentDataPath;
				string postFix = (i == 0) ? "_bass.pak" : string.Format ("_update_{0}.pak", i);

				versions [i] = RandomData.Random.Next ();
				packetPath [i] = System.IO.Path.Combine (where, packetName + postFix).Replace ("\\", "/");
				PreparePacket (dictPacket [i], packetPath [i], versions [i], fileOffset);

				fileOffset += dictPacket [i].Count;
			}

			// 初始化时，数据包必须已经存在。
			DownloaderFactory.Instance.Initialize (DownloadMode.Packet, packetName);
			downloader = DownloaderFactory.Instance.Downloader;
		}

		void PreparePacket (Dictionary<string,byte[]> dict, string packetPath, int version, int startIndex)
		{
			ZipFile zip = new ZipFile ();

			/// 随机文件数
			int files = RandomData.Random.Next (2, 5);
			for (int i = 0; i < files; i++) {
				/// 每个随机文件内容
				string fileName = string.Format (filePrefix, i + 1 + startIndex);
				byte[] bytes = RandomData.Build (2048, 512);
				dict.Add (fileName, bytes);

				zip.AddEntry (fileName, bytes);
			}
			zip.Comment = version.ToString ();
			zip.Save (packetPath);
			zip.Dispose ();
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			DownloaderFactory.Dispose ();
			downloader = null;
			try {
				for (int i = 0; i < packetPath.Length; i++) {
					System.IO.File.Delete (packetPath [i]);
					dictPacket [i].Clear ();
				}
				packetPath = null;
				dictPacket = null;
			} catch (Exception e) {
				Debug.Log (e.Message);
			}
		}


		[Test]		
		public void HomeVar ()
		{
			Assert.IsInstanceOf<PacketDownloader> (downloader);
			Assert.AreEqual ((downloader as PacketDownloader).Home, packetName);
		}

		[Test]
		public void PacketMustHasVersionNumber ()
		{
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
		public void DataValidEveryPacket ()
		{
			for (int i = 0; i < packetPath.Length; i++) {
				DataValid (dictPacket [i]);
			}
		}

		[Test]
		public void AllPacketsExist ()
		{
			PacketDownloader fd = downloader as PacketDownloader;
			Assert.IsNotNull (fd);
			PacketInfo[] ps = fd.Packets;
			Assert.AreEqual (ps.Length, packetPath.Length);
			for (int i = 0; i < packetPath.Length; i++) {
				int index = Array.FindIndex<PacketInfo> (ps, new Predicate<PacketInfo> ((p) => {
					return p.File.Name == packetPath [i];
				}));
				Assert.GreaterOrEqual (index, 0);
			}
		}

		[Test]
		public void AllVersionValid ()
		{
			PacketDownloader fd = downloader as PacketDownloader;
			Assert.IsNotNull (fd);
			PacketInfo[] ps = fd.Packets;
			Assert.AreEqual (ps.Length, packetPath.Length);
			for (int i = 0; i < packetPath.Length; i++) {
				int index = Array.FindIndex<PacketInfo> (ps, new Predicate<PacketInfo> ((p) => {
					return p.File.Name == packetPath [i];
				}));
				Assert.GreaterOrEqual (index, 0);
				Assert.AreEqual (versions [i], ps [index].Version);
			}
		}

		[Test]
		public void VersionSorted ()
		{
			PacketDownloader fd = downloader as PacketDownloader;
			Assert.IsNotNull (fd);			
			PacketInfo[] ps = fd.Packets;
			for (int i = 0; i < ps.Length - 2; i++) {
				Assert.Greater (ps [i].Version, ps [i + 1].Version);
			}
		}

		void DataValid (Dictionary<string,byte[]> dict)
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


	[TestFixture]
	public class PacketDownloaderDuplicated
	{
		const string packetName = "test_packet";
		const string filePrefix = "file_{0}.dat";

		IDownloader downloader;

		string[] packetPath;
		Dictionary<string,byte[]>[] dictPacket;
		int[] versions;
		Dictionary<string,byte[]> dictNewestPacket = new Dictionary<string, byte[]> ();

		[TestFixtureSetUp]
		public void Init ()
		{
			int packets = RandomData.Random.Next (3, 6);
			packetPath = new string[packets];
			dictPacket = new Dictionary<string, byte[]>[packets];
			versions = new int[packets];

			int fileOffset = 0;
			int version = 0;
			for (int i = 0; i < packets; i++) {
				dictPacket [i] = new Dictionary<string, byte[]> ();

				string where = (i == 0) ? Application.streamingAssetsPath : Application.persistentDataPath;
				string postFix = (i == 0) ? "_bass.pak" : string.Format ("_update_{0}.pak", i);

				versions [i] = version;
				packetPath [i] = System.IO.Path.Combine (where, packetName + postFix).Replace ("\\", "/");
				PreparePacket (dictPacket [i], packetPath [i], versions [i], fileOffset);

				int adder = RandomData.Random.Next (1, dictPacket [i].Count - 1);
				fileOffset += adder;
				version++;
			}

			// 初始化时，数据包必须已经存在。
			DownloaderFactory.Instance.Initialize (DownloadMode.Packet, packetName);
			downloader = DownloaderFactory.Instance.Downloader;

			// 收集最新版本的文件字典。版本号是按照从小到大排列好的。
			for (int i = dictPacket.Length - 1; i >= 0; i--) {
				Dictionary<string,byte[]>.Enumerator e = dictPacket [i].GetEnumerator ();
				while (e.MoveNext ()) {
					string f = e.Current.Key;
					byte[] d = e.Current.Value;
					if (!dictNewestPacket.ContainsKey (f)) {
						dictNewestPacket.Add (f, d);
					}
				}
			}
		}

		void PreparePacket (Dictionary<string,byte[]> dict, string packetPath, int version, int startIndex)
		{
			ZipFile zip = new ZipFile ();

			/// 随机文件数
			int files = RandomData.Random.Next (2, 5);
			for (int i = 0; i < files; i++) {
				/// 每个随机文件内容
				string fileName = string.Format (filePrefix, i + 1 + startIndex);
				byte[] bytes = RandomData.Build (2048, 512);
				dict.Add (fileName, bytes);

				zip.AddEntry (fileName, bytes);
			}
			zip.Comment = version.ToString ();
			zip.Save (packetPath);
			zip.Dispose ();
		}

		[TestFixtureTearDown]
		public void Clear ()
		{
			DownloaderFactory.Dispose ();
			downloader = null;
			try {
				for (int i = 0; i < packetPath.Length; i++) {
					System.IO.File.Delete (packetPath [i]);
					dictPacket [i].Clear ();
				}
				packetPath = null;
				dictPacket = null;
			} catch (Exception e) {
				Debug.Log (e.Message);
			}
		}

		[Test]
		public void DataValid ()
		{
			PacketDownloader fd = downloader as PacketDownloader;
			Assert.IsNotNull (fd);

			Dictionary<string,byte[]>.Enumerator e = dictNewestPacket.GetEnumerator ();
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