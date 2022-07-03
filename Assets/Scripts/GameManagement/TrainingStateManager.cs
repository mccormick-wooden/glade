using UnityEngine;
using UnityEngine.UI;

public class TrainingStateManager : BaseStateManager
{
    /// <summary>
    /// Controls whether the training will be skipped
    /// </summary>
    [Header("Scene Settings")]
    [SerializeField]
    private bool skipTraining = false;

    //[SerializeField]
    //private string mainExitRootName = "MainExitGame";

    protected void FixedUpdate()
    {
#if UNITY_EDITOR
        if (skipTraining)
            GameManager.UpdateGameState(GameState.Level1);
#endif
    }

    protected override void OnSceneLoaded()
    {
        //Utility.AddButtonCallback(mainNewGameRootName, () => GameManager.UpdateGameState(GameState.NewGame));
        //Utility.AddButtonCallback(mainExitRootName, () => Quitter.QuitGame());
        //GameObject.Find(mainNewGameRootName).GetComponentInChildren<Button>().Select();
    }

    protected override void OnSceneUnloaded()
    {
    }
}
