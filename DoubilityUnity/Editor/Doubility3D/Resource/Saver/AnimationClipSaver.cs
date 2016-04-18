using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using FlatBuffers;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

namespace Doubility3D.Resource.Saver
{
    public static class AnimationClipSaver
    {
        const int InitBufferSize = 8192;

        public static void Save(UnityEngine.AnimationClip clip, string dstFolder)
        {
           EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip);

            FlatBufferBuilder builder = new FlatBufferBuilder(InitBufferSize);
            Offset<Schema.CurveBinding>[] vecOffsetCurveBindings = new Offset<CurveBinding>[bindings.Length];

            for (int i = 0; i < bindings.Length; i++)
            {
                UnityEngine.AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, bindings[i]);
                Keyframe[] kfs = curve.keys;

                Schema.AnimationCurve.StartKeyFramesVector(builder, kfs.Length);
                for (int j = 0; j < kfs.Length; j++)
                {
                    Offset<Schema.KeyFrame> offset = Schema.KeyFrame.CreateKeyFrame(builder, kfs[i].inTangent, kfs[i].outTangent, kfs[i].tangentMode, kfs[i].time, kfs[i].value);
                    builder.AddOffset(offset.Value);
                }
                VectorOffset vecKeyFrames = builder.EndVector();

                Schema.AnimationCurve.StartAnimationCurve(builder);
                Schema.AnimationCurve.AddKeyFrames(builder,vecKeyFrames);
                Schema.AnimationCurve.AddPostWrapMode(builder, (Schema.WrapMode)curve.postWrapMode);
                Schema.AnimationCurve.AddPostWrapMode(builder, (Schema.WrapMode)curve.preWrapMode);
                Offset<Schema.AnimationCurve> vecCurve = Schema.AnimationCurve.EndAnimationCurve(builder);

                Schema.CurveBinding.StartCurveBinding(builder);
                Schema.CurveBinding.AddPropertyName(builder,builder.CreateString(bindings[i].propertyName));
                Schema.CurveBinding.AddPath(builder, builder.CreateString(bindings[i].propertyName));
                Schema.CurveBinding.AddType(builder, builder.CreateString(bindings[i].type.FullName));
                Schema.CurveBinding.AddCurve(builder, vecCurve);
                vecOffsetCurveBindings[i] = Schema.CurveBinding.EndCurveBinding(builder);
            }

            Schema.AnimationClip.StartAnimationClip(builder);
            Schema.AnimationClip.CreateBindingsVector(builder, vecOffsetCurveBindings);
            Offset<Schema.AnimationClip> offClip = Schema.AnimationClip.EndAnimationClip(builder);

            builder.Finish(offClip.Value);
            FileSaver.Save(builder.DataBuffer, Context.AnimationClip, dstFolder + "/" + clip.name + ".db3d");
        }
    }
}
