
using UnityEngine;

public abstract class BaseLevelManager : BaseSceneManager
{
    private GameObject player;

    protected override void OnSceneLoaded()
    {
        player = GameObject.Find("ybot");
    }

    protected override void OnSceneUnloaded()
    {
    }

    protected override void Update()
    {
        base.Update();
    }
}

