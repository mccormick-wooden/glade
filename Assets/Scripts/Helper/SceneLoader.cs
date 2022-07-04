using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string sceneToLoad)
    {
        if (CanLoadScene(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public static void ReloadCurrentScene()
    {
        LoadScene(GetCurrentSceneName());
    }
    
    public static bool CanLoadScene(string sceneToLoad)
    {
        if (string.IsNullOrEmpty(sceneToLoad))
            return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var lastSlash = scenePath.LastIndexOf("/");
            var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

            if (string.Compare(sceneToLoad, sceneName, true) == 0)
                return true;
        }

        return false;
    }

    public static string GetCurrentSceneName()
    {
        Scene scene = new Scene();
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            scene = SceneManager.GetSceneByBuildIndex(i);
            if (scene.isLoaded)
                break;
        }
        return scene.name;
    }
}
