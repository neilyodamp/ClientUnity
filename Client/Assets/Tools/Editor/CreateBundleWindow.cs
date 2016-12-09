using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateBundleWindow : EditorWindow
{

    static void ExportAssetBundle()
    {
        //1.清理掉原来所有的AB名称
        ClearAssetBundlesName();

        //2.对各个模块进行打AB处理
        ExportAssetCommonFont();
        ExportAssetCommonUI();
        ExportAssetCommonImage();
        ExportAssetIcons();
        ExportCommonTexture();

        ExportAssetCommonEffect();
        ExportAssetCommonRole();
        ExportAssetCommonAudio();
        ExportAssetCommonOther();

        //编辑器使用的 BuildTarget
        AssetBundleManifest fest = BuildPipeline.BuildAssetBundles(ToolsConst.AssetBundlesOutPutPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

    }

    //通用的UI
    static void ExportAssetCommonUI()
    {
        List<string> tabPaths = ToolsConst.GetCommonUIFilePath(); //目标AB路径
        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();
        List<string> includeNames = new List<string>(); //拓展名
        includeNames.Add(".prefab");

        //把该路径下的所有.prefab 路径找出来放入 pathFiles
        foreach( var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, includeNames);
        }

        ActivePrefab(pathFiles, "ui");
    }

    //各个角色
    static void ExportAssetCommonRole()
    {

    }

    //通用的图片
    static void ExportAssetCommonImage()
    {

    }

    static void ExportAssetIcons()
    {

    }
    /// <summary>
    ///导出sprite AB
    /// </summary>
    /// <param name="tabPaths"></param>
    /// <param name="abName"></param>
    /// <param name="extList"></param>
    private static void ExprotAssetSprites(List<string> tabPaths, string abName, List<string> extList = null)
    {

    }
    /// <summary>
    /// 导出不打图集的sha
    /// </summary>
    static void ExportCommonTexture()
    {

    }

    static void ExportAssetCommonFont()
    {

    }

    static void ExportAssetCommonAudio()
    {

    }

    static void ExportAssetCommonEffect()
    {

    }

    static void ExportAssetCommonAnimation()
    {

    }

    static void ExportAssetCommonOther()
    {

    }

    static void SetAssetBundleName(List<string> pathFiles, string name)
    {

    }

    static void SetAssetBundlesName(List<string> pathFiles, string name)
    {

    }

    static void SetAssetBundlesNameByFileName(List<string> pahtFiles, string name)
    {

    }
    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    /// <param name="recPath"></param>
    /// <param name="paths"></param>
    /// <param name="pathFiles"></param>
    /// <param name="includeNames"></param>
    static void Recursive(string recPath, List<string> paths, List<string> pathFiles, List<string> includeNames)
    {

    }

    static void Recursive(string recPath, List<string> paths, List<string> pathFiles)
    {

    }

    static void ClearAssetBundlesName()
    {

    }

    private static void CleanScripts(GameObject go)
    {

    }

    private static void CleanScript<T>(GameObject go) where T : Component
    {

    }

    static void ActivePrefab(List<string> pathFiles, string name)
    {

    }

    private static void Activate(Transform t)
    {

    }
}
