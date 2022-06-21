using Assets.Scripts.Interfaces;

public class DevCommandResult : IDevCommandResult
{
    public bool WasSuccessful { get; private set; } = false;

    public string Response { get; private set; } = null;

    private DevCommandResult() { }

    public static DevCommandResult Success()
    {
        return new DevCommandResult { WasSuccessful = true };
    }

    public static DevCommandResult Failed(string response)
    {
        return new DevCommandResult { Response = response };
    }

    public static DevCommandResult NotFound()
    {
        return new DevCommandResult { Response = "Command not found." };
    }
}
