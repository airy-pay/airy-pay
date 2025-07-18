using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.ShopComplaints;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.ShopComplaints;

public record CreateShopComplaintRequest(
    ShopId ShopId, ulong CreatorDiscordUserId, string Reason, string Details)
    : IRequest<OperationResult>;

public class CreateShopComplaintRequestHandler(
    IShopComplaintRepository shopComplaintRepository,
    IShopRepository shopRepository)
    : IRequestHandler<CreateShopComplaintRequest, OperationResult>
{
    public async Task<OperationResult> Handle(
        CreateShopComplaintRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Reason) || string.IsNullOrEmpty(request.Details))
            return OperationResult.Error("Reason and details are required");

        var newShopComplaint = new ShopComplaint
        {
            ShopId = request.ShopId,
            CreatorDiscordUserId = request.CreatorDiscordUserId,
            Reason = request.Reason,
            Details = request.Details
        };

        var userComplaints = await shopRepository.GetShopComplaintsAsync(
            request.ShopId, cancellationToken, request.CreatorDiscordUserId);
        if (userComplaints.Count >= 10)
            return OperationResult.Error("You create too many complaints");
        
        await shopComplaintRepository.CreateAsync(newShopComplaint, cancellationToken);
        
        return OperationResult.Success();
    }
}