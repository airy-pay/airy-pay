using AiryPay.Shared.Messaging.Contracts;

namespace AiryPay.Application.Common;

public interface IRoleAssignmentQueueService
{
    Task EnqueueAsync(AssignRoleMessage message, CancellationToken cancellationToken = default);
}