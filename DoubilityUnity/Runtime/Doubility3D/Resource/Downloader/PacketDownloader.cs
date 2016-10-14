using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;

/// <summary>
/// 解压多线程
/// zipFile的内存方式
/// 由Updater维护一个最新的文件列表
/// </summary>

namespace Doubility3D.Resource.Downloader
{
	public class PacketInfo {
		ZipFile zipFile;
		int version;

		public PacketInfo(ZipFile zf,int ver){
			zipFile = zf;
			version = ver;
		}
		public ZipFile File { get { return zipFile; } }
		public int Version { get { return version; } }
	}

	public class PacketException : Exception {
		public PacketException(Exception e) : base(e.GetType().FullName + ":" + e.Message,e)
		{
		}
	}

	internal class ZipEntryComparerClass : IComparer  {

		// Calls CaseInsensitiveComparer.Compare with the parameters reversed.
		int IComparer.Compare( object x, object y )  {
			ZipEntry entry = x as ZipEntry;
			string path = y as string;
			return string.Compare(entry.FileName,path);
		}

	}

	// 包名模式(两个包)
	// 所在目录(StreamAssets/DataPath)
	// 
	public class PacketDownloader : IDownloader
	{
		string home;
		PacketInfo[] packets;
		Dictionary<ZipFile,ZipEntry[]> dictEntries = new Dictionary<ZipFile, ZipEntry[]> ();

		internal PacketDownloader (string _home)
		{
			home = _home;

			// 在 streamingAssetsPath 和 dataPath 中寻找
			List<PacketInfo> lstPackets = new List<PacketInfo> ();
			SearchPackets (Application.streamingAssetsPath, lstPackets);
			SearchPackets (Application.persistentDataPath, lstPackets);
			lstPackets.Sort (
				(p1,p2)=>{
					return p2.Version - p1.Version;
				}
			);
			packets = lstPackets.ToArray();
		}

		void SearchPackets(string where,List<PacketInfo> lstPackets){
			string[] files = Directory.GetFiles(where,home + "*.pak",SearchOption.AllDirectories);
			var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
			for (int i = 0; i < files.Length; i++) {
				ZipFile zip = null;
				int version = -1;
				try{
					zip = ZipFile.Read (files [i].Replace ("\\", "/"), options);
					version = int.Parse(zip.Comment);

				}catch(Exception e){
					if (zip != null) {
						zip.Dispose ();
					}
					dictEntries.Clear ();
					dictEntries = null;
					for(int x=0;x<lstPackets.Count;x++){
						lstPackets [x].File.Dispose ();
					}
					lstPackets.Clear ();
					throw(new PacketException (e));
				}
				lstPackets.Add (new PacketInfo(zip,version));
				ZipEntry[] entries = new ZipEntry[zip.EntriesSorted.Count];
				zip.EntriesSorted.CopyTo (entries, 0);
				dictEntries.Add(zip,entries);
			}		
		}

		public IEnumerator ResourceTask (string path, Action<Byte[],string> actOnComplate)
		{
			// 在每个packets中找
			ZipEntry entry = null;
			for (int i = 0; i < packets.Length; i++) {
				ZipEntry[] entries = dictEntries[packets[i].File];
				int where = Array.BinarySearch (entries, path, new ZipEntryComparerClass());
				if (where >= 0) {
					entry = entries [where];
					break;
				}
				yield return null;
			}
			if (entry == null) {
				actOnComplate (null, "path not founded :" + path);
			} else {
				yield return null;
				byte[] bytes = new byte[entry.UncompressedSize];
				MemoryStream ms = new MemoryStream (bytes);

				try{
					entry.Extract (ms);
				}catch(Exception e){
					throw(new PacketException (e));
				}

				if (actOnComplate != null) {
					actOnComplate (bytes, null);
				}
			}
		}
		public string Home { get { return home;} }
		public void Dispose(){
			dictEntries.Clear ();
			dictEntries = null;
			for (int i = 0; i < packets.Length; i++) {
				packets[i].File.Dispose();
				packets[i] = null;
			}
			packets = null;
		}

		public PacketInfo[] Packets { get { return packets; } }
	}
}

