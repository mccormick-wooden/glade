using System;
using System.Collections.Generic;
using System.Linq;

public class DeveloperConsole
{
    private readonly string _prefix;
    private readonly IEnumerable<IConsoleCommand> _commands;

    public DeveloperConsole(string prefix, IEnumerable<IConsoleCommand> commands)
    {
        _prefix = prefix;
        _commands = commands;
    }

    public void ProcessCommand(string inputValue)
    {
        if (!IsValid(inputValue))
            return;

        (string commandInput, string[] args) = GetParsedInput(inputValue);

        ExecuteCommand(commandInput, args);
    }

    private bool IsValid(string inputValue)
    {
        return !string.IsNullOrWhiteSpace(inputValue)
            && inputValue.StartsWith(_prefix)
            && inputValue.Length >= _prefix.Length + 1;
    }

    private (string, string[]) GetParsedInput(string inputValue)
    {
        string[] inputSplit = inputValue.Remove(0, _prefix.Length).Split(' ');

        return (inputSplit[0], inputSplit.Skip(1).ToArray());
    }

    public void ExecuteCommand(string commandInput, string[] args)
    {
        foreach (IConsoleCommand command in _commands)
        {
            if (commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
            {
                command.Process(args);
                break;
            }
        }
    }
}
