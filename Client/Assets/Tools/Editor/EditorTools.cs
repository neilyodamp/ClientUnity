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
}
