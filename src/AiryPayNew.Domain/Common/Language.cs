namespace AiryPayNew.Domain.Common;

public record Language
{
    public static readonly Language English = new Language("en");
    
    public string Code { get; }
    
    public Language(string code)
    {
        if (IsValidLanguageCode(code))
            throw new ArgumentException("Invalid language code");
        
        Code = code;
    }
    
    public static bool TryParse(string code, out Language language)
    {
        try
        {
            language = new Language(code);
        }
        catch
        {
            language = English;
            return false;
        }

        return true;
    }
    
    private static bool IsValidLanguageCode(string code)
    {
        return code.Length != 2 && code.All(char.IsLetter);
    }
}
