using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Doubility3D.Resource.Unserializing;
using Doubility3D.Resource.Schema;

namespace UnitTest.Doubility3D.Resource.Unserializing
{
	[TestFixture]
	public class UnserializerFactoryTest
	{

		[Test]
		public void Do ()
		{
			IUnserializer unserializer;

			unserializer = UnserializerFactory.Instance.Create (Context.Skeletons);
			Assert.IsInstanceOf<SkeletonUnserializer> (unserializer);

			unserializer = UnserializerFactory.Instance.Create (Context.Mesh);
			Assert.IsInstanceOf<MeshUnserializer> (unserializer);

			unserializer = UnserializerFactory.Instance.Create (Context.Material);
			Assert.IsInstanceOf<MaterialUnserializer> (unserializer);

			unserializer = UnserializerFactory.Instance.Create (Context.AnimationClip);
			Assert.IsInstanceOf<AnimationClipUnserializer> (unserializer);

			unserializer = UnserializerFactory.Instance.Create (Context.Texture);
			Assert.IsInstanceOf<TextureUnserializer> (unserializer);

			unserializer = UnserializerFactory.Instance.Create ((Context)(-1));
			Assert.IsNull (unserializer);


		}
	}
}