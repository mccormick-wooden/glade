using UnityEngine;

[CreateAssetMenu(fileName =nameof(LoadSceneCommand), menuName = "Console/LoadSceneCommand")]
public class LoadSceneCommand : BaseDevCommand
{
    public override IDevCommandResult Process(string[] args)
    {
        if (args.Length == 0 || !SceneLoader.CanLoadScene(args[0]))
            return DevCommandResult.Failed(args.Length == 0 ? "No scene name provided" : $"Can't load scene '{args[0]}'. Ensure that it exists AND is included in the build settings.");

        SceneLoader.LoadScene(args[0]);

        return DevCommandResult.Success();
    }
}
