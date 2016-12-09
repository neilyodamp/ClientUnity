using UnityEngine;
using System.Collections.Generic;

public class ToolsConst{
    public const string UIPrefabPath = "Assets/UI/prefab";
    public const string AssetBundlesOutPutPath = "Assets/StreamingAssets";

    public static List<string> GetCommonUIFilePath()
    {
        List<string> uiPaths = new List<string>();
        uiPaths.Add(Application.dataPath + "/UI/prefab");
        return uiPaths;
    }


}
