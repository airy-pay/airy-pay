using AiryPayNew.Domain.Common.Result;
using AiryPayNew.Domain.Entities.ShopComplaints;
using AiryPayNew.Domain.Entities.Shops;
using MediatR;

namespace AiryPayNew.Application.Requests.ShopComplaints;

using Error = CreateShopComplaintRequest.Error;

public record CreateShopComplaintRequest(
    ShopId ShopId,
    ulong CreatorDiscordUserId,
    string Reason,
    string Details)
    : IRequest<Result<CreateShopComplaintRequest.Error>>
{
    public enum Error
    {
        NoReasonAndDetails,
        TooManyComplaints
    }
}

public class CreateShopComplaintRequestHandler(
    IShopComplaintRepository shopComplaintRepository,
    IShopRepository shopRepository)
    : IRequestHandler<CreateShopComplaintRequest, Result<Error>>
{
    public async Task<Result<Error>> Handle(
        CreateShopComplaintRequest request, CancellationToken cancellationToken)
    {
        var resultBuilder = new ResultBuilder<Error>();   
        
        if (string.IsNullOrEmpty(request.Reason) || string.IsNullOrEmpty(request.Details))
            return resultBuilder.WithError(Error.NoReasonAndDetails);

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
            return resultBuilder.WithError(Error.TooManyComplaints);
        
        await shopComplaintRepository.CreateAsync(newShopComplaint, cancellationToken);
        
        return resultBuilder.WithSuccess();
    }
}