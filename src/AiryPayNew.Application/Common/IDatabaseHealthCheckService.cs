namespace AiryPayNew.Application.Common;

public interface IDatabaseHealthCheckService
{
    public Task CheckConnection();
}