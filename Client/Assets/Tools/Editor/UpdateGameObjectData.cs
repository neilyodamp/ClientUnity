using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
public class UpdateGameObjectData : Editor {

	static void _UpdateGameObjectData()
    {
        Init();
        ExportAll(ToolsConst.UIPrefabPath);
    }

    static void UpdateThisPrefabGameObjectData()
    {

    }

    public static void ClearPrefabScript()
    {

    }

    private static readonly HashSet<string> ControllPrefixes = new HashSet<string>();

    private static void Init()
    {
        ControllPrefixes.Add("m");
        ControllPrefixes.Add("btn");
        ControllPrefixes.Add("ckb");
        ControllPrefixes.Add("pgsb");
        ControllPrefixes.Add("txt");
        ControllPrefixes.Add("inp");
        ControllPrefixes.Add("rdb");
        ControllPrefixes.Add("scp");
        ControllPrefixes.Add("grp");
        ControllPrefixes.Add("list");
    }

    private static void ExportAll(string folderPath)
    {
        DirectoryInfo rootFolder = new DirectoryInfo(folderPath);
        foreach(FileInfo file in rootFolder.GetFiles()) //对当前文件进行处理
        {
            string fileName = file.FullName;
            if(file.Name.EndsWith(".prefab"))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath(fileName.Substring(fileName.LastIndexOf("Assets")),
                    typeof(GameObject)) as GameObject;
                Export(go);
            }
        }

        foreach(DirectoryInfo folder in rootFolder.GetDirectories()) //继续递归文件夹
        {
            ExportAll(folder.FullName);
        }
    }

    private static bool Export(GameObject rootGo,bool forLua = false)
    {
        rootGo.SetActive(true);
        GameObjectData data;
        DestroyImmediate(rootGo.GetComponent<GameObjectData>(), true);//删掉该组件
        if(rootGo.GetComponent<GameObjectData>() == null)
        {
            data = rootGo.AddComponent<GameObjectData>(); //重新添加一个新的
        }
        else
        {
            DestroyImmediate(rootGo.GetComponent<GameObjectData>(), true);
            data = rootGo.AddComponent<GameObjectData>();
        }

        //如果不是lua 生成UI代码
        if(!forLua)
        {
            GenUICode(rootGo, data);
        }

        //标脏
        EditorUtility.SetDirty(rootGo);

        return true;
    }

    private static void GenUICode(GameObject root,GameObjectData data)
    {
        List<GameObject> controls = new List<GameObject>(); // 装所有控件
        List<GameObject> groups = new List<GameObject>();   // 
        Traverse(root, controls, groups, false);
        Dictionary<string, int> usedNames = new Dictionary<string, int>();

        //如果原先有代码生成，就先将原来的添加到gameobjectList中
        List<string> gameobjectList = getGameobjectStrListByFlag(ToolsConst.UI_CODE_PATH + root.name + ".cs");//这个是代码原有的
        Dictionary<string, GameObject> gameObjectDic = TransListToDic(controls); //将所有的控件建立名字和GameObject映射,这个是重新生成的

        string flag = string.Empty;

        if (gameobjectList.Count > 0)
        {
            for (int i = 0; i < gameobjectList.Count; i++)
            {
                if (gameObjectDic.ContainsKey(gameobjectList[i])) //如果有这个控件包含了列表该元素
                {
                    data.GameObjects.Add(gameObjectDic[gameobjectList[i]]); //添加这个对象到data中
                    flag += gameobjectList[i] + ",";//添加标记
                    gameObjectDic.Remove(gameobjectList[i]); //移除字典里该元素
                    //UnityEditor.EditorUtility.DisplayProgressBar(gameobjectList[i], "", (float)i/(float));
                    continue;
                }
            }
        }
        //这是原来代码没有的控件
        foreach (KeyValuePair<string, GameObject> go in gameObjectDic)
        {
            data.GameObjects.Add(go.Value);
            flag += go.Value.name + ",";
        }

        //Generator
        Generator gen = new Generator();
        if(flag.Contains(","))
        {

        }

    }
    
    private static void Traverse(GameObject go,List<GameObject> controls,List<GameObject> groups,bool inGroup)
    {
        if (go == null)
            return;

        bool grouped = inGroup;
        foreach(Transform child in go.transform)
        {
            grouped = inGroup;
            go = child.gameObject;
            if(grouped)
            {
                if(go.GetComponent<GroupName>()!= null)
                    groups.Add(go);
            }
            else
            {
                if (IsControll(go))
                    controls.Add(go);
                if(go.GetComponent<GroupName>() != null)
                {
                    grouped = true;
                    groups.Add(go);
                }
            }

            Traverse(child.gameObject, controls, groups, grouped);
        }
    }
    
    private static bool IsControll(GameObject go)
    {
        int index = go.name.IndexOf("_");
        if (index <= 0)
            return false;

        string prefix = go.name.Substring(0, index);
        return ControllPrefixes.Contains(prefix);
    }

    private static void GenGroupCode(GameObject root)
    {

    }

    private sealed class GameObjectNameCMP : IComparer<GameObject>
    {
        public int Compare(GameObject x,GameObject y)
        {
            return x.name.CompareTo(y.name);
        }
    }
    /// <summary>
    /// 建立一个名字与go的映射表
    /// </summary>
    /// <param name="gameobjects"></param>
    /// <returns></returns>
    static Dictionary<string,GameObject> TransListToDic(List<GameObject> gameobjects)
    {
        Dictionary<string, GameObject> gameobjectDic = new Dictionary<string, GameObject>();
        for (int i = 0, max = gameobjects.Count; i < max; i++)
        {
            if (gameobjectDic.ContainsKey(gameobjects[i].name))
            {
                gameobjectDic.Add(gameobjects[i].name + "_" + i, gameobjects[i]);
            }
            else
            {
                gameobjectDic.Add(gameobjects[i].name, gameobjects[i]);
            }
        }
        return gameobjectDic;
    }

    static List<string> getGameobjectStrList(string path)
    {
     /*   List<string> gameobjectStrDic = new List<string>();
        if (!File.Exists(path)) return gameobjectStrDic;
        StreamReader sr = new StreamReader(path,encoding.);*/
    }

    /// <summary>
    /// 根据标记//-L-<>返回列表
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    static List<string> getGameobjectStrListByFlag(string path)
    {
        string[] gameObjectNames = new string[0];

        if (!File.Exists(path)) return new List<string>();
        StreamReader sr = new StreamReader(path, Encoding.Default);
        string line;
        while((line = sr.ReadLine()) != null)
        {
            if (line.Contains("//-L-<")) ;
            {
                line = line.Substring(line.IndexOf("<"));
                int index = line.IndexOf(">");
                if(index > 0)
                {
                    line = line.Substring(1, index - 1);
                }
                gameObjectNames = line.Split(',');
                break;
            }
        }
        sr.Close();
        List<string> gameobjectStrList = new List<string>(gameObjectNames);
        return gameobjectStrList;
    }

    private static void CleanAll(string folderPath)
    {
        DirectoryInfo rootFolder = new DirectoryInfo(folderPath);
        foreach(FileInfo file in rootFolder.GetFiles())
        {
            string fileName = file.FullName;
            if (file.Name.EndsWith(".prefab"))
            {
                Debug.LogError("filename:" + fileName);
                GameObject go = AssetDatabase.LoadAssetAtPath(fileName.Substring(fileName.LastIndexOf("Assets")), typeof(GameObject)) as GameObject;
            }
        }
        foreach(DirectoryInfo folder in rootFolder.GetDirectories())
        {
            CleanAll(folder.FullName);
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
        
        while(t != null)
        {
            DestroyImmediate(t);
            t = go.GetComponent<T>();
        }
    }
}
