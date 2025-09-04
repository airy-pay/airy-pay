namespace AiryPay.Discord.Utils;

public static class CardFormatter
{
    public static string Format(string creditCardNumber)
    {
        if (string.IsNullOrEmpty(creditCardNumber))
        {
            throw new ArgumentException("Credit card number cannot be null or empty", nameof(creditCardNumber));
        }

        return string.Join(" ", Enumerable.Range(0, creditCardNumber.Length / 4)
            .Select(i => creditCardNumber.Substring(i * 4, 4)));
    }
}