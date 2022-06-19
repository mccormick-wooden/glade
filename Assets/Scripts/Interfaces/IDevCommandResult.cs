namespace Assets.Scripts.Interfaces
{
    public interface IDevCommandResult
    {
    bool WasSuccessful { get; }

    string Response { get; }
    }
}
