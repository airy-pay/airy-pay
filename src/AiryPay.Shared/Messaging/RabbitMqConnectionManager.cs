using AiryPay.Shared.Utils;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AiryPay.Shared.Messaging;

public class RabbitMqConnectionManager(
    ILogger<RabbitMqConnectionManager> logger) : IRabbitMqConnectionManager
{
    public async Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = EnvironmentVariableReader.Get("RABBITMQ_HOST"),
            Port = int.Parse(EnvironmentVariableReader.Get("RABBITMQ_PORT")),
            UserName = EnvironmentVariableReader.Get("RABBITMQ_USER"),
            Password = EnvironmentVariableReader.Get("RABBITMQ_PASSWORD")
        };

        logger.LogInformation("Creating RabbitMQ connection...");
        return await factory.CreateConnectionAsync(cancellationToken);
    }

    public async Task<IChannel> CreateChannelAsync(IConnection connection, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating RabbitMQ channel...");
        return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }
}
