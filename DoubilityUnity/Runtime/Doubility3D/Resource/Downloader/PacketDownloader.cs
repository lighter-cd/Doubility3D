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
	public class ZipEntryComparerClass : IComparer  {

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
		ZipFile[] zipFiles;
		Dictionary<ZipFile,ZipEntry[]> dictEntries = new Dictionary<ZipFile, ZipEntry[]> ();

		internal PacketDownloader (string _home)
		{
			home = _home;

			// 在 streamingAssetsPath 和 dataPath 中寻找
			List<ZipFile> lstZipFiles = new List<ZipFile> ();
			SearchPackets (Application.streamingAssetsPath, lstZipFiles);
			SearchPackets (Application.persistentDataPath, lstZipFiles);
			lstZipFiles.Sort (
				(z1,z2)=>{
					int v1 = 0,v2 = 0;
					int.TryParse(z1.Comment,out v1);
					int.TryParse(z2.Comment,out v2);
					return v2 - v1;
				}
			);
			zipFiles = lstZipFiles.ToArray();
		}

		void SearchPackets(string where,List<ZipFile> lstZipFiles){
			string[] files = Directory.GetFiles(where,home + "*.pak",SearchOption.AllDirectories);
			var options = new ReadOptions { StatusMessageWriter = System.Console.Out };
			for (int i = 0; i < files.Length; i++) {
				ZipFile zip = ZipFile.Read (files [i], options);
				lstZipFiles.Add (zip);
				ZipEntry[] entries = new ZipEntry[zip.EntriesSorted.Count];
				zip.EntriesSorted.CopyTo (entries, 0);
				dictEntries.Add(zip,entries);
			}		
		}

		public IEnumerator ResourceTask (string path, Action<Byte[],string> actOnComplate)
		{
			// 在每个packets中找
			ZipEntry entry = null;
			for (int i = 0; i < zipFiles.Length; i++) {
				ZipEntry[] entries = dictEntries[zipFiles[i]];
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
				entry.Extract (ms);
				actOnComplate (bytes, null);
			}
		}
		public string Home { get { return home;} }
	}
}

