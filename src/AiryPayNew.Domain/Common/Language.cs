namespace AiryPayNew.Domain.Common;

public class Language
{
    public string Code { get; private set; }

    public Language(string languageCode)
    {
        if (languageCode.Length != 2)
            throw new ArgumentException("Invalid language code");
        
        Code = languageCode.ToLower();
    }
}