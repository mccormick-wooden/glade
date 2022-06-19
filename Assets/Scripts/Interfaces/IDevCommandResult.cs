public interface IDevCommandResult
{
    bool WasSuccessful { get; }

    string Response { get; }
}
