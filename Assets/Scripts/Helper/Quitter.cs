using UnityEngine;

public class Quitter : MonoBehaviour
{
    public static void QuitGame()
    {
#if UNITY_EDITOR
        GameManager.instance.InvokeTransition(() => UnityEditor.EditorApplication.isPlaying = false);
#else
        GameManager.instance.InvokeTransition(() => Application.Quit());
#endif
    }
}
