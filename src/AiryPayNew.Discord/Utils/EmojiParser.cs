using Discord;

namespace AiryPayNew.Discord.Utils;

public static class EmojiParser
{
    public static async Task<string?> GetEmojiText(string emojiText)
    {
        var parsedAsEmoji = Emoji.TryParse(emojiText, out var emoji);
        var parsedAsEmote = Emote.TryParse(emojiText, out var emote);
        if (!parsedAsEmoji && !parsedAsEmote)
            return null;
        
        return parsedAsEmoji ? emoji.ToString() : emote.ToString();
    }
}