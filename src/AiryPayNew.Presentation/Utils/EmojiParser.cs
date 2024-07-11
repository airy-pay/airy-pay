using Discord;
using Discord.WebSocket;

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

    public static IEmote GetEmoji(string emojiText)
    {
        var parsedAsEmoji = Emoji.TryParse(emojiText, out var emoji);
        var parsedAsEmote = Emote.TryParse(emojiText, out var emote);
        if (!parsedAsEmoji && !parsedAsEmote)
            throw new ArgumentException("Both emoji texts are invalid");
        
        return parsedAsEmoji ? emoji : emote;
    }
    
    public static async Task<bool> IsEmoteDeletedAsync(IGuild guild, IEmote emote)
    {
        if (emote is not Emote guildEmote)
            return false;
        
        var emotes = await guild.GetEmotesAsync();
        return emotes.All(e => e.Id != guildEmote.Id);
    }

    public static async Task<IEmote> GetExistingEmojiAsync(IGuild guild, string emojiText)
    {
        var isEmoteDeleted = await IsEmoteDeletedAsync(
            guild, GetEmoji(emojiText));
        return isEmoteDeleted
            ? GetEmoji(":package:")
            : GetEmoji(emojiText);
    }
}