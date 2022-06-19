using Assets.Scripts.Interfaces;
using UnityEngine;

public abstract class BaseDevCommand : ScriptableObject, IDevCommand
{
    [SerializeField]
    private string commandWord = string.Empty;

    public string CommandWord => commandWord;

    public abstract IDevCommandResult Process(string[] args);
}
