using System.Globalization;
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
    private record PaymentSystem(string DiscordEmoji, string Name, string? Description);

    private readonly Dictionary<string, PaymentSystem> _paymentSystems = new()
    {
        { "card", new(":credit_card:", "Карты", "(VISA/Mastercard/MIR)") },
        { "card_kzt", new(":flag_kz:", "Карты Казахстан", "(VISA/Mastercard)") },
        { "card_uzs", new(":flag_uz:", "Карты Узбекистан", "(VISA/Mastercard)") },
        { "card_azn", new(":flag_az:", "Карты Азербайджан", "(VISA/Mastercard)") },
        { "sbp", new("<:sbp:1259761043312349247>", "СБП", null) },
        { "yandexmoney", new("<:yandexmoney:1259761039830945855>", "YooMoney", null) },
        { "payeer", new("<:payeer:1259761041685086278>", "Payeer", null) },
        { "skinpay", new("<:skinpay:1259761038161612860>", "SkinPay", "Оплата скинами") },
        { "crypta", new("<:crypta:1259761036626493471>", "Криптовалюты", null) },
        { "clever", new(":four_leaf_clover:", "Clever Wallet", null) },
    };
    
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
        
        var selectMenu = new SelectMenuBuilder()
            .WithCustomId("SetupInteractionModule.ChooseProduct")
            .WithPlaceholder("\ud83d\udecd\ufe0f Выберите товар для покупки")
            .WithOptions(operationResult.Entity.Products
                .Select(x => new SelectMenuOptionBuilder()
                    .WithLabel(x.Name)
                    .WithDescription(x.Price.ToString(CultureInfo.InvariantCulture))
                    .WithEmote(EmojiParser.GetEmoji(x.Emoji))
                    .WithValue(x.Id.Value.ToString()))
                .ToList());
        
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
        var productParseResult = long.TryParse(selectedProductId, out var productId);
        if (!productParseResult)
        {
            await RespondAsync(":no_entry_sign: Некорректный товар", ephemeral: true);
            return;
        }

        var getProductRequest = new GetProductRequest(Context.Guild.Id, productId);
        var operationResult = await mediator.Send(getProductRequest);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
            return;
        }
        if (operationResult.Entity is null)
        {
            await RespondAsync(":no_entry_sign: Некорректный товар.", ephemeral: true);
            return;
        }
        
        var selectMenu = new SelectMenuBuilder()
            .WithCustomId("SetupInteractionModule.ChooseProduct")
            .WithPlaceholder("\ud83e\ude99 Выберите способ оплаты")
            .WithOptions(_paymentSystems.Select(x => new SelectMenuOptionBuilder()
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
}