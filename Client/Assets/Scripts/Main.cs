using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	void Awake()
    {
        Application.targetFrameRate = 60;
        /*接下来初始化各个模块*/

    }
    
    // Use this for initialization
	IEnumerator Start () {
        yield return StartCoroutine(LoadAllCfg()); //加载所有的配置文件
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    private IEnumerator LoadAllCfg()
    {
        yield return null;
    }

    private void OnDestroy()
    {

    }
}
