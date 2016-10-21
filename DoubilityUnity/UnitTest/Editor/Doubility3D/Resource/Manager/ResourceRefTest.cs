using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace UnitTest.Doubility3D.Resource.Manager
{
	public class ResourceRefTest
	{
		// 数据：无依赖 有依赖(1层，2层，3层), 依赖无直接引用的，依赖也被直接引用的
		// 手写 json ?
		TestParam[] _params  = {
			new TestParam("NDObject01",0,null),
			new TestParam("NDObject02",3,null),
			new TestParam("NDObject03",2,null),
			new TestParam("NDObject04",4,new string[]{"NDObject01"}),
			new TestParam("NDObject05",8,new string[]{"NDObject01","NDObject02"}),
			new TestParam("NDObject06",7,new string[]{"NDObject01","NDObject02","NDObject03"}),
			new TestParam("NDObject07",9,new string[]{"NDObject04"}),
			new TestParam("NDObject08",6,new string[]{"NDObject07"}),
		};


		[Test]
		public void NoDependencs ()
		{
		}
		[Test]
		public void OneDependencs ()
		{
		}
		[Test]
		public void TwoDependencs ()
		{
		}
		[Test]
		public void ThreeDependencs ()
		{
		}
		[Test]
		public void DownloadError ()
		{
		}
		[Test]
		public void PaserError ()
		{
		}
		[Test]
		public void DependencsError ()
		{
		}
	}
}