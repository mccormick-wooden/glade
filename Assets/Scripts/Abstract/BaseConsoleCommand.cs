using UnityEngine;

public abstract class BaseConsoleCommand : ScriptableObject, IConsoleCommand
{
    [SerializeField]
    private string commandWord = string.Empty;

    public string CommandWord => commandWord;

    public abstract bool Process(string[] args);
}
