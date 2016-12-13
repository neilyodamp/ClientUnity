using UnityEngine;
using System.Collections;

public class EditorTools {

    /// <summary>
    /// 获取文件的名称
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileName(string path)
    {
        int begin = path.LastIndexOf("/");
        if(begin < 0)
        {
            begin = path.LastIndexOf("\\");
        }
        int end = path.LastIndexOf(".");
        return path.Substring(begin + 1, end - begin - 1);
    }
    public static string GetParentFileName(string path, int parentIndex)
    {
        if (parentIndex < 1)
            parentIndex = 1;
        string[] filenames = path.Split('/');
        return filenames[filenames.Length - parentIndex];
    }
    /// <summary>
    /// 获得节点的相对路径
    /// </summary>
    /// <param name="go"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public static string GetPath(GameObject go, GameObject root = null)
    {
        string path = go.name;
        Transform parent = go.transform.parent;
        while (parent != null && parent.gameObject != root)
        {
            path = parent.gameObject.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }
}
