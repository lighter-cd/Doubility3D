using UnityEngine;
using UnityEditor;
using System.Collections;

public class ModelProcessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        UnityEngine.Debug.Log("OnPreprocessModel:" + assetPath);
    }
    Material OnAssignMaterialModel (Material material,Renderer renderer)
    {
        UnityEngine.Debug.Log("OnAssignMaterialModel:" + assetPath);
        return material;
    }
    void OnPostprocessModel(GameObject go)
    {
        UnityEngine.Debug.Log("OnPostprocessModel:" + assetPath);
    }
}
