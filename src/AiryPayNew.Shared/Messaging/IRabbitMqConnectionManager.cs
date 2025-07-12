using RabbitMQ.Client;

namespace AiryPayNew.Shared.Messaging;

public interface IRabbitMqConnectionManager
{
    Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    Task<IChannel> CreateChannelAsync(IConnection connection, CancellationToken cancellationToken);
}
