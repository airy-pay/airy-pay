using System.Text;
using AiryPay.Application.Common;
using AiryPay.Shared.Messaging;
using AiryPay.Shared.Messaging.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace AiryPay.Infrastructure.Services.Messaging;

public class RabbitMqRoleAssignmentQueueService(
    IRabbitMqConnectionManager connectionManager,
    ILogger<RabbitMqRoleAssignmentQueueService> logger)
    : IRoleAssignmentQueueService, IHostedService
{
    private const string QueueName = "airypaynew_discord.assign_role_queue";

    private IConnection? _connection;
    private IChannel? _channel;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = await connectionManager.CreateConnectionAsync(cancellationToken);
        _channel = await connectionManager.CreateChannelAsync(_connection, cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        logger.LogInformation($"Role assignment queue '{QueueName}' initialized.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task EnqueueAsync(AssignRoleMessage message, CancellationToken cancellationToken = default)
    {
        if (_channel is null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized.");

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: QueueName,
            mandatory: false,
            body: body,
            cancellationToken: cancellationToken);

        logger.LogInformation($"Message enqueued to queue '{QueueName}'.");
    }
}
