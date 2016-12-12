using UnityEngine;
using System.Collections.Generic;

public class ToolsConst{
    public const string UIPrefabPath = "Assets/UI/prefab";
    public const string AssetBundlesOutPutPath = "Assets/StreamingAssets";
    public const string TMPPREFABPATH = @"Assets/tempUIPrefab/";

    public static List<string> GetCommonUIFilePath()
    {
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath + "/UI/prefab");
        return uiPaths;
    }

    public static List<string> GetCommonUIRolePath()
    {
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath+"/Role/prefab");
        return uiPaths;
    }

    public static List<string> GetCommonImageFilePath()
    {
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath + "/UI/pics/common");
        return uiPaths;
    }

    public static List<string> GetCommonFontFilePath()
    {
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath + "/UI/fonts");
        return uiPaths;
    }

    public static List<string> GetCommonAudioFilePath()
    {
        //目前没有通用的声音文件，声音文件都是像UI一样统一管理
        List<string> paths = new List<string>();
        paths.Add(Application.dataPath + "/Sound");
        return paths;
    }

    //特效路径
    public static List<string> GetCommonEffectFilePath()
    {
        //将客户端的表和共享的表打包
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath + "/Effect/effect_zhandou");
        uiPaths.Add(Application.dataPath + "/Effect/effect_ui");
        return uiPaths;
    }

    public static List<string> GetCommonAnimationFilePath()
    {
        //将客户端的表和共享的表打包
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath + "/Resources/Animation");
        return uiPaths;
    }

    public static List<string> GetCommonOtherFilePath()
    {
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath + "/Tools/prefab");
        return uiPaths;
    }
    public static List<string> GetIconsFilePath()
    {
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath + "/UI/icons");
        return uiPaths;
    }
    public static List<string> GetCommonTextureFilePath()
    {
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath + "/UI/texture");
        return uiPaths;
    }
}
