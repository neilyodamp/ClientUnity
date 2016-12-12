using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
public class UpdateGameObjectData : Editor {

	static void _UpdateGameObjectData()
    {

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
        foreach(FileInfo file in rootFolder.GetFiles())
        {
            string fileName = file.FullName;
            if(file.Name.EndsWith(".prefab"))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath(fileName.Substring(fileName.LastIndexOf("Assets")),
                    typeof(GameObject)) as GameObject;
            }
        }
    }

    private static bool Export(GameObject rootGo,bool forLua = false)
    {
        
    }

    private static void GenUICode(GameObject root,GameObject data)
    {

    }
    
    private static void Traverse(GameObject go,List<GameObject> controls,List<GameObject> groups,bool isGroup)
    {

    }
    
    private static bool IsControll(GameObject go)
    {

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
