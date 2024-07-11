using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.AppSettings;

public class PaymentMethod
{
    [YamlMember(typeof(string), Alias = "serviceName")]
    public required string ServiceName { get; set; }
    
    [YamlMember(typeof(string), Alias = "methodId")]
    public required string MethodId { get; set; }
    
    [YamlMember(typeof(string), Alias = "discordEmoji")]
    public required string DiscordEmoji { get; set; }
    
    [YamlMember(typeof(string), Alias = "name")]
    public required string Name { get; set; }
    
    [YamlMember(typeof(string), Alias = "description")]
    public required string Description { get; set; }
    
    [YamlMember(typeof(decimal), Alias = "minimalSum")]
    public required decimal MinimalSum { get; set; }
}