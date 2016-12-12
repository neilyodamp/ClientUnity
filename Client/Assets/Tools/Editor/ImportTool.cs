using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ImportTool : AssetPostprocessor {
    
    void OnPostProcessTexture(Texture2D texture)
    {
        string folder = Path.GetDirectoryName(assetPath);
        folder = folder.Replace('\\','/');
        if(folder.Contains("Assets/UI/pics")||folder.Contains("Assets/UI/icons"))
        {
            ProcessUISprite(texture);
        }
        else if(folder.Contains("Assets/UI/texture"))
        {
            ProcessUITexture(texture);
        }
    }

    /// <summary>
    /// 设置UI图集格式
    /// </summary>
    /// <param name="texture"></param>
    private void ProcessUISprite(Texture2D texture)
    {

        TextureImporter texImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        texImporter.textureType = TextureImporterType.Sprite;
        texImporter.spriteImportMode = SpriteImportMode.Single;

        string packingTag = new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
        texImporter.spritePackingTag = packingTag;

        texImporter.mipmapEnabled = false;
        texImporter.filterMode = FilterMode.Bilinear;
        texImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;

        texImporter.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.RGBA32, 100, true);
        texImporter.SetPlatformTextureSettings("Standalone", 1024, TextureImporterFormat.RGBA32, 100, true);
        texImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.RGBA32, 100, true);
        texImporter.assetBundleName = null;

    }
    /// <summary>
    /// 设置不打图集的大图
    /// </summary>
    /// <param name="texture"></param>
    private void ProcessUITexture(Texture2D texture)
    {
        TextureImporter texImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        texImporter.textureType = TextureImporterType.Sprite;
        texImporter.spriteImportMode = SpriteImportMode.Single;

        texImporter.spritePackingTag = "";

        texImporter.mipmapEnabled = false;
        texImporter.filterMode = FilterMode.Bilinear;
        texImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;

        texImporter.SetPlatformTextureSettings("Android", 1024, TextureImporterFormat.RGBA32, 100, true);
        texImporter.SetPlatformTextureSettings("Standalone", 1024, TextureImporterFormat.RGBA32, 100, true);
        texImporter.SetPlatformTextureSettings("iPhone", 1024, TextureImporterFormat.RGBA32, 100, true);
    }

    /// <summary>
    /// 导入音频设置
    /// </summary>
    void OnPreprocessAudio()
    {

    }
}
