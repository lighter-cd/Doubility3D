﻿using UnityEngine;
using NUnit.Framework;

using FlatBuffers;
using Doubility3D.Resource.Saver;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using System.Collections.Generic;

using UnitTest.Doubility3D;

namespace UnitTest.Doubility3D.Resource.Saver
{
	[TestFixture]
	public class SkeletonSaverTest
	{
        AssetBundle ab;
		GameObject go;
		Schema.Skeletons skeletons;

		[SetUp]
		public void Init ()
		{
            ab = TestData.LoadBundle("skeletontest.bundle");
            go = TestData.LoadFirstAsset<GameObject>(ab);
            Assert.IsNotNull(go);

			ByteBuffer result = SkeletonSaver.Save(go);
			skeletons = Schema.Skeletons.GetRootAsSkeletons(result);
		}

		[TearDown] 
		public void Cleanup ()
		{
			ab.Unload(true);
			go = null;
			ab = null;
			skeletons = null;
		}

		[Test]
		public void JointEqualSource()
		{
            List<UnityEngine.Transform> lstTfs = new List<UnityEngine.Transform>();
            CollectTransforms.Do(lstTfs, go.transform);
            Dictionary<string, UnityEngine.Transform> dictTfs = new Dictionary<string, UnityEngine.Transform>();
            for (int i = 0; i < lstTfs.Count; i++)
            {
                dictTfs.Add(lstTfs[i].name, lstTfs[i]);
            }

			for(int i=0;i<skeletons.JointsLength;i++){
				Schema.Joint j = skeletons.GetJoints(i);

                Assert.IsTrue(dictTfs.ContainsKey(j.Names));
                UnityEngine.Transform tf = dictTfs[j.Names];

                Renderer r = tf.gameObject.GetComponent<Renderer>();
				Assert.IsTrue(r == null);

				string parentGoName = (tf.parent!=go.transform)?tf.parent.name:null;
				string parentJointName = (j.Parent >= 0)?skeletons.GetJoints(j.Parent).Names:null;
				Assert.AreEqual(parentGoName,parentJointName);

				Assert.AreEqual(tf.localPosition.x,j.Transform.Pos.X);
				Assert.AreEqual(tf.localPosition.y,j.Transform.Pos.Y);
				Assert.AreEqual(tf.localPosition.z,j.Transform.Pos.Z);

				Assert.AreEqual(tf.localRotation.x,j.Transform.Rot.X);
				Assert.AreEqual(tf.localRotation.y,j.Transform.Rot.Y);
				Assert.AreEqual(tf.localRotation.z,j.Transform.Rot.Z);
				Assert.AreEqual(tf.localRotation.w,j.Transform.Rot.W);

				Assert.AreEqual(tf.localScale.x, j.Transform.Scl.X);
                Assert.AreEqual(tf.localScale.y, j.Transform.Scl.Y);
                Assert.AreEqual(tf.localScale.z, j.Transform.Scl.Z);
			}
		}
		[Test]
		public void JointNoMultiple()
		{
			Dictionary<string,int> dictCount = new Dictionary<string, int>();
			for(int i=0;i<skeletons.JointsLength;i++){
				Schema.Joint j = skeletons.GetJoints(i);
				if(dictCount.ContainsKey(j.Names)){
					dictCount[j.Names]++;
				}else{
					dictCount.Add(j.Names,1);
				}
			}
			Dictionary<string,int>.Enumerator e = dictCount.GetEnumerator();
			while(e.MoveNext()){
				Assert.AreEqual(e.Current.Value,1);
			}
		}
		[Test]
		public void AllSourceBeExported(){
			List<UnityEngine.Transform> lstTfs = new List<UnityEngine.Transform>();
			CollectTransforms.Do(lstTfs, go.transform);

			HashSet<string> hashJoints = new HashSet<string>();
			for(int i=0;i<skeletons.JointsLength;i++){
				Schema.Joint j = skeletons.GetJoints(i);
				hashJoints.Add(j.Names);
			}

			for(int i=0;i<lstTfs.Count;i++){
				Assert.IsTrue(hashJoints.Contains(lstTfs[i].name)); 
			}
		}
	}
}
