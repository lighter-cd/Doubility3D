using UnityEngine;
using UnityEditor;
using NUnit.Framework;

using FlatBuffers;
using Doubility3D.Resource.Saver;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Util;

using System.Collections.Generic;
using System;

using UnitTest.Doubility3D;

namespace UnitTest.Doubility3D.Resource.Saver
{
	[TestFixture]
	public class ClipSaverTest
    {
        AssetBundle ab;
        UnityEngine.AnimationClip originClip;
        Schema.AnimationClip clip;

        [SetUp]
        public void Init()
        {
            ab = TestData.LoadBundle("animationcliptest.bundle");
            GameObject go = TestData.LoadFirstAsset<GameObject>(ab);
            Assert.IsNotNull(go);
            
            Animation animation = go.GetComponentInChildren<Animation>();
            originClip = animation.clip;
            Assert.IsNotNull(originClip);

            ByteBuffer result = AnimationClipSaver.Save(originClip);
            clip = Schema.AnimationClip.GetRootAsAnimationClip(result);
        }

        [TearDown]
        public void Cleanup()
        {
            ab.Unload(true);
            originClip = null;
            ab = null;
            clip = null;
        }
        
        [Test]
        public void EqualSource()
        {
			Assert.AreEqual(originClip.frameRate,clip.FrameRate);
			Assert.AreEqual(originClip.wrapMode,(UnityEngine.WrapMode)clip.WrapMode);

			EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(originClip);
            Assert.AreEqual(clip.BindingsLength, bindings.Length);
            for (int i = 0; i < bindings.Length; i++)
            {
                Schema.CurveBinding bind = clip.GetBindings(i);
                Assert.AreEqual(bind.PropertyName, bindings[i].propertyName);
                Assert.AreEqual(bind.Path, bindings[i].path);
                Assert.AreEqual(bind.Type, bindings[i].type.FullName);

                Schema.AnimationCurve curv = bind.GetCurve(new Schema.AnimationCurve());
                UnityEngine.AnimationCurve originCurve = AnimationUtility.GetEditorCurve(originClip, bindings[i]);

                Assert.AreEqual(curv.PreWrapMode, (Schema.WrapMode)originCurve.preWrapMode);
                Assert.AreEqual(curv.PostWrapMode, (Schema.WrapMode)originCurve.postWrapMode);
                Assert.AreEqual(curv.KeyFramesLength, originCurve.keys.Length);

                for (int j = 0; j < curv.KeyFramesLength; j++)
                {
                    Schema.KeyFrame keyFrame = curv.GetKeyFrames(j);
                    UnityEngine.Keyframe originKeyframe = originCurve.keys[j];

                    Assert.AreEqual(keyFrame.InTangent, originKeyframe.inTangent);
                    Assert.AreEqual(keyFrame.OutTangent, originKeyframe.outTangent);
                    Assert.AreEqual(keyFrame.TangentMode, originKeyframe.tangentMode);
                    Assert.AreEqual(keyFrame.Time, originKeyframe.time);
                    Assert.AreEqual(keyFrame.Value, originKeyframe.value);
                }
            }
        }
    }
}