using YamlDotNet.Serialization;

namespace AiryPayNew.Shared.Settings.Models;

public class Color
{
    [YamlMember(typeof(byte), Alias = "R")]
    public byte R { get; set; }
    
    [YamlMember(typeof(byte), Alias = "G")]
    public byte G { get; set; }
    
    [YamlMember(typeof(byte), Alias = "G")]
    public byte B { get; set; }
}