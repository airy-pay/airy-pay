namespace AiryPayNew.Discord.Utils;

public static class DiscordTokenReader
{
    private const string DiscordTokenEnvironmentVariableName = "DISCORD_TOKEN";
    
    public static string GetToken()
    {
        var token = Environment.GetEnvironmentVariable(DiscordTokenEnvironmentVariableName);
        if (token is null)
        {
            throw new ArgumentNullException(
                nameof(token), "Environment variable DISCORD_TOKEN was not found");
        }

        return token;
    }
}