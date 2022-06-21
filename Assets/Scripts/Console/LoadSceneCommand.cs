using Assets.Scripts.Interfaces;
using UnityEngine;

// TODO: update to properly use scene state manager instead of loading scenes directly
[CreateAssetMenu(fileName =nameof(LoadSceneCommand), menuName = "Console/LoadSceneCommand")]
public class LoadSceneCommand : BaseDevCommand
{
    public override IDevCommandResult Process(string[] args)
    {
        if (args.Length == 0)
            return DevCommandResult.Failed("No scene name provided.");

        if (!SceneLoader.CanLoadScene(args[0]))
            return DevCommandResult.Failed($"Can't load scene '{args[0]}'. Ensure that it exists AND is included in the build settings.");

        SceneLoader.LoadScene(args[0]);

        return DevCommandResult.Success();
    }
}
