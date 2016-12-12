using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/*
    打AB规则 
    ExportAssetCommmonFont:
        将 Assets/UI/fonts/.../xxxfolder/ 下的字体(x(.ttf .TTF)) 打成 fonts/xxxfolder.ab

    ExportAssetCommonUI:
        将 Assets/UI/prefab 文件夹下的预设体(x.prefab),拷贝到Assets/tempUIPrefab/,打成ui/x.ab
    
    ExportAssetCommonRole:
        将 Assets/Role/prefab 文件夹下的预设体(x.prefab),打成role/x.ab
    
    ExportAssetCommonImage:
        将 Assets/UI/pics/common/.../xxxfolder/ 下的图片(x(.png .jpg .bmp .tga)) 打成 common/xxxfolder.ab

    ExportAssetIcons:
        将 Assets/UI/icons/.../xxxfolder 下的图片(x(.png .jpg .bmp .tga))打成 icons/xxxfolder.ab
    
    ExportCommonTexture:
        将 Assets/UI/texture/.../xxxfolder 下的图片(x(.png .jpg .bmp .tga))打成 texture/xxxfolder/x.ab

    ExportAssetCommonAudio:
        将 Assets/Sounds 下的音频文件(x(.mp3 .wav .ogg .aiff)) 打成 audio/x.ab
    
    ExportAssetCommonEffect:
        将 Assets/Effect/effect_zhandou 下的音频文件(x.prefab) 打成 effect/x.ab
        将 Assets/Effect/effect_ui 下的音频文件(x.prefab) 打成 effect/x.ab

    ExportAssetCommonOther:
        将 Assets/Tools/prefab 下的动画(x(.prefab .png)) 打成 other/x.ab

    ExportAssetCommonAnimation:
        将 Assets/Resources/Animation 下的动画(x(.anim .controller)) 打成 anim/x.abs
    
   

 */


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
        List<string> tabPaths = ToolsConst.GetCommonUIRolePath();
        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();
        List<string> includeNames = new List<string>(); //拓展名
        includeNames.Add(".prefab");

        foreach (var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, includeNames);
        }

        SetAssetBundleName(pathFiles, "role");
    }

    //通用的图片
    static void ExportAssetCommonImage()
    {
        /*  
        List<string> tabPaths = ToolsConst.GetCommonImageFilePath();
        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();
        List<string> includeNames = new List<string>(); //拓展名

        includeNames.Add(".png");
        includeNames.Add(".jpg");
        includeNames.Add(".bmp");
        includeNames.Add(".tga");

        foreach (var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, includeNames);
        }

        SetAssetBundlesName(pathFiles, "icon");
        */
        ExportAssetSprites(ToolsConst.GetCommonImageFilePath(), "common");
    }


    static void ExportAssetIcons() 
    {
        ExportAssetSprites(ToolsConst.GetIconsFilePath(), "icons");
    }

    /// <summary>
    ///导出sprite AB
    /// </summary>
    /// <param name="tabPaths"></param>
    /// <param name="abName"></param>
    /// <param name="extList"></param>
    private static void ExportAssetSprites(List<string> tabPaths, string abName, List<string> extList = null)
    {
        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();

        if(extList == null)
        {
            List<string> includeNames = new List<string>();
            includeNames.Add(".png");
            includeNames.Add(".jpg");
            includeNames.Add(".bmp");
            includeNames.Add(".tga");

            extList = includeNames;
        }
        foreach(var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, extList);
        }

        SetAssetBundlesName(pathFiles,abName);
    }

    /// <summary>
    /// 不打图集的散图
    /// </summary>
    static void ExportCommonTexture()
    {
        List<string> tabPaths = ToolsConst.GetCommonTextureFilePath();

        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();
        List<string> includeNames = new List<string>();

        includeNames.Add(".png");
        includeNames.Add(".jpg");
        includeNames.Add(".bmp");
        includeNames.Add(".tga");

        foreach (var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, includeNames);
        }

        //给文件设置AssetBundleName
        SetAssetBundlesNameByFileName(pathFiles, "texture");
    }

    static void ExportAssetCommonFont()
    {
        List<string> tabPaths = ToolsConst.GetCommonFontFilePath();
        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();
        List<string> includeNames = new List<string>();

        includeNames.Add(".ttf");
        includeNames.Add(".TTF");
        foreach (var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, includeNames);
        }

        SetAssetBundlesName(pathFiles, "fonts");
    }

    static void ExportAssetCommonAudio()
    {
        List<string> tabPaths = ToolsConst.GetCommonAudioFilePath();
        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();
        List<string> includeNames = new List<string>();

        includeNames.Add(".mp3");
        includeNames.Add(".wav");
        includeNames.Add(".ogg");
        includeNames.Add(".aiff");
        foreach (var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, includeNames);
        }
        //给文件设置AssetBundleName
        SetAssetBundleName(pathFiles, "audio");
    }

    static void ExportAssetCommonEffect()
    {
        List<string> tabPaths = ToolsConst.GetCommonEffectFilePath();
        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();
        List<string> includeNames = new List<string>();

        includeNames.Add(".prefab");
        foreach (var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, includeNames);
        }
        //给文件设置AssetBundleName
        SetAssetBundleName(pathFiles, "effect");
    }

    static void ExportAssetCommonAnimation()
    {
        List<string> tabPaths = ToolsConst.GetCommonAnimationFilePath();
        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();
        List<string> includeNames = new List<string>();

        includeNames.Add(".anim");
        includeNames.Add(".controller");
        foreach (var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, includeNames);
        }
        //给文件设置AssetBundleName
        SetAssetBundleName(pathFiles, "anim");
    }

    static void ExportAssetCommonOther()
    {
        List<string> tabPaths = ToolsConst.GetCommonOtherFilePath();
        List<string> paths = new List<string>();
        List<string> pathFiles = new List<string>();
        List<string> includeNames = new List<string>();

        includeNames.Add(".prefab");
        includeNames.Add(".png");
        foreach (var v in tabPaths)
        {
            Recursive(v, paths, pathFiles, includeNames);
        }
        //给文件设置AssetBundleName
        SetAssetBundleName(pathFiles, "other");
    }
    /// <summary>
    /// 设置ab名
    /// </summary>
    /// <param name="pathFiles"></param>
    /// <param name="name">ab 归类</param>
    static void SetAssetBundleName(List<string> pathFiles, string name)
    {
        for(int i = 0,max = pathFiles.Count;i< max; i++)
        {
            string strBundleName = EditorTools.GetFileName(pathFiles[i]);
            string newfile = pathFiles[i].Replace(Application.dataPath, "Assets");

            AssetImporter assetImporter = AssetImporter.GetAtPath(newfile);
            assetImporter.assetBundleName = name + "/" + strBundleName + ".ab";

            EditorUtility.DisplayProgressBar(strBundleName, "", (float)i / (float)max);
        }
    }

    /// <summary>
    /// 设置ab名,取父文件夹名字
    /// </summary>
    /// <param name="pathFiles"></param>
    /// <param name="name"></param>
    static void SetAssetBundlesName(List<string> pathFiles, string name)
    {
        for (int i = 0, max = pathFiles.Count; i < max; i++)
        {
            string strBundleName = EditorTools.GetParentFileName(pathFiles[i], 2);
            string newfile = pathFiles[i].Replace(Application.dataPath,"Assets");
            AssetImporter assetImporter = AssetImporter.GetAtPath(newfile);
            assetImporter.assetBundleName = name + "/" + strBundleName + ".ab";
            EditorUtility.DisplayProgressBar(strBundleName, "", (float)i / (float)max);
        }
    }

    static void SetAssetBundlesNameByFileName(List<string> pathFiles, string name)
    {
        for (int i = 0, max = pathFiles.Count; i < max; i++)
        {
            string strBundleName = EditorTools.GetParentFileName(pathFiles[i], 2) + "/" + EditorTools.GetFileName(pathFiles[i]);
            string newfile = pathFiles[i].Replace(Application.dataPath, "Assets");
            //在代码中给资源设置AssetBundleName
            AssetImporter assetImporter = AssetImporter.GetAtPath(newfile);
            assetImporter.assetBundleName = name + "/" + strBundleName + ".ab";
            //assetImporter.assetBundleVariant = "ab";
            EditorUtility.DisplayProgressBar(strBundleName, "", (float)i / (float)max);
        }
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
        string[] names = Directory.GetFiles(recPath);
        string[] dirs = Directory.GetDirectories(recPath);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            pathFiles.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir, paths, pathFiles);
        }
    }

    static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;

        string[] oldAssetBundleNames = new string[length];
        for (int i = 0;i<length;i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }
        
        for(int j = 0;j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j],true);
        }   
    }

    private static void CleanScripts(GameObject go)
    {
        if (go == null)
            return;

        foreach(Transform child in go.transform)
        {
            CleanScripts(child.gameObject);
        }

        CleanScript<GroupName>(go);
    }

    private static void CleanScript<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        while (t != null)
        {
            UnityEngine.MonoBehaviour.DestroyImmediate(t);
            t = go.GetComponent<T>();
        }
    }
    /// <summary>
    /// active Prefabs，并将其标记ab名称为 name/filename
    /// </summary>
    /// <param name="pathFiles"></param>
    /// <param name="name"></param>
    static void ActivePrefab(List<string> pathFiles, string name)
    {
        //创建 tempUIPrefab文件夹
        if (!Directory.Exists(Application.dataPath + "/tempUIPrefab/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/tempUIPrefab/");
        }

        for(int i = 0,max = pathFiles.Count;i<max;i++)
        {
            string strBundleName = EditorTools.GetFileName(pathFiles[i]);
            string newfile = pathFiles[i].Replace(Application.dataPath,"Assets");

            //获取该GameObject
            GameObject rootGo = AssetDatabase.LoadAssetAtPath(newfile, typeof(GameObject)) as GameObject;
            //实例化一个新的
            GameObject root = Instantiate(rootGo) as GameObject;
            root.name = rootGo.name;
            Activate(root.transform);
            root.SetActive(true);

            //临时 Prefab路径
            string tmpPrefabName = ToolsConst.TMPPREFABPATH + strBundleName + ".prefab";
            PrefabUtility.CreatePrefab(tmpPrefabName,root);
            AssetDatabase.Refresh();

            AssetImporter assetImporter = AssetImporter.GetAtPath(tmpPrefabName);
            assetImporter.assetBundleName = name + "/" + strBundleName + ".ab";
            DestroyImmediate(root); //立即删除掉临时创建的root GameObject
            EditorUtility.DisplayProgressBar(strBundleName, "", (float)i / (float)max); //显示正在进行处理
        }
    }

    private static void Activate(Transform t)
    {
        t.gameObject.SetActive(true);
        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            Activate(child);
        }
    }
}
