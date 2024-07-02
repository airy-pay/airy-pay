namespace AiryPayNew.Domain.Entities.Bills.BillSecrets;

public class BillSecret
{
    public string Key;

    public BillSecret(IBillSecretGenerator billSecretGenerator)
    {
        Key = billSecretGenerator.Generate();
    }
}