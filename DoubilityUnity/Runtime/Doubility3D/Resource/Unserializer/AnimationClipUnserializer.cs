using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using FlatBuffers;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;
using Doubility3D.Resource.ResourceObj;

//using Doubility3D.Util;

namespace Doubility3D.Resource.Unserializing
{
	public class AnimationClipUnserializer : IUnserializer
    {
		public ResourceObject Parse(ByteBuffer bb)
		{
            UnityEngine.AnimationClip clip = new UnityEngine.AnimationClip();

            Schema.AnimationClip _clip = Schema.AnimationClip.GetRootAsAnimationClip(bb);
			clip.frameRate = _clip.FrameRate;
			clip.wrapMode = (UnityEngine.WrapMode)_clip.WrapMode;
			clip.legacy = true;
				
			for(int i=0; i<_clip.BindingsLength;i++){
                Schema.CurveBinding bind = _clip.GetBindings(i);
                Schema.AnimationCurve _curve = bind.Curve;
                UnityEngine.AnimationCurve curve = new UnityEngine.AnimationCurve();

                for (int j = 0; j < _curve.KeyFramesLength; j++)
                {
                    Schema.KeyFrame _kf = _curve.GetKeyFrames(j);
                    UnityEngine.Keyframe kf = new UnityEngine.Keyframe(_kf.Time,_kf.Value,_kf.InTangent,_kf.OutTangent);
                    kf.tangentMode = _kf.TangentMode;
                    curve.AddKey(kf);
                }
                curve.preWrapMode = (UnityEngine.WrapMode)_curve.PreWrapMode;
                curve.postWrapMode = (UnityEngine.WrapMode)_curve.PostWrapMode;
                
                var assembly = Assembly.Load("UnityEngine");
                Type type = assembly.GetType(bind.Type);
                clip.SetCurve(bind.Path, type, bind.PropertyName, curve);
            }
			return new ResourceObjectSingle(clip);
        }
    }
}
