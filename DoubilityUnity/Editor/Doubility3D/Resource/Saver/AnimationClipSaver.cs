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
                    Offset<Schema.KeyFrame> offset = Schema.KeyFrame.CreateKeyFrame(builder, kfs[j].inTangent, kfs[j].outTangent, kfs[j].tangentMode, kfs[j].time, kfs[j].value);
                    builder.AddOffset(offset.Value);
                }
                VectorOffset vecKeyFrames = builder.EndVector();

                Schema.AnimationCurve.StartAnimationCurve(builder);
                Schema.AnimationCurve.AddKeyFrames(builder,vecKeyFrames);
                Schema.AnimationCurve.AddPostWrapMode(builder, (Schema.WrapMode)curve.postWrapMode);
                Schema.AnimationCurve.AddPostWrapMode(builder, (Schema.WrapMode)curve.preWrapMode);
                Offset<Schema.AnimationCurve> offCurve = Schema.AnimationCurve.EndAnimationCurve(builder);

				StringOffset offPropertyName = builder.CreateString(bindings[i].propertyName);
				StringOffset offPath = builder.CreateString(bindings[i].path);
				StringOffset offType = builder.CreateString(bindings[i].type.FullName);

                Schema.CurveBinding.StartCurveBinding(builder);
				Schema.CurveBinding.AddPropertyName(builder,offPropertyName);
				Schema.CurveBinding.AddPath(builder, offPath);
				Schema.CurveBinding.AddType(builder, offType);
				Schema.CurveBinding.AddCurve(builder, offCurve);
                vecOffsetCurveBindings[i] = Schema.CurveBinding.EndCurveBinding(builder);
            }

			VectorOffset vecCurveBindings = Schema.AnimationClip.CreateBindingsVector(builder, vecOffsetCurveBindings);


			Schema.AnimationClip.StartAnimationClip(builder);
			Schema.AnimationClip.AddBindings(builder,vecCurveBindings);
            Offset<Schema.AnimationClip> offClip = Schema.AnimationClip.EndAnimationClip(builder);

            builder.Finish(offClip.Value);
            FileSaver.Save(builder.DataBuffer, Context.AnimationClip, dstFolder + "/" + clip.name + ".db3d");
        }
    }
}
