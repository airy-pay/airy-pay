namespace AiryPay.Domain.Entities.Bills.BillSecrets.BillSecretGenerators;

public class GuidBillSecretGenerator : IBillSecretGenerator
{
    public string Generate()
    {
        return Guid.NewGuid().ToString();
    }
}