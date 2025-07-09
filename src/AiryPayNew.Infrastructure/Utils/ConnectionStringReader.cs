using System.Text;

namespace AiryPayNew.Infrastructure.Utils;

internal record DatabaseCredential(string ConnectionStringName, string EnvironmentVariableName);

internal static class ConnectionStringReader
{
    private static readonly IList<DatabaseCredential> DatabaseCredentials =
    [
        new ("Host", "POSTGRES_HOST"),
        new ("Port", "POSTGRES_PORT"),
        new ("Database", "POSTGRES_DB"),
        new ("Userid", "POSTGRES_USER"),
        new ("Pwd", "POSTGRES_PASSWORD"),
    ];
    
    public static string GetString()
    {
        var stringBuilder = new StringBuilder();
        
        foreach (var credential in DatabaseCredentials)
        {
            stringBuilder.Append(
                GetConnectionStringElement(
                    credential.ConnectionStringName,
                    credential.EnvironmentVariableName));
        }

        stringBuilder.Append("Pooling=true;");

        return stringBuilder.ToString();
    }

    private static string GetConnectionStringElement(string connectionStringParameterName, string connectionStringValue)
    {
        return $"{connectionStringParameterName}={GetDatabaseCredential(connectionStringValue)};";
    }
    
    private static string GetDatabaseCredential(string environmentVariable)
    {
        var data = Environment.GetEnvironmentVariable(environmentVariable);
        if (data is null)
            throw new InvalidOperationException($"Environment variable {environmentVariable} is missing");

        return data;
    }
}