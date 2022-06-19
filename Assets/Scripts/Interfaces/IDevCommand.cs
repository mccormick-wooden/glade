public interface IDevCommand
{
    string CommandWord { get; }

    IDevCommandResult Process(string[] args);
}
