using System;
using UnityEngine;
using System.Collections.Generic;

namespace Doubility3D.Util
{
    static public class CollectTransforms
    {
        static public void Do(List<UnityEngine.Transform> lstTfs, UnityEngine.Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                UnityEngine.Transform tf = parent.transform.GetChild(i);
                if (tf.gameObject.GetComponent<Renderer>() == null)
                {
                    lstTfs.Add(tf);
                    Do(lstTfs, tf);
                }
            }
        }
    }
}
