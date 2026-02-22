using AiryPay.Application.Requests.Shops;
using AiryPay.Discord.Localization;
using AiryPay.Discord.Utils;
using AiryPay.Domain.Common;
using AiryPay.Domain.Entities.Withdrawals;
using AiryPay.Shared.Settings;
using Discord;
using Discord.Interactions;
using MediatR;

namespace AiryPay.Discord.InteractionModules.Impl;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[RequireUserPermission(GuildPermission.Administrator)]
public class InfoInteractionModule : ShopInteractionModuleBase
{
    private readonly IMediator _mediator;
    private readonly Color _embedsColor;
    private readonly AppSettings _appSettings;

    public InfoInteractionModule(
        IMediator mediator,
        AppSettings appSettings) : base(mediator)
    {
        _mediator = mediator;
        _appSettings = appSettings;

        _embedsColor = ColorMapper.Map(appSettings.Discord.EmbedMessageColor);
    }

    [SlashCommand("info", "🌐 Shop information")]
    public async Task Info()
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var shopInfoEmbed = new EmbedBuilder()
            .WithTitle($"🌐 {localizer.ShopInformation}")
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName($"💰 {localizer.Balance}")
                    .WithValue($"{shop.Balance} ₽")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"🔄 {localizer.Status}")
                    .WithValue(shop.Blocked
                        ? localizer.Blocked
                        : localizer.Active)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"🛍️ {localizer.Products}")
                    .WithValue(shop.Products.Count)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"🏷️ {localizer.ShopId}")
                    .WithValue($"`{Context.Guild.Id}`")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"{localizer.LanguageEmoji} {localizer.Language}")
                    .WithValue(localizer.LanguageName)
                    .WithIsInline(true))
            .WithFooter(
                $"AiryPay © {DateTime.UtcNow.Year}",
                Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        var productsButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.GetProducts")
            .WithLabel(localizer.Products)
            .WithEmote(new Emoji("🛍️"))
            .WithStyle(ButtonStyle.Primary);

        var withdrawalsButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.GetWithdrawals")
            .WithLabel(localizer.Withdrawals)
            .WithEmote(new Emoji("💸"))
            .WithStyle(ButtonStyle.Primary);

        var lastPurchasesButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.GetPurchases")
            .WithLabel(localizer.Purchases)
            .WithEmote(new Emoji("📦"))
            .WithStyle(ButtonStyle.Primary);

        var settingsButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.OpenSettings")
            .WithLabel(localizer.SettingsButton)
            .WithEmote(new Emoji("⚙️"))
            .WithStyle(ButtonStyle.Secondary);

        var supportButton = new ButtonBuilder()
            .WithLabel(localizer.Support)
            .WithUrl("https://discord.gg/Arn9RsRqD9")
            .WithEmote(new Emoji("💬"))
            .WithStyle(ButtonStyle.Link);

        var termsButton = new ButtonBuilder()
            .WithUrl("https://airypay.ru/terms")
            .WithLabel(localizer.Terms)
            .WithEmote(new Emoji("📃"))
            .WithStyle(ButtonStyle.Link);

        var messageComponents = new ComponentBuilder()
            .WithRows([
                new ActionRowBuilder()
                    .WithButton(productsButton)
                    .WithButton(withdrawalsButton)
                    .WithButton(lastPurchasesButton)
                    .WithButton(settingsButton),
                new ActionRowBuilder()
                    .WithButton(supportButton)
                    .WithButton(termsButton),
            ])
            .Build();

        await RespondAsync(embed: shopInfoEmbed, components: messageComponents, ephemeral: true);
    }

    [ComponentInteraction("InfoInteractionModule.GetProducts")]
    public async Task GetProducts()
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var fieldsTasks = shop.Products.Select(async x =>
        {
            var emoji = await EmojiParser.GetExistingEmojiAsync(Context.Guild, x.Emoji);
            return new EmbedFieldBuilder()
                .WithName($"{emoji} {x.Name}")
                .WithValue($"""
                            {localizer.Amount}: **{x.Price} ₽**
                            {localizer.Role}: <@&{x.DiscordRoleId}>
                            """)
                .WithIsInline(true);
        });

        var fields = await Task.WhenAll(fieldsTasks);

        var productsEmbed = new EmbedBuilder()
            .WithTitle($"📦 {localizer.Products}")
            .WithDescription(shop.Products.Count == 0
                ? localizer.NoProducts
                : null)
            .WithFields(fields)
            .WithFooter(
                $"AiryPay © {DateTime.UtcNow.Year}",
                Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        await RespondAsync(embed: productsEmbed, ephemeral: true);
    }

    [ComponentInteraction("InfoInteractionModule.GetWithdrawals")]
    public async Task GetWithdrawals()
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var getShopWithdrawalsRequest = new GetShopWithdrawalsRequest(ShopId);
        var withdrawals = await _mediator.Send(getShopWithdrawalsRequest);

        var withdrawalStatusesGetter = new Dictionary<WithdrawalStatus, string>()
        {
            { WithdrawalStatus.InProcess, $"🟡 {localizer.InProcess}" },
            { WithdrawalStatus.Paid, $"🟢 {localizer.Paid}" },
            { WithdrawalStatus.Canceled, $"🔴 {localizer.Canceled}" },
        };

        var withdrawalsEmbed = new EmbedBuilder()
            .WithTitle($"💸 {localizer.Withdrawals}")
            .WithDescription(withdrawals.Count == 0
                ? localizer.NoWithdrawals
                : null)
            .WithFields(withdrawals.Select(x => new EmbedFieldBuilder()
                .WithName($"💳 {x.DateTime:dd/MM/yyyy H:mm} Card")
                .WithValue($"""
                            {localizer.CardNumber}: ||{CardFormatter.Format(x.ReceivingAccountNumber)}||
                            {localizer.Amount}: **{x.Amount} ₽**
                            {localizer.StatusField}: **{withdrawalStatusesGetter[x.WithdrawalStatus]}**
                            """)
                .WithIsInline(false)))
            .WithFooter($"AiryPay © {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        await RespondAsync(embed: withdrawalsEmbed, ephemeral: true);
    }

    [ComponentInteraction("InfoInteractionModule.GetPurchases")]
    public async Task GetPurchases()
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var getShopPurchasesRequest = new GetShopPurchasesRequest(ShopId);
        var purchases = await _mediator.Send(getShopPurchasesRequest);

        var fieldsTasks = purchases.Select(async x =>
        {
            var productEmoji = await EmojiParser.GetExistingEmojiAsync(Context.Guild, x.Product.Emoji);
            return new EmbedFieldBuilder()
                .WithName($"{productEmoji} {x.Product.Name}")
                .WithValue($"""
                            {localizer.Buyer}: <@{x.Bill.BuyerDiscordId}>
                            {localizer.Role}: <@&{x.Product.DiscordRoleId}>
                            {localizer.Profit}: **{x.Product.Price} ₽**
                            {localizer.Date}: `{x.DateTime} (UTC)`
                            """)
                .WithIsInline(true);
        });

        var fields = await Task.WhenAll(fieldsTasks);

        var purchasesEmbed = new EmbedBuilder()
            .WithTitle($"📦 {localizer.Purchases}")
            .WithDescription(purchases.Count == 0
                ? localizer.NoPurchases
                : null)
            .WithFields(fields)
            .WithFooter($"AiryPay © {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        await RespondAsync(embed: purchasesEmbed, ephemeral: true);
    }

    [ComponentInteraction("InfoInteractionModule.OpenSettings")]
    public async Task OpenSettings()
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var purchasesEmbed = new EmbedBuilder()
            .WithTitle($"⚙️ {localizer.SettingsButton}")
            .WithDescription($"{localizer.SettingsCurrent}")
            .WithFooter($"AiryPay © {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        var selectMenuBuilder = new SelectMenuBuilder()
            .WithCustomId("InfoInteractionModule.OpenSettings.LanguageSelector")
            .WithType(ComponentType.SelectMenu)
            .WithPlaceholder($"\ud83c\udf10 {localizer.SettingsLanguageDescription}");

        foreach (var lang in _appSettings.BotSupportedLanguages)
        {
            var currentLanguage = new Language(lang.Code);

            localizer = new Localizer(currentLanguage);
            var emote = new Emoji(localizer.LanguageEmoji);

            selectMenuBuilder.AddOption(new SelectMenuOptionBuilder()
                .WithLabel(localizer.LanguageName)
                .WithValue(lang.Code)
                .WithEmote(emote));
        }

        // localizer needs to be reset after iteration is complete so localization after uses shop language
        // localizer = new Localizer(shop.Language);

        var messageComponents = new ComponentBuilder()
            .WithRows([
                new ActionRowBuilder().WithSelectMenu(selectMenuBuilder)])
            .Build();

        await RespondAsync(embed: purchasesEmbed, components: messageComponents, ephemeral: true);
    }

    [ComponentInteraction("InfoInteractionModule.OpenSettings.LanguageSelector")]
    public async Task ChooseNewStoreLanguage(string selectedLanguageString)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        if (!Language.TryParse(selectedLanguageString, out var selectedLanguage))
        {
            await RespondAsync(
                ":no_entry_sign: " + localizer.LanguageNotValid,
                ephemeral: true);
            return;
        }

        var updateShopLanguageRequest = new UpdateShopLanguageRequest(shop.Id, selectedLanguage);

        var operationResult = await _mediator.Send(updateShopLanguageRequest);
        if (operationResult.Failed)
        {
            var localizedMessageCode = operationResult.ErrorType switch
            {
                UpdateShopLanguageRequest.Error.ShopNotFound => "shopNotFound",
                UpdateShopLanguageRequest.Error.LanguageNotSupported => "languageNotValid",
                _ => "unknownError"
            };

            await RespondAsync(
                ":no_entry_sign: " + localizer.GetString(localizedMessageCode),
                ephemeral: true);
            return;
        }

        await RespondAsync(
            ":white_check_mark: " + localizer.SettingsNewLanguageSelected,
            ephemeral: true);
    }
}
