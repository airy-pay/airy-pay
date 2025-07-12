using AiryPayNew.Domain.Entities.Bills;

namespace AiryPayNew.Shared.Messaging.Contracts;

public record AssignRoleMessage(ulong GuildId, ulong UserId, ulong RoleId, BillId BillId);