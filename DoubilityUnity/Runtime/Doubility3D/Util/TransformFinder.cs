using System;
using System.Collections.Generic;
using UnityEngine;

namespace Doubility3D.Util
{
    static public class TransformFinder
    {
        static public Transform Find(Transform transform, string boneName)
        {
            Transform child = transform.FindChild(boneName);
            if (child == null)
            {
                foreach (Transform c in transform)
                {
                    child = Find(c, boneName);
                    if (child != null) 
                        return child;
                }
            }
            return child;
        }
    }
}
