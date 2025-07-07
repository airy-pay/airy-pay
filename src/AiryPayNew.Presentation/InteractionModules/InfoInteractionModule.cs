using AiryPayNew.Application.Common;
using AiryPayNew.Application.Requests.Shops;
using AiryPayNew.Presentation.Utils;
using AiryPayNew.Domain.Entities.Withdrawals;
using AiryPayNew.Presentation.Localization;
using Discord;
using Discord.Interactions;
using MediatR;

namespace AiryPayNew.Presentation.InteractionModules;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[RequireUserPermission(GuildPermission.Administrator)]
public class InfoInteractionModule(
    IMediator mediator,
    IShopLanguageService shopLanguageService) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly Color _embedsColor = new(40, 117, 233);

    [SlashCommand("info", "🌐 Shop information")]
    public async Task Info()
    {
        var getShopRequest = new GetShopRequest(Context.Guild.Id);
        var operationResult = await mediator.Send(getShopRequest);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
            return;
        }

        var localizer = new Localizer(operationResult.Entity.Language);

        var re = localizer.GetString("status");
        
        var shopInfoEmbed = new EmbedBuilder()
            .WithTitle($"🌐 {localizer.GetString("shopInformation")}")
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName($"💰 {localizer.GetString("balance")}")
                    .WithValue($"{operationResult.Entity.Balance} ₽")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"🔄 {localizer.GetString("status")}")
                    .WithValue(operationResult.Entity.Blocked
                        ? localizer.GetString("blocked")
                        : localizer.GetString("active"))
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"🛍️ {localizer.GetString("products")}")
                    .WithValue(operationResult.Entity.Products.Count)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"🏷️ {localizer.GetString("shopId")}")
                    .WithValue($"`{Context.Guild.Id}`")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"{localizer.GetString("_languageEmoji")} {localizer.GetString("language")}")
                    .WithValue(localizer.GetString("_languageName"))
                    .WithIsInline(true))
            .WithFooter($"AiryPay © {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        var productsButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.GetProducts")
            .WithLabel(localizer.GetString("products"))
            .WithEmote(new Emoji("🛍️"))
            .WithStyle(ButtonStyle.Primary);

        var withdrawalsButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.GetWithdrawals")
            .WithLabel(localizer.GetString("withdrawals"))
            .WithEmote(new Emoji("💸"))
            .WithStyle(ButtonStyle.Primary);

        var lastPurchasesButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.GetPurchases")
            .WithLabel(localizer.GetString("purchases"))
            .WithEmote(new Emoji("📦"))
            .WithStyle(ButtonStyle.Primary);

        var supportButton = new ButtonBuilder()
            .WithLabel(localizer.GetString("support"))
            .WithUrl("https://discord.gg/Arn9RsRqD9")
            .WithEmote(new Emoji("💬"))
            .WithStyle(ButtonStyle.Link);

        var termsButton = new ButtonBuilder()
            .WithUrl("https://airypay.ru/terms")
            .WithLabel(localizer.GetString("terms"))
            .WithEmote(new Emoji("📃"))
            .WithStyle(ButtonStyle.Link);

        var messageComponents = new ComponentBuilder()
            .WithRows([
                new ActionRowBuilder()
                    .WithButton(productsButton)
                    .WithButton(withdrawalsButton)
                    .WithButton(lastPurchasesButton),
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
        var getShopRequest = new GetShopRequest(Context.Guild.Id);
        var operationResult = await mediator.Send(getShopRequest);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
            return;
        }

        var localizer = new Localizer(operationResult.Entity.Language);

        var fieldsTasks = operationResult.Entity.Products.Select(async x =>
        {
            var emoji = await EmojiParser.GetExistingEmojiAsync(Context.Guild, x.Emoji);
            return new EmbedFieldBuilder()
                .WithName($"{emoji} {x.Name}")
                .WithValue($"""
                            {localizer.GetString("amount")}: **{x.Price} ₽**
                            {localizer.GetString("role")}: <@&{x.DiscordRoleId}>
                            """)
                .WithIsInline(true);
        });

        var fields = await Task.WhenAll(fieldsTasks);

        var productsEmbed = new EmbedBuilder()
            .WithTitle($"📦 {localizer.GetString("products")}")
            .WithDescription(operationResult.Entity.Products.Count == 0
                ? localizer.GetString("noProducts")
                : null)
            .WithFields(fields)
            .WithFooter($"AiryPay © {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        await RespondAsync(embed: productsEmbed, ephemeral: true);
    }

    [ComponentInteraction("InfoInteractionModule.GetWithdrawals")]
    public async Task GetWithdrawals()
    {
        var getShopRequest = new GetShopRequest(Context.Guild.Id);
        var operationResult = await mediator.Send(getShopRequest);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
            return;
        }
        
        var localizer = new Localizer(operationResult.Entity.Language);
        
        var getShopWithdrawalsRequest = new GetShopWithdrawalsRequest(Context.Guild.Id);
        var withdrawals = await mediator.Send(getShopWithdrawalsRequest);

        var withdrawalStatusesGetter = new Dictionary<WithdrawalStatus, string>()
        {
            { WithdrawalStatus.InProcess, $"🟡 {localizer.GetString("inProcess")}" },
            { WithdrawalStatus.Paid, $"🟢 {localizer.GetString("paid")}" },
            { WithdrawalStatus.Canceled, $"🔴 {localizer.GetString("canceled")}" },
        };

        var withdrawalsEmbed = new EmbedBuilder()
            .WithTitle($"💸 {localizer.GetString("withdrawals")}")
            .WithDescription(withdrawals.Count == 0
                ? localizer.GetString("noWithdrawals")
                : null)
            .WithFields(withdrawals.Select(x => new EmbedFieldBuilder()
                .WithName($"💳 {x.DateTime:dd/MM/yyyy H:mm} Card")
                .WithValue($"""
                            {localizer.GetString("cardNumber")}: ||{CardFormatter.Format(x.ReceivingAccountNumber)}||
                            {localizer.GetString("amount")}: **{x.Amount} ₽**
                            {localizer.GetString("statusField")}: **{withdrawalStatusesGetter[x.WithdrawalStatus]}**
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
        var getShopRequest = new GetShopRequest(Context.Guild.Id);
        var operationResult = await mediator.Send(getShopRequest);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
            return;
        }
        
        var localizer = new Localizer(operationResult.Entity.Language);
        
        var getShopPurchasesRequest = new GetShopPurchasesRequest(Context.Guild.Id);
        var purchases = await mediator.Send(getShopPurchasesRequest);

        var fieldsTasks = purchases.Select(async x =>
        {
            var productEmoji = await EmojiParser.GetExistingEmojiAsync(Context.Guild, x.Product.Emoji);
            return new EmbedFieldBuilder()
                .WithName($"{productEmoji} {x.Product.Name}")
                .WithValue($"""
                            {localizer.GetString("buyer")}: <@{x.Bill.BuyerDiscordId}>
                            {localizer.GetString("role")}: <@&{x.Product.DiscordRoleId}>
                            {localizer.GetString("profit")}: **{x.Product.Price} ₽**
                            {localizer.GetString("date")}: `{x.DateTime} (UTC)`
                            """)
                .WithIsInline(true);
        });

        var fields = await Task.WhenAll(fieldsTasks);

        var purchasesEmbed = new EmbedBuilder()
            .WithTitle($"📦 {localizer.GetString("purchases")}")
            .WithDescription(purchases.Count == 0
                ? localizer.GetString("noPurchases")
                : null)
            .WithFields(fields)
            .WithFooter($"AiryPay © {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        await RespondAsync(embed: purchasesEmbed, ephemeral: true);
    }
}
