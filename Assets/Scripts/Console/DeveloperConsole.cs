using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Interfaces;

public class DeveloperConsole
{
    private readonly string _prefix;
    private readonly IEnumerable<IDevCommand> _commands;

    public DeveloperConsole(string prefix, IEnumerable<IDevCommand> commands)
    {
        _prefix = prefix;
        _commands = commands;
    }

    public IDevCommandResult ProcessCommand(string inputValue)
    {
        if (!IsValid(inputValue))
            return DevCommandResult.NotFound();

        (string commandInput, string[] args) = GetParsedInput(inputValue);

        return ExecuteCommand(commandInput, args);
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

    public IDevCommandResult ExecuteCommand(string commandInput, string[] args)
    {
        foreach (IDevCommand command in _commands)
        {
            if (commandInput.Equals(command.CommandWord, StringComparison.OrdinalIgnoreCase))
            {
                return command.Process(args);
            }
        }

        return DevCommandResult.NotFound();
    }
}
