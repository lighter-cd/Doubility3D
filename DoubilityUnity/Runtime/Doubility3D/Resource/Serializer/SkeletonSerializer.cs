using System;
using System.Collections.Generic;

using UnityEngine;

using FlatBuffers;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

//using Doubility3D.Util;

namespace Doubility3D.Resource.Serializer
{
    public static class SkeletonSerializer
    {
        static public GameObject Load(ByteBuffer bb)
        {
            Schema.Skeletons skeleton = Schema.Skeletons.GetRootAsSkeletons(bb);
            GameObject[] goes = new GameObject[skeleton.JointsLength];
            for (int i = 0; i < skeleton.JointsLength; i++)
            {
                Schema.Joint joint = skeleton.GetJoints(i);
                GameObject go = new GameObject(joint.Names);

                // 设置局部 Transform 一定要在设置父节点之后进行，否则会因为设置了父节点导致局部transform而改变
                if (joint.Parent >= 0)
                {
                    go.transform.parent = goes[joint.Parent].transform;
                }

                Vec3 pos = joint.Transform.Pos;
                go.transform.localPosition = new Vector3(pos.X, pos.Y, pos.Z);

                Quat quat = joint.Transform.Rot;
                go.transform.localRotation = new Quaternion(quat.X, quat.Y, quat.Z, quat.W);

                Vec3 scl = joint.Transform.Scl;
                go.transform.localScale = new Vector3(scl.X, scl.Y, scl.Z);

                goes[i] = go;
            }
            return goes[0];
        }
    }
}
