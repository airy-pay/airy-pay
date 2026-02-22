using AiryPay.Discord.Utils;
using Discord;
using Moq;

namespace AiryPay.Tests.Discord.Utils;

public class EmojiParserTests
{
    [Fact]
    public async Task GetEmojiText_WithValidUnicodeEmoji_ShouldReturnEmojiString()
    {
        var result = await EmojiParser.GetEmojiText("😀");

        result.Should().NotBeNull();
        result.Should().Contain("😀");
    }

    [Fact]
    public async Task GetEmojiText_WithInvalidEmoji_ShouldReturnNull()
    {
        var result = await EmojiParser.GetEmojiText("invalid_emoji_xyz");

        result.Should().BeNull();
    }

    [Fact]
    public void GetEmoji_WithValidUnicodeEmoji_ShouldReturnEmoji()
    {
        var result = EmojiParser.GetEmoji("😀");

        result.Should().NotBeNull();
        result.Name.Should().Be("😀");
    }

    [Fact]
    public void GetEmoji_WithInvalidEmoji_ShouldThrowArgumentException()
    {
        var act = () => EmojiParser.GetEmoji("invalid_emoji_xyz");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Both emoji texts are invalid");
    }

    [Fact]
    public async Task IsEmoteDeletedAsync_WhenEmoteIsNotGuildEmote_ShouldReturnFalse()
    {
        var emoji = EmojiParser.GetEmoji("😀");
        var mockGuild = new Mock<IGuild>();

        var result = await EmojiParser.IsEmoteDeletedAsync(mockGuild.Object, emoji);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsEmoteDeletedAsync_WhenGuildEmoteNotInGuild_ShouldReturnTrue()
    {
        var emote = Emote.Parse("<:test:123456789012345678>");
        var mockGuild = new Mock<IGuild>();
        mockGuild
            .Setup(g => g.GetEmotesAsync(It.IsAny<RequestOptions>()))
            .ReturnsAsync(Array.Empty<GuildEmote>());

        var result = await EmojiParser.IsEmoteDeletedAsync(mockGuild.Object, emote);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetExistingEmojiAsync_WhenEmoteDeleted_ShouldReturnFallbackPackageEmoji()
    {
        var mockGuild = new Mock<IGuild>();
        mockGuild
            .Setup(g => g.GetEmotesAsync(It.IsAny<RequestOptions>()))
            .ReturnsAsync(Array.Empty<GuildEmote>());

        var result = await EmojiParser.GetExistingEmojiAsync(mockGuild.Object, "<:deleted:999999999999999999>");

        result.Should().NotBeNull();
        result.Name.Should().ContainAny("package", "📦");
    }

    [Fact]
    public async Task GetExistingEmojiAsync_WhenUnicodeEmoji_ShouldReturnSameEmoji()
    {
        var mockGuild = new Mock<IGuild>();

        var result = await EmojiParser.GetExistingEmojiAsync(mockGuild.Object, "😀");

        result.Name.Should().Be("😀");
    }
}
