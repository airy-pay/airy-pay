using System.Globalization;
using AiryPayNew.Application.Requests.Payments;
using AiryPayNew.Application.Requests.Products;
using AiryPayNew.Application.Requests.Shops;
using AiryPayNew.Presentation.Services;
using AiryPayNew.Presentation.Utils;
using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Presentation.Utils;
using AiryPayNew.Shared.Settings.AppSettings;
using Discord;
using Discord.Interactions;
using MediatR;

namespace AiryPayNew.Presentation.InteractionModules;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
public class SetupInteractionModule(
    IMediator mediator,
    AppSettings appSettings,
    UserRepositoryService userRepositoryService) : InteractionModuleBase
{
    private readonly Color _embedsColor = new(40, 117, 233);
    
    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("setup", "\u2728 Change message for selling products")]
    public async Task Setup()
    {
        var getShopRequest = new GetShopRequest(Context.Guild.Id);
        var operationResult = await mediator.Send(getShopRequest);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
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
        var operationResult = await GetProductFromIdAsync(selectedProductId);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
            return;
        }

        var affordablePaymentMethods = appSettings.PaymentSettings.PaymentMethods
            .Where(x => operationResult.Entity.Price >= x.MinimalSum);
        
        var selectMenu = new SelectMenuBuilder()
            .WithCustomId($"SetupInteractionModule.ChoosePaymentMethod:{selectedProductId}")
            .WithPlaceholder("\ud83d\udcb3 Выберите способ оплаты")
            .WithOptions(affordablePaymentMethods.Select(x => new SelectMenuOptionBuilder()
                    .WithLabel(x.Name)
                    .WithDescription(x.Description)
                    .WithEmote(EmojiParser.GetEmoji(x.DiscordEmoji))
                    .WithValue(x.MethodId))
                .ToList());
        
        var messageComponents = new ComponentBuilder()
            .WithSelectMenu(selectMenu)
            .Build();
        
        await RespondAsync(" ", components: messageComponents, ephemeral: true);
    }

    [ComponentInteraction("SetupInteractionModule.ChoosePaymentMethod:*")]
    public async Task ChooseProduct(string selectedProductId, string paymentMethodKey)
    {
        var paymentMethod = appSettings.PaymentSettings.PaymentMethods
            .FirstOrDefault(x => x.MethodId == paymentMethodKey);
        if (paymentMethod is null)
        {
            await RespondAsync(":no_entry_sign: Выбран некорректный способ оплаты", ephemeral: true);
            return;
        }

        var operationResult = await GetProductFromIdAsync(selectedProductId);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
            return;
        }

        var product = operationResult.Entity;
        
        var createPaymentRequest = new CreatePaymentRequest(
            operationResult.Entity.Id,
            paymentMethod.ServiceName,
            paymentMethod.MethodId,
            Context.Interaction.User.Id,
            Context.Guild.Id);
        var createPaymentOperationResult = await mediator.Send(createPaymentRequest);
        if (!createPaymentOperationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + createPaymentOperationResult.ErrorMessage, ephemeral: true);
            return;
        }
        
        userRepositoryService.SetUser(Context.Interaction.User);
        
        var payEmbed = new EmbedBuilder()
            .WithTitle($"\ud83d\udcb8 Оплата")
            .WithDescription($"Оплатите [счёт]({createPaymentOperationResult.Entity}) в течение 10 минут.")
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

    private async Task<OperationResult<Product>> GetProductFromIdAsync(string id)
    {
        var productParseResult = long.TryParse(id, out var productId);
        if (!productParseResult)
        {
            return OperationResult<Product>.Error(new Product(), "Выбран некорректный товар.");
        }
        
        var getProductRequest = new GetProductRequest(Context.Guild.Id, productId);
        var getProductOperationResult = await mediator.Send(getProductRequest);
        if (!getProductOperationResult.Successful)
        {
            return OperationResult<Product>.Error(new Product(), getProductOperationResult.ErrorMessage);
        }
        if (getProductOperationResult.Entity is null)
        {
            return OperationResult<Product>.Error(new Product(), "Тован не найден.");
        }

        return OperationResult<Product>.Success(getProductOperationResult.Entity);
    }
}