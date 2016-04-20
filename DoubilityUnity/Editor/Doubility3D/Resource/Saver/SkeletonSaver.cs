using System;
using System.Collections.Generic;
using UnityEngine;

using FlatBuffers;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

namespace Doubility3D.Resource.Saver
{
    static public class SkeletonSaver
    {
        const int InitBufferSize = 8192;

        static public ByteBuffer Save(GameObject go)
        {
            // 第一层，寻找自己不带Renderer的节点
            List<UnityEngine.Transform> lstTfs = new List<UnityEngine.Transform>();
            CollectTransforms(lstTfs, go.transform);

            FlatBufferBuilder builder = new FlatBufferBuilder(InitBufferSize);

            Offset<Schema.Joint>[] joints = new Offset<Schema.Joint>[lstTfs.Count];
            for (int i = 0; i < lstTfs.Count; i++)
            {
                int parent = lstTfs.FindIndex(new Predicate<UnityEngine.Transform>(
                                             (target) =>
                                             {
                                                 return target.Equals(lstTfs[i].parent);
                                             }
                                         ));
                var name = builder.CreateString(lstTfs[i].name);

                Schema.Joint.StartJoint(builder);
                Schema.Joint.AddNames(builder, name);
                Schema.Joint.AddTransform(builder, Schema.Transform.CreateTransform(builder,
                    lstTfs[i].localPosition.x, lstTfs[i].localPosition.y, lstTfs[i].localPosition.z,
                    lstTfs[i].localRotation.x, lstTfs[i].localRotation.y, lstTfs[i].localRotation.z, lstTfs[i].localRotation.w,
                    lstTfs[i].localScale.x, lstTfs[i].localScale.y, lstTfs[i].localScale.z
                ));
                Schema.Joint.AddParent(builder, parent);
                joints[i] = Schema.Joint.EndJoint(builder);
            }
            VectorOffset jointVector = Skeletons.CreateJointsVector(builder, joints);

            Skeletons.StartSkeletons(builder);
            Skeletons.AddJoints(builder, jointVector);
            Offset<Schema.Skeletons> skeleton = Skeletons.EndSkeletons(builder);
            
            builder.Finish(skeleton.Value);
            return builder.DataBuffer;
        }

        static void CollectTransforms(List<UnityEngine.Transform> lstTfs, UnityEngine.Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                UnityEngine.Transform tf = parent.transform.GetChild(i);
                if (tf.gameObject.GetComponent<Renderer>() == null)
                {
                    lstTfs.Add(tf);
                    CollectTransforms(lstTfs, tf);
                }
            }
        }
    }
}
