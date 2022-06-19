using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // MonoBehaviour method documentation - https://docs.unity3d.com/ScriptReference/MonoBehaviour.html

    [SerializeField]
    private string sceneToLoad;

    public void LoadScene() 
    {
        LoadScene(sceneToLoad);
    }

    public static void LoadScene(string sceneToLoad)
    {
        if (CanLoadScene(sceneToLoad))
            SceneManager.LoadScene(sceneToLoad);
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
}
