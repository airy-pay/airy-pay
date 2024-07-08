using System.Globalization;
using AiryPayNew.Application.Requests.Payments;
using AiryPayNew.Application.Requests.Products;
using AiryPayNew.Application.Requests.Shops;
using AiryPayNew.Discord.Utils;
using Discord;
using Discord.Interactions;
using MediatR;
using DiscordCommands = Discord.Commands;

namespace AiryPayNew.Discord.InteractionModules;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
public class SetupInteractionModule(IMediator mediator) : InteractionModuleBase
{
    private record PaymentSystem(string DiscordEmoji, string Name, string Description);

    private readonly Dictionary<string, PaymentSystem> _paymentMethods = new()
    {
        { "card", new(":credit_card:", "Карты", "(VISA/Mastercard/MIR)") },
        { "card_kzt", new(":flag_kz:", "Карты Казахстан", "(VISA/Mastercard)") },
        { "card_uzs", new(":flag_uz:", "Карты Узбекистан", "(VISA/Mastercard)") },
        { "card_azn", new(":flag_az:", "Карты Азербайджан", "(VISA/Mastercard)") },
        { "sbp", new("<:sbp:1259761043312349247>", "СБП", "Оплата QR кодом") },
        { "yandexmoney", new("<:yandexmoney:1259761039830945855>", "YooMoney", "yoomoney.ru") },
        { "payeer", new("<:payeer:1259761041685086278>", "Payeer", "payeer.com") },
        { "skinpay", new("<:skinpay:1259761038161612860>", "SkinPay", "Оплата скинами") },
        { "crypta", new("<:crypta:1259761036626493471>", "Криптовалюты", "BTC, ETH, USDT, TON") },
        { "clever", new(":four_leaf_clover:", "Clever Wallet", "klever.io") },
    };
    private readonly Color _embedsColor = new(40, 117, 233);
    
    [DiscordCommands.RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("setup", "\u2728 Установка сообщения для продажи товаров")]
    public async Task Setup()
    {
        var getShopRequest = new GetShopRequest(Context.Guild.Id);
        var operationResult = await mediator.Send(getShopRequest);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
            return;
        }
        if (operationResult.Entity is null)
        {
            await RespondAsync(":no_entry_sign: " + "Магазин не найден.", ephemeral: true);
            return;
        }

        var selectMenuOptionsTasks = operationResult.Entity.Products
            .Select(async x => new SelectMenuOptionBuilder()
                .WithLabel(x.Name)
                .WithDescription($"{x.Price.ToString(CultureInfo.InvariantCulture)} \u20bd")
                .WithEmote(await EmojiParser.GetExistingEmojiAsync(Context.Guild, x.Emoji))
                .WithValue(x.Id.Value.ToString()));
        var selectMenuOptions = await Task.WhenAll(selectMenuOptionsTasks);
        
        var selectMenu = new SelectMenuBuilder()
            .WithCustomId("SetupInteractionModule.ChooseProduct")
            .WithPlaceholder("\ud83d\udecd\ufe0f Выберите товар для покупки")
            .WithOptions(selectMenuOptions.ToList());
        
        var messageComponents = new ComponentBuilder()
            .WithSelectMenu(selectMenu)
            .Build();

        await Context.Channel.SendMessageAsync(" ", components: messageComponents);
        await RespondAsync(":warning: При обновлении товаров магазина это сообщение нужно будет удалить " +
                           "и установить снова при помощи команды `/setup`.",
            ephemeral: true);
    }

    [ComponentInteraction("SetupInteractionModule.ChooseProduct")]
    public async Task ChooseProduct(string selectedProductId)
    {
        var selectMenu = new SelectMenuBuilder()
            .WithCustomId($"SetupInteractionModule.ChoosePaymentMethod:{selectedProductId}")
            .WithPlaceholder("\ud83d\udcb3 Выберите способ оплаты")
            .WithOptions(_paymentMethods.Select(x => new SelectMenuOptionBuilder()
                    .WithLabel(x.Value.Name)
                    .WithDescription(x.Value.Description)
                    .WithEmote(EmojiParser.GetEmoji(x.Value.DiscordEmoji))
                    .WithValue(x.Key))
                .ToList());
        
        var messageComponents = new ComponentBuilder()
            .WithSelectMenu(selectMenu)
            .Build();
        
        await RespondAsync(" ", components: messageComponents, ephemeral: true);
    }

    [ComponentInteraction($"SetupInteractionModule.ChoosePaymentMethod:*")]
    public async Task ChooseProduct(string selectedProductId, string paymentMethodKey)
    {
        var productParseResult = long.TryParse(selectedProductId, out var productId);
        if (!productParseResult)
        {
            await RespondAsync(":no_entry_sign: Выбран некорректный товар.", ephemeral: true);
            return;
        }

        if (!_paymentMethods.ContainsKey(paymentMethodKey))
        {
            await RespondAsync(":no_entry_sign: Выбран некорректный способ оплаты", ephemeral: true);
            return;
        }
        
        var getProductRequest = new GetProductRequest(Context.Guild.Id, productId);
        var getProductOperationResult = await mediator.Send(getProductRequest);
        if (!getProductOperationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + getProductOperationResult.ErrorMessage, ephemeral: true);
            return;
        }
        if (getProductOperationResult.Entity is null)
        {
            await RespondAsync(":no_entry_sign: Тован не найден.", ephemeral: true);
            return;
        }

        var product = getProductOperationResult.Entity;
        
        var createPaymentRequest = new CreatePaymentRequest(
            productId, paymentMethodKey, Context.Interaction.User.Id, Context.Guild.Id);
        var createPaymentOperationResult = await mediator.Send(createPaymentRequest);
        if (!createPaymentOperationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + createPaymentOperationResult.ErrorMessage, ephemeral: true);
            return;
        }

        var paymentMethod = _paymentMethods[paymentMethodKey];
        var payEmbed = new EmbedBuilder()
            .WithTitle($"\ud83d\udcb8 Оплата")
            .WithDescription($"Оплатите [счёт]({createPaymentOperationResult.Entity}) в течение 30 минут.")
            .WithFields([
                new EmbedFieldBuilder()
                    .WithName($"{product.Emoji} {product.Name}")
                    .WithValue($"{product.Price} \u20bd")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"{paymentMethod.DiscordEmoji} {paymentMethod.Name}")
                    .WithValue($"{paymentMethod.Description} ")
                    .WithIsInline(true)])
            .WithFooter($"AiryPay \u00a9 {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();
        
        var payButton = new ButtonBuilder()
            .WithLabel($"Оплатить | {product.Price} \u20bd")
            .WithUrl(createPaymentOperationResult.Entity)
            .WithEmote(new Emoji("\ud83d\udcb3"))
            .WithStyle(ButtonStyle.Link);
        
        var messageComponents = new ComponentBuilder()
            .WithRows(new[]
            {
                new ActionRowBuilder()
                    .WithButton(payButton)
            })
            .Build();
        
        await RespondAsync(
            embed: payEmbed,
            components: messageComponents,
            ephemeral: true);
    }
}