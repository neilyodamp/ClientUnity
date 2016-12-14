using UnityEngine;
using System.Collections;
using UnityEditor;

public class SceneTools : EditorWindow {

	public static void ExportScene()
    {
        string currentScene = EditorApplication.currentScene;
        string currentSceneName = currentScene.Substring(currentScene.LastIndexOf('/') + 1, currentScene.LastIndexOf('.') - currentScene.LastIndexOf('/') - 1);
        BuildPipeline.BuildStreamedSceneAssetBundle(new string[1] { EditorApplication.currentScene },
            "Assets/StreamingAssets/scene/" + currentSceneName + ".ab", 
            EditorUserBuildSettings.activeBuildTarget); //对场景进行了打ab处理
    }

    private static void RemoveColliders()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            Collider[] colliders = go.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                if (collider is TerrainCollider)
                    continue;

                DestroyImmediate(collider);
            }
        }
    }

    [MenuItem("Tools/Scene/Bake Lightmap")]
    private static void BakeLightmap()
    {
        LightmapEditorSettings.maxAtlasHeight = 512;
        LightmapEditorSettings.maxAtlasWidth = 512;
    }

}
