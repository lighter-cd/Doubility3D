using UnityEngine;
using UnityEditor;
using System.Collections;

public class ModelProcessor : AssetPostprocessor
{
    // 单纹理的角色如此处理。
    Material OnAssignMaterialModel (Material material,Renderer renderer)
    {
        UnityEngine.Debug.Log("OnAssignMaterialModel:" + assetPath);

        var materialName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        var materialPath = System.IO.Path.GetDirectoryName(assetPath) + "/Materials/" + materialName + ".mat";

        // Find if there is a material at the material path
        // Turn this off to always regeneration materials
        Material existed = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (existed != null)
            return existed;

        // Create a new material asset using the specular shader
        // but otherwise the default values from the model
        Texture2D normalTexture = null;
        if (material.mainTexture != null)
        {
            string mainTexturePath = AssetDatabase.GetAssetPath(material.mainTexture.GetInstanceID());
            string normalTexturePath = System.IO.Path.GetDirectoryName(mainTexturePath) + "/" 
                + System.IO.Path.GetFileNameWithoutExtension(mainTexturePath) + "_NRM"
                + System.IO.Path.GetExtension(mainTexturePath);
            normalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(normalTexturePath);
        }

        material.shader = Shader.Find("Charactor/Bumped Specular");
        material.name = materialName;
        if (normalTexture != null)
        {
            material.SetTexture("_BumpMap", normalTexture);
        }
        AssetDatabase.CreateAsset(material, materialPath);
        return material;
    }

    void OnPreprocessTexture()
    {
        if (assetPath.Contains("_NRM"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Bump;
            textureImporter.convertToNormalmap = true;
        }
    }

    void OnPreprocessModel()
    {
        UnityEngine.Debug.Log("OnPreprocessModel:" + assetPath);
    }

    void OnPostprocessModel(GameObject go)
    {
        UnityEngine.Debug.Log("OnPostprocessModel:" + assetPath);
    }
}
