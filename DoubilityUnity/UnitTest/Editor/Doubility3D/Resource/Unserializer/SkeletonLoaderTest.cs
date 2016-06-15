using UnityEngine;
using NUnit.Framework;

using FlatBuffers;
using Doubility3D.Resource.Unserializing;
using Doubility3D.Resource.Schema;
using Doubility3D.Resource.ResourceObj;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using System.Collections.Generic;

using UnitTest.Doubility3D;

namespace UnitTest.Doubility3D.Resource.Unserializing
{
	[TestFixture]
	public class SkeletonLoaderTest
	{
		GameObject go;
		Schema.Skeletons skeletons;
		ResourceObjectSingle skeletonObj;

		[SetUp]
		public void Init ()
		{
			Context context = Context.Unknown;
			ByteBuffer bb = TestData.LoadResource ("skeleton.doub", out context);
			Assert.IsNotNull (bb);
			Assert.AreNotEqual (context, Context.Unknown);
			skeletons = Schema.Skeletons.GetRootAsSkeletons (bb);

			SkeletonUnserializer unserializer = UnserializerFactory.Instance.Create (context) as SkeletonUnserializer;

			skeletonObj = unserializer.Parse(bb) as ResourceObjectSingle;
			go = new GameObject ("Skeleton");
			(skeletonObj.Unity3dObject as GameObject).transform.parent = go.transform;
		}

		[TearDown]
		public void Cleanup ()
		{
			skeletonObj.Dispose ();
			UnityEngine.Object.DestroyImmediate (go);
			go = null;
			skeletons = null;
		}

		[Test]
		public void JointEqualSource ()
		{
			List<UnityEngine.Transform> lstTfs = new List<UnityEngine.Transform> ();
			CollectTransforms.Do (lstTfs, go.transform);
			Dictionary<string, UnityEngine.Transform> dictTfs = new Dictionary<string, UnityEngine.Transform> ();
			for (int i = 0; i < lstTfs.Count; i++) {
				dictTfs.Add (lstTfs [i].name, lstTfs [i]);
			}

			for (int i = 0; i < skeletons.JointsLength; i++) {
				Schema.Joint j = skeletons.GetJoints (i);

				Assert.IsTrue (dictTfs.ContainsKey (j.Names));
				UnityEngine.Transform tf = dictTfs [j.Names];

				Renderer r = tf.gameObject.GetComponent<Renderer> ();
				Assert.IsTrue (r == null);

				string parentGoName = (tf.parent != go.transform) ? tf.parent.name : null;
				string parentJointName = (j.Parent >= 0) ? skeletons.GetJoints (j.Parent).Names : null;
				Assert.AreEqual (parentGoName, parentJointName);

				Assert.AreEqual (tf.localPosition.x, j.Transform.Pos.X);
				Assert.AreEqual (tf.localPosition.y, j.Transform.Pos.Y);
				Assert.AreEqual (tf.localPosition.z, j.Transform.Pos.Z);

				Assert.AreEqual (tf.localRotation.x, j.Transform.Rot.X);
				Assert.AreEqual (tf.localRotation.y, j.Transform.Rot.Y);
				Assert.AreEqual (tf.localRotation.z, j.Transform.Rot.Z);
				Assert.AreEqual (tf.localRotation.w, j.Transform.Rot.W);

				Assert.AreEqual (tf.localScale.x, j.Transform.Scl.X);
				Assert.AreEqual (tf.localScale.y, j.Transform.Scl.Y);
				Assert.AreEqual (tf.localScale.z, j.Transform.Scl.Z);
			}
		}

		[Test]
		public void JointNoMultiple ()
		{
			List<UnityEngine.Transform> lstTfs = new List<UnityEngine.Transform> ();
			CollectTransforms.Do (lstTfs, go.transform);
			Dictionary<string, int> dictCount = new Dictionary<string, int> ();
			for (int i = 0; i < lstTfs.Count; i++) {
				UnityEngine.Transform tf = lstTfs [i];
				if (dictCount.ContainsKey (tf.name)) {
					dictCount [tf.name]++;
				} else {
					dictCount.Add (tf.name, 1);
				}
			}
			Dictionary<string, int>.Enumerator e = dictCount.GetEnumerator ();
			while (e.MoveNext ()) {
				Assert.AreEqual (e.Current.Value, 1);
			}
		}

		[Test]
		public void AllSourceBeExported ()
		{
			List<UnityEngine.Transform> lstTfs = new List<UnityEngine.Transform> ();
			CollectTransforms.Do (lstTfs, go.transform);

			HashSet<string> hashJoints = new HashSet<string> ();
			for (int i = 0; i < skeletons.JointsLength; i++) {
				Schema.Joint j = skeletons.GetJoints (i);
				hashJoints.Add (j.Names);
			}

			for (int i = 0; i < lstTfs.Count; i++) {
				Assert.IsTrue (hashJoints.Contains (lstTfs [i].name));
			}
		}
	}
}
