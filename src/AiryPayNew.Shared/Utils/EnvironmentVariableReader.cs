namespace AiryPayNew.Shared.Utils;

public static class EnvironmentVariableReader
{
    public static string Get(string environmentVariable)
    {
        var data = Environment.GetEnvironmentVariable(environmentVariable);
        if (data is null)
            throw new InvalidOperationException($"Environment variable {environmentVariable} is missing");

        return data;
    }
}