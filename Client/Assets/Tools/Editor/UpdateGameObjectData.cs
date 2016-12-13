using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Text;
public class UpdateGameObjectData : Editor {

    [MenuItem("Tools/UpdateGameObjectData")]
    static void _UpdateGameObjectData()
    {
        Init();
        ExportAll(ToolsConst.UIPrefabPath);
    }

    [MenuItem("Tools/UpdateThisPrefabGameObjectData")]
    [MenuItem("Assets/UpdateThisPrefabGameObjectData")]
    static void UpdateThisPrefabGameObjectData()
    {
        Init();
        GameObject go = UnityEditor.Selection.activeGameObject;
        Export(go);
    }

    public static void ClearPrefabScript()
    {
        CleanAll(ToolsConst.UIPrefabPath);
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
        //将标记写入文本中
        if (flag.Contains(","))
            flag = flag.Substring(0, flag.LastIndexOf(","));
        gen.Println("//-L-<" + flag + ">");
        gen.Println("using UnityEngine;");
        gen.Println();
        gen.Println("public partial class " + root.name + " : View");
        gen.Println("{");
        gen.AddIndent();
        foreach (GameObject controlObject in data.GameObjects) //遍历控件对象 声明变量
        {
            int times = 1;
            string name = controlObject.name;
            if (usedNames.TryGetValue(name, out times))
            {
                name = name + "_" + times;
                ++times;
            }
            usedNames[controlObject.name] = times;

            GroupName groupName = controlObject.GetComponent<GroupName>();
            //如果有UIGameObjectList组件
            if (controlObject.GetComponent<UIGameObjectList>() != null)
            {
                gen.Printfln("private GameObject[] {0};", name); //声明这个数组
                gen.Printfln("private GameObject {0}Obj;", name);
            }
            else if (groupName == null) //如果没有组
            {
                gen.Println("private GameObject " + name + ";");
            }
            else
            {
                gen.Printfln("private {0} {1};", groupName.groupName, name);
            }
        }
        gen.Println();
        gen.Println("protected override void Awake()");
        gen.Println("{");
        gen.AddIndent();
        gen.Println("base.Awake();");
        gen.Println();
        gen.Println("GameObjectData data = gameObject.GetComponent<GameObjectData>();");
        usedNames.Clear();
        for (int i = 0, max = data.GameObjects.Count; i < max; i++)
        {
            int times = 1;
            string name = data.GameObjects[i].name;
            if (usedNames.TryGetValue(name, out times))
            {
                name = name + "_" + times;
                ++times;
            }
            usedNames[data.GameObjects[i].name] = times;

            GroupName groupName = data.GameObjects[i].GetComponent<GroupName>();
            //如果有UIGameObjectList
            if (data.GameObjects[i].GetComponent<UIGameObjectList>() != null)
            {
                gen.Printfln("{0} = data.GameObjects[{1}].gameObject.GetComponent<UIGameObjectList>().objects;", name, i.ToString());
                gen.Printfln("{0}Obj = data.GameObjects[{1}].gameObject;", name, i.ToString());

                //gen.Printfln("{0} = transform.Find(@\"{1}\").gameObject.GetComponent<UIGameObjectList>().objects;", name, GetPath(controls[i], root));
                //gen.Printfln("{0}Obj = transform.Find(@\"{1}\").gameObject;", name, GetPath(controls[i], root));
            }
            else if (groupName == null)
            {
                gen.Println(name + " =  data.GameObjects[" + i.ToString() + "].gameObject;");
                //gen.Println(name + " =  transform.Find(@\"" + GetPath(controls[i], root) + "\").gameObject;");
            }
            else
            {
                gen.Printfln("{0} = View.AddComponentIfNotExist<{1}>(data.GameObjects[" + i.ToString() + "].gameObject);", name, groupName.groupName);

                //gen.Printfln("{0} = View.AddComponentIfNotExist<{2}>(transform.Find(@\"{1}\").gameObject);", name, GetPath(controls[i], root), groupName.groupName);
            }
        }

        gen.Println("ViewMgr.Ins.addView(this);");
        gen.ReduceIndent();
        gen.Println("}");
        gen.Println();

        HashSet<string> groupNames = new HashSet<string>();
        foreach (GameObject group in groups)
        {
            string groupName = group.GetComponent<GroupName>().groupName;
            if (groupNames.Contains(groupName))
                continue;

            GenGroupCode(group); //生成Group 代码
            groupNames.Add(groupName);
            gen.Println();
        }

        gen.ReduceIndent();
        gen.Println("}");
        StreamWriter sw = new StreamWriter(ToolsConst.UI_CODE_PATH + root.name + ".cs", false);
        sw.Write(gen.GetContent());
        sw.Flush();
        sw.Close();
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

    /// <summary>
    /// 生成GroupName代码，这里可以改进生成内部类
    /// </summary>
    /// <param name="root"></param>
    private static void GenGroupCode(GameObject root)
    {
        Generator gen = new Generator();
        gen.Println("using UnityEngine;");
        gen.Println();

        List<GameObject> controls = new List<GameObject>();
        List<GameObject> groups = new List<GameObject>();
        Traverse(root, controls, groups, false);
        controls.Sort(new GameObjectNameCMP());
        string clazzName = root.GetComponent<GroupName>().groupName;
        gen.Println("public partial class " + clazzName + " : MonoBehaviour");
        gen.Println("{");
        gen.AddIndent();

        Dictionary<string, int> usedNames = new Dictionary<string, int>();
        foreach (GameObject controlObject in controls)
        {
            int times = 1;
            string name = controlObject.name;
            if (usedNames.TryGetValue(name, out times))
            {
                name = name + "_" + times;
                ++times;
            }
            usedNames[controlObject.name] = times;
            GroupName groupName = controlObject.GetComponent<GroupName>();
            if (controlObject.GetComponent<UIGameObjectList>() != null)
            {
                gen.Printfln("public GameObject[] {0};", name);
                gen.Printfln("public GameObject {0}Obj;", name);
            }
            else if (groupName == null)
            {
                gen.Println("public GameObject " + name + ";");
            }
            else if (controlObject.GetComponent<UIGameObjectList>() != null)
            {
                gen.Printfln("public GameObject[] {0};", name);
            }
            else
            {
                gen.Printfln("public {0} {1};", groupName.groupName, name);
            }
        }

        gen.Println();

        gen.Println("private void Awake()");
        gen.Println("{");
        gen.AddIndent();

        usedNames.Clear();
        foreach (GameObject controlObject in controls)
        {
            int times = 1;
            string name = controlObject.name;
            if (usedNames.TryGetValue(name, out times))
            {
                name = name + "_" + times;
                ++times;
            }
            usedNames[controlObject.name] = times;
            GroupName groupName = controlObject.GetComponent<GroupName>();
            if (controlObject.GetComponent<UIGameObjectList>() != null)
            {
                gen.Printfln("{0} = transform.Find(@\"{1}\").gameObject.GetComponent<UIGameObjectList>().objects;", name, EditorTools.GetPath(controlObject, root));
                gen.Printfln("{0}Obj = transform.Find(@\"{1}\").gameObject;", name, EditorTools.GetPath(controlObject, root));
            }
            else if (groupName == null)
            {
                gen.Println(name + " =  transform.Find(@\"" + EditorTools.GetPath(controlObject, root) + "\").gameObject;");
            }
            else
            {
                gen.Printfln("{0} = View.AddComponentIfNotExist<{2}>(transform.Find(@\"{1}\").gameObject);", name, EditorTools.GetPath(controlObject, root), groupName.groupName);
            }
        }

        gen.ReduceIndent();
        gen.Println("}");

        gen.ReduceIndent();
        gen.Println("}");

        //组不会再递归新的组

        StreamWriter sw = new StreamWriter(ToolsConst.UI_CODE_PATH + clazzName + ".cs", false);
        sw.Write(gen.GetContent());
        sw.Flush();
        sw.Close();
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
        List<string> gameobjectStrDic = new List<string>();
        if (!File.Exists(path)) return gameobjectStrDic;
        StreamReader sr = new StreamReader(path, Encoding.Default);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            if (line.Contains("private"))
            {
                string sStr = "";
                if (line.IndexOf(' ') > 0)
                {
                    sStr = line.Substring(line.LastIndexOf(" "));
                    int index = sStr.IndexOf(";");
                    if (index > 0)
                    {
                        sStr = sStr.Substring(0, index);
                    }
                    gameobjectStrDic.Add(sStr.Trim());
                }
            }
        }
        sr.Close();
        return gameobjectStrDic;
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
