using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

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
    /// 遍历目录及其子目录，并返回指定拓展名的文件名(包含路径)
    /// </summary>
    /// <param name="recPath">要遍历的目录</param>
    /// <param name="paths">用于递归查找的子目录</param>
    /// <param name="pathFiles">返回查找的所有文件名(包含路径)</param>
    /// <param name="includeNames">文件拓展名</param>
    static void Recursive(string recPath, List<string> paths, List<string> pathFiles, List<string> includeNames)
    {
        string [] names = Directory.GetFiles(recPath);
        string[] dirs = Directory.GetDirectories(recPath);
        foreach(string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (!includeNames.Contains(ext))
            {
                continue;
            }
            pathFiles.Add(filename.Replace('\\','/'));
        }

        foreach(string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir,paths,pathFiles,includeNames);
        }

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
    /// <summary>
    /// active Prefabs，并将其标记ab名称为 name/filename
    /// </summary>
    /// <param name="pathFiles"></param>
    /// <param name="name"></param>
    static void ActivePrefab(List<string> pathFiles, string name)
    {
        if (!Directory.Exists(Application.dataPath + "/tempUIPrefab/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/tempUIPrefab/");
        }

        for(int i = 0,max = pathFiles.Count;i<max;i++)
        {
            //string strBundleName = Edit
        }
    }

    private static void Activate(Transform t)
    {

    }
}
