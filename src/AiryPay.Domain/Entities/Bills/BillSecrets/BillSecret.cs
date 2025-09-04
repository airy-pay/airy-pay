namespace AiryPay.Domain.Entities.Bills.BillSecrets;

public class BillSecret
{
    public readonly string Key;

    public BillSecret(IBillSecretGenerator billSecretGenerator)
    {
        Key = billSecretGenerator.Generate();
    }
    
    public BillSecret(string key)
    {
        Key = key;
    }
}