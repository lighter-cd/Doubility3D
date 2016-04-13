using UnityEngine;
using UnityEditor;
using System.Collections;

/*
 * 自动处理法线纹理
 * 自动处理lightmap
 * 更换材质shader
 * 如果有法线贴图指定给材质
 * 模型动画输入模式为 Legacy
 */

namespace Doubility3D
{

	public class ModelProcessor : AssetPostprocessor
	{
		// 单纹理的角色如此处理。
		Material OnAssignMaterialModel (Material material, Renderer renderer)
		{
			var materialName = System.IO.Path.GetFileNameWithoutExtension (assetPath);

			int lastAt = materialName.LastIndexOf ('@');
			if (lastAt >= 0) {
				materialName = materialName.Substring (0, lastAt);
			}

			var materialPath = System.IO.Path.GetDirectoryName (assetPath) + "/Materials/" + materialName + ".mat";

			// Find if there is a material at the material path
			// Turn this off to always regeneration materials
			Material existed = AssetDatabase.LoadAssetAtPath<Material> (materialPath);
			if (existed != null)
				return existed;

			// Create a new material asset using the specular shader
			// but otherwise the default values from the model
			Texture2D normalTexture = null;
			if (material.mainTexture != null) {
				string mainTexturePath = AssetDatabase.GetAssetPath (material.mainTexture.GetInstanceID ());
				string normalTexturePath = System.IO.Path.GetDirectoryName (mainTexturePath) + "/"
				                                   + System.IO.Path.GetFileNameWithoutExtension (mainTexturePath) + "_NRM"
				                                   + System.IO.Path.GetExtension (mainTexturePath);
				normalTexture = AssetDatabase.LoadAssetAtPath<Texture2D> (normalTexturePath);
			}

			material.shader = Shader.Find ("Charactor/Bumped Specular");
			material.name = materialName;
			if (normalTexture != null) {
				material.SetTexture ("_BumpMap", normalTexture);
			}
			AssetDatabase.CreateAsset (material, materialPath);
			return material;
		}

		void OnPreprocessTexture ()
		{
			TextureImporter textureImporter = (TextureImporter)assetImporter;
			if (assetPath.Contains ("_NRM")) {
				textureImporter.textureType = TextureImporterType.Bump;
				textureImporter.convertToNormalmap = true;
			} else if (System.IO.Path.GetExtension (assetPath) == ".exr") {
				textureImporter.textureType = TextureImporterType.Lightmap;
			}
		}

		void OnPreprocessModel ()
		{
			ModelImporter modelImporter = (ModelImporter)assetImporter;
			modelImporter.animationType = ModelImporterAnimationType.Legacy;
		}

		void OnPostprocessModel (GameObject go)
		{
		
		}
	}
}