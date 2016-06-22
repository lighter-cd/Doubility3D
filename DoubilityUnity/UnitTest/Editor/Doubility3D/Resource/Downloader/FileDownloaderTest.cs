using System;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace UnitTest.Doubility3D.Resource.Downloader
{
	[TestFixture]
	public class FileDownloaderTest
	{
		[Test]		
		public void FileReadError ()
		{

		}
		[Test]		
		public void FileDataValid ()
		{
			// 先写再读
			// 删除文件
		}
		[Test]		
		public void HomeVar ()
		{
			Environment.SetEnvironmentVariable ("DOUBILITY_HOME","",EnvironmentVariableTarget.Machine);
		}
	}
}