using AiryPayNew.Shared.Messaging;
using RabbitMQ.Client;

namespace AiryPayNew.Discord.Services.Messaging;

public class RoleAssignmentListenerService(
    ILogger<RoleAssignmentListenerService> logger,
    IRabbitMqConnectionManager connectionManager,
    RoleAssignmentConsumer consumer) : BackgroundService
{
    private const string QueueName = "airypaynew_discord.assign_role_queue";
    
    private IConnection? _connection;
    private IChannel? _channel;
    
    public override async Task StartAsync(CancellationToken cancellationToken)
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

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null)
        {
            logger.LogWarning("Queue channel is unavailable.");
            return;
        }
        
        await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: true,
            consumer: consumer, cancellationToken: stoppingToken);
    }
}