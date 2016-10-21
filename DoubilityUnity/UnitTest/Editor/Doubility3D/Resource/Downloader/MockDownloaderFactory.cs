using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using Doubility3D.Resource.Downloader;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	public static class MockDownloaderFactory
	{
		static public void Initialize (IDownloader downloader)
		{
			Assembly assembly = Assembly.GetAssembly(typeof(DownloaderFactory));
			Type type = assembly.GetType("Doubility3D.Resource.Downloader.DownloaderFactory");

			DownloaderFactory  df = DownloaderFactory.Instance;
			BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Instance;
			FieldInfo f_key = type.GetField("downloader", flag);
			f_key.SetValue(df, downloader);
		}
	}
}

