using System.Text;
using AiryPayNew.Shared.Messaging.Contracts;
using Discord.WebSocket;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AiryPayNew.Discord.Services.Messaging;

public class RoleAssignmentConsumer(
    DiscordSocketClient discordSocketClient,
    ILogger<RoleAssignmentConsumer> logger)
    : IAsyncBasicConsumer
{
    // Required by the interface
    public IChannel? Channel { get; set; }

    public async Task HandleBasicDeliverAsync(
        string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IReadOnlyBasicProperties properties,
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = Encoding.UTF8.GetString(body.Span);
            var message = JsonConvert.DeserializeObject<AssignRoleMessage>(json);

            await GiveRoleToUser(message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process role assignment message.");
        }
    }

    private async Task GiveRoleToUser(AssignRoleMessage? message)
    {
        if (message is null)
        {
            logger.LogWarning("Received null role assignment message.");
            return;
        }
            
        var guild = discordSocketClient.GetGuild(message.GuildId);
        if (guild is null)
        {
            logger.LogWarning("Received null guild.");
            return;
        }
            
        var user = guild.GetUser(message.UserId);
        var role = guild.GetRole(message.RoleId);
        if (role is null || user is null)
        {
            logger.LogWarning("Received null role assignment message.");
            return;
        }
            
        await user.AddRoleAsync(role);
            
        logger.LogInformation(
            string.Format("Gave a role to user {0} for bill #{1} in shop {2}",
                user.Id,
                message.BillId.Value,
                message.GuildId));
    }

    public Task HandleBasicConsumeOkAsync(string consumerTag, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(string.Format("Consumer registered: {0}", consumerTag));
        return Task.CompletedTask;
    }

    public Task HandleBasicCancelOkAsync(string consumerTag, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(string.Format("Consumer cancel OK: {0}", consumerTag));
        return Task.CompletedTask;
    }

    public Task HandleBasicCancelAsync(string consumerTag, CancellationToken cancellationToken = default)
    {
        logger.LogWarning(string.Format("Consumer canceled: {0}", consumerTag));
        return Task.CompletedTask;
    }

    public Task HandleChannelShutdownAsync(object channel, ShutdownEventArgs reason)
    {
        logger.LogWarning(string.Format("Channel shutdown: {0}", reason.ReplyText));
        return Task.CompletedTask;
    }
}