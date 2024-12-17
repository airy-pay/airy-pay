namespace AiryPayNew.Domain.Common;

public record Language
{
    public string Code { get; }
    
    public Language(string code)
    {
        if (code.Length != 2)
            throw new ArgumentException("Invalid language code");
        
        Code = code;
    }
}
