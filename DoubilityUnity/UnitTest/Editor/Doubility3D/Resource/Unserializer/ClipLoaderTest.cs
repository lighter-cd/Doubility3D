﻿using UnityEngine;
using UnityEditor;
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
    public class ClipLoaderTest
    {
        ResourceObjectSingle result;
        Schema.AnimationClip clip;

        [SetUp]
        public void Init()
        {
            Context context = Context.Unknown;
            ByteBuffer bb = TestData.LoadResource("dm_knockdown.doub", out context);
            Assert.IsNotNull(bb);
            Assert.AreNotEqual(context, Context.Unknown);

			AnimationClipUnserializer unserializer = UnserializerFactory.Instance.Create (context) as AnimationClipUnserializer;
			result = unserializer.Parse(bb) as ResourceObjectSingle;
            clip = Schema.AnimationClip.GetRootAsAnimationClip(bb);
        }

        [TearDown]
        public void Cleanup()
        {
			result.Dispose ();
			result = null;
            clip = null;
        }

        [Test]
        public void EqualSource()
        {
			UnityEngine.AnimationClip originClip = result.Unity3dObject as UnityEngine.AnimationClip;

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
