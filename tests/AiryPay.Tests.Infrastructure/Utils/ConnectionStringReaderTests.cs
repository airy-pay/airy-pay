using AiryPay.Infrastructure.Utils;
using FluentAssertions;

namespace AiryPay.Tests.Infrastructure.Utils;

public class ConnectionStringReaderTests
{
    [Fact]
    public void GetString_WhenAllEnvVarsSet_ShouldReturnValidConnectionString()
    {
        SetPostgresEnvVars("localhost", "5432", "testdb", "user", "secret");

        try
        {
            var result = ConnectionStringReader.GetString();

            result.Should().Contain("Host=localhost;");
            result.Should().Contain("Port=5432;");
            result.Should().Contain("Database=testdb;");
            result.Should().Contain("Userid=user;");
            result.Should().Contain("Pwd=secret;");
            result.Should().Contain("Pooling=true;");
        }
        finally
        {
            ClearPostgresEnvVars();
        }
    }

    [Fact]
    public void GetString_WhenEnvVarMissing_ShouldThrowInvalidOperationException()
    {
        SetPostgresEnvVars("localhost", "5432", "testdb", "user", "secret");
        try
        {
            Environment.SetEnvironmentVariable("POSTGRES_PASSWORD", null);

            var act = () => ConnectionStringReader.GetString();

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*POSTGRES_PASSWORD*");
        }
        finally
        {
            SetPostgresEnvVars("localhost", "5432", "testdb", "user", "secret");
            ClearPostgresEnvVars();
        }
    }

    private static void SetPostgresEnvVars(string host, string port, string db, string user, string pwd)
    {
        Environment.SetEnvironmentVariable("POSTGRES_HOST", host);
        Environment.SetEnvironmentVariable("POSTGRES_PORT", port);
        Environment.SetEnvironmentVariable("POSTGRES_DB", db);
        Environment.SetEnvironmentVariable("POSTGRES_USER", user);
        Environment.SetEnvironmentVariable("POSTGRES_PASSWORD", pwd);
    }

    private static void ClearPostgresEnvVars()
    {
        Environment.SetEnvironmentVariable("POSTGRES_HOST", null);
        Environment.SetEnvironmentVariable("POSTGRES_PORT", null);
        Environment.SetEnvironmentVariable("POSTGRES_DB", null);
        Environment.SetEnvironmentVariable("POSTGRES_USER", null);
        Environment.SetEnvironmentVariable("POSTGRES_PASSWORD", null);
    }
}
