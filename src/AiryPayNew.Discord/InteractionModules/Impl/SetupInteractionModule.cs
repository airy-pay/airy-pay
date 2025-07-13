using System.Globalization;
using AiryPayNew.Application.Requests.Payments;
using AiryPayNew.Application.Requests.Products;
using AiryPayNew.Discord.Localization;
using AiryPayNew.Discord.Utils;
using AiryPayNew.Domain.Common;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Shared.Settings.AppSettings;
using Discord;
using Discord.Interactions;
using MediatR;

namespace AiryPayNew.Discord.InteractionModules.Impl;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
public class SetupInteractionModule : ShopInteractionModuleBase
{
    private readonly IMediator _mediator;
    private readonly AppSettings _appSettings;
    private readonly Color _embedsColor;

    public SetupInteractionModule(
        IMediator mediator,
        AppSettings appSettings) : base(mediator)
    {
        _appSettings = appSettings;
        _mediator = mediator;
        
        _embedsColor = ColorMapper.Map(appSettings.Discord.EmbedMessageColor);
    }

    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("setup", "✨ Change message for selling products")]
    public async Task Setup()
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var selectMenuOptionsTasks = shop.Products
            .Select(async x => new SelectMenuOptionBuilder()
                .WithLabel(x.Name)
                .WithDescription($"{x.Price.ToString(CultureInfo.InvariantCulture)} ₽")
                .WithEmote(await EmojiParser.GetExistingEmojiAsync(Context.Guild, x.Emoji))
                .WithValue(x.Id.Value.ToString()));
        var selectMenuOptions = await Task.WhenAll(selectMenuOptionsTasks);

        if (selectMenuOptions.Length == 0)
        {
            await RespondAsync(
                localizer.GetString("setup.createProductsFirst"), ephemeral: true);
            return;
        }
        
        var selectMenu = new SelectMenuBuilder()
            .WithCustomId("SetupInteractionModule.ChooseProduct")
            .WithPlaceholder(localizer.GetString("setup.selectProduct.placeholder"))
            .WithOptions(selectMenuOptions.ToList());

        var messageComponents = new ComponentBuilder()
            .WithSelectMenu(selectMenu)
            .Build();

        await Context.Channel.SendMessageAsync(" ", components: messageComponents);

        await RespondAsync(localizer.GetString("setup.selectProduct.warning"), ephemeral: true);
    }

    [ComponentInteraction("SetupInteractionModule.ChooseProduct")]
    public async Task ChooseProduct(string selectedProductId)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var operationResult = await GetProductFromIdAsync(selectedProductId);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + localizer.GetString("setup.invalidProduct"),
                ephemeral: true);
            return;
        }

        var affordablePaymentMethods = _appSettings.PaymentSettings.PaymentMethods
            .Where(x => operationResult.Entity.Price >= x.MinimalSum);

        var selectMenu = new SelectMenuBuilder()
            .WithCustomId($"SetupInteractionModule.ChoosePaymentMethod:{selectedProductId}")
            .WithPlaceholder(localizer.GetString("setup.selectPayment.placeholder"))
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
    public async Task ChoosePaymentMethod(string selectedProductId, string paymentMethodKey)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var paymentMethod = _appSettings.PaymentSettings.PaymentMethods
            .FirstOrDefault(x => x.MethodId == paymentMethodKey);
        if (paymentMethod is null)
        {
            await RespondAsync(":no_entry_sign: " + localizer.GetString("setup.invalidPaymentMethod"),
                ephemeral: true);
            return;
        }

        var operationResult = await GetProductFromIdAsync(selectedProductId);
        if (!operationResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + localizer.GetString("setup.invalidProduct"),
                ephemeral: true);
            return;
        }

        var product = operationResult.Entity;

        var createPaymentRequest = new CreatePaymentRequest(
            product.Id,
            paymentMethod.ServiceName,
            paymentMethod.MethodId,
            Context.Interaction.User.Id,
            Context.Guild.Id);

        var createPaymentOperationResult = await _mediator.Send(createPaymentRequest);
        if (!createPaymentOperationResult.Successful)
        {
            var error = string.Format(
                localizer.GetString("setup.paymentError"),
                createPaymentOperationResult.ErrorMessage);
            await RespondAsync(":no_entry_sign: " + error, ephemeral: true);
            return;
        }

        var payEmbed = new EmbedBuilder()
            .WithTitle(localizer.GetString("setup.paymentEmbed.title"))
            .WithDescription(
                string.Format(
                    localizer.GetString("setup.paymentEmbed.description"),
                    createPaymentOperationResult.Entity))
            .WithFields([
                new EmbedFieldBuilder()
                    .WithName($"{product.Emoji} {product.Name}")
                    .WithValue($"{product.Price} ₽")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName($"{paymentMethod.DiscordEmoji} {paymentMethod.Name}")
                    .WithValue($"{paymentMethod.Description} ")
                    .WithIsInline(true)])
            .WithFooter(
                string.Format(
                        localizer.GetString("setup.paymentEmbed.footer"),
                        DateTime.UtcNow.Year),
                    Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        var payButton = new ButtonBuilder()
            .WithLabel(
                string.Format(
                    localizer.GetString("setup.paymentButton.label"),
                    product.Price))
            .WithUrl(createPaymentOperationResult.Entity)
            .WithEmote(new Emoji("💳"))
            .WithStyle(ButtonStyle.Link);

        var messageComponents = new ComponentBuilder()
            .WithRows([new ActionRowBuilder().WithButton(payButton)])
            .Build();

        await RespondAsync(embed: payEmbed, components: messageComponents, ephemeral: true);
    }

    private async Task<OperationResult<Product>> GetProductFromIdAsync(string id)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        if (!long.TryParse(id, out var productId))
        {
            return OperationResult<Product>.Error(
                new Product(),
                localizer.GetString("setup.invalidProduct"));
        }

        var getProductRequest = new GetProductRequest(Context.Guild.Id, productId);
        var getProductOperationResult = await _mediator.Send(getProductRequest);
        if (!getProductOperationResult.Successful || getProductOperationResult.Entity is null)
        {
            return OperationResult<Product>.Error(
                new Product(),
                localizer.GetString("setup.invalidProduct"));
        }

        return OperationResult<Product>.Success(getProductOperationResult.Entity);
    }
}
