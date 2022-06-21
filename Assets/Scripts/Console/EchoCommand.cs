using Assets.Scripts.Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName =nameof(EchoCommand), menuName = "Console/EchoCommand")]
public class EchoCommand : BaseDevCommand
{
    public override IDevCommandResult Process(string[] args)
    {
        string logText = string.Join(" ", args);
        if (string.IsNullOrWhiteSpace(logText))
            return DevCommandResult.Failed($"Nothing to log!");

        Debug.Log(logText);

        return DevCommandResult.Success();
    }
}
