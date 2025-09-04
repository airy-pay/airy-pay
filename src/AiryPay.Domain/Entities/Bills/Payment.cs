namespace AiryPay.Domain.Entities.Bills;

public class Payment
{
    public long SystemId { get; set; }
    public string SystemName { get; set; } = string.Empty;
    public string MethodId { get; set; } = string.Empty;
}