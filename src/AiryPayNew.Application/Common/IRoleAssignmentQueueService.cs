using AiryPayNew.Shared.Messaging.Contracts;

namespace AiryPayNew.Application.Common;

public interface IRoleAssignmentQueueService
{
    Task EnqueueAsync(AssignRoleMessage message, CancellationToken cancellationToken = default);
}