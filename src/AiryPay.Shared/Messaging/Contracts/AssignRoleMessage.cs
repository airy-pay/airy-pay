using AiryPay.Domain.Entities.Bills;

namespace AiryPay.Shared.Messaging.Contracts;

public record AssignRoleMessage(ulong GuildId, ulong UserId, ulong RoleId, BillId BillId);