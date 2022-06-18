using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneLoader : MonoBehaviour
{
    // MonoBehaviour method documentation - https://docs.unity3d.com/ScriptReference/MonoBehaviour.html

    [SerializeField]
    private string sceneToLoad;

    public void LoadMenuScene() 
    {
        if (string.IsNullOrEmpty(sceneToLoad)) 
            throw new ArgumentNullException(nameof(sceneToLoad));

        SceneManager.LoadScene(sceneToLoad);
    }
    
}
