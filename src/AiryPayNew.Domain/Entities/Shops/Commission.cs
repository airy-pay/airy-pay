namespace AiryPayNew.Domain.Entities.Shops;

public class Commission
{
    public decimal Value { get; } = 0m;

    public Commission(decimal value)
    {
        if (value is < 0 or > 100)
            throw new ArgumentException("Commission has invalid value: " + value);
        
        Value = value;
    }
}