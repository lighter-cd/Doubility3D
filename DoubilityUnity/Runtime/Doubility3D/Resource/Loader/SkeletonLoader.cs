using System;
using System.Collections.Generic;

using UnityEngine;

using FlatBuffers;
using Doubility3D.Resource.Schema;
using Schema = Doubility3D.Resource.Schema;

//using Doubility3D.Util;

namespace Doubility3D.Resource.Loader
{
    public static class SkeletonLoader
    {
        static public GameObject Load(ByteBuffer bb)
        {
            Schema.Skeletons skeleton = Schema.Skeletons.GetRootAsSkeletons(bb);
            GameObject[] goes = new GameObject[skeleton.JointsLength];
            for (int i = 0; i < skeleton.JointsLength; i++)
            {
                Schema.Joint joint = skeleton.GetJoints(i);
                GameObject go = new GameObject(joint.Names);
                
                Vec3 pos = joint.Transform.Pos;
                go.transform.localPosition.Set(pos.X, pos.Y, pos.Z);

                Quat quat = joint.Transform.Rot;
                go.transform.localRotation.Set(quat.X, quat.Y, quat.Z, quat.W);

                Vec3 scl = joint.Transform.Scl;
                go.transform.localScale.Set(scl.X, scl.Y, scl.Z);

                if (joint.Parent >= 0)
                {
                    go.transform.parent = goes[joint.Parent].transform;
                }

                goes[i] = go;
            }
            return goes[0];
        }
    }
}
