using System.Globalization;
using AiryPay.Application.Requests.Payments;
using AiryPay.Application.Requests.Products;
using AiryPay.Application.Requests.ShopComplaints;
using AiryPay.Discord.Localization;
using AiryPay.Discord.Modals;
using AiryPay.Discord.Utils;
using AiryPay.Domain.Entities.Products;
using AiryPay.Shared.Settings;
using Discord;
using Discord.Interactions;
using MediatR;

namespace AiryPay.Discord.InteractionModules.Impl;

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

        var openComplaintModalButton = new ButtonBuilder()
            .WithCustomId("SetupInteractionModule.OpenComplaintModal")
            .WithLabel(localizer.GetString("complaint"))
            .WithEmote(new Emoji("\ud83d\udea8"))
            .WithStyle(ButtonStyle.Secondary);
        
        var supportButton = new ButtonBuilder()
            .WithLabel(localizer.GetString("support"))
            .WithUrl("https://discord.gg/Arn9RsRqD9") // TODO: Move to config
            .WithEmote(new Emoji("💬"))
            .WithStyle(ButtonStyle.Link);

        var termsButton = new ButtonBuilder()
            .WithUrl("https://airypay.ru/terms") // TODO: Move to config
            .WithLabel(localizer.GetString("terms"))
            .WithEmote(new Emoji("📃"))
            .WithStyle(ButtonStyle.Link);

        var messageComponents = new ComponentBuilder()
            .WithRows([
                new ActionRowBuilder()
                    .WithSelectMenu(selectMenu),
                new ActionRowBuilder()
                    .WithButton(supportButton)
                    .WithButton(termsButton)
                    .WithButton(openComplaintModalButton)
            ])
            .Build();
        
        await Context.Channel.SendMessageAsync(" ", components: messageComponents);

        await RespondAsync(localizer.GetString("setup.selectProduct.warning"), ephemeral: true);
    }

    [ComponentInteraction("SetupInteractionModule.ChooseProduct")]
    public async Task ChooseProduct(string selectedProductId)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var product = await GetProductFromIdAsync(selectedProductId);
        if (product is null)
        {
            await RespondAsync(
                ":no_entry_sign: " + localizer.GetString("setup.invalidProduct"),
                ephemeral: true);
            return;
        }

        var affordablePaymentMethods = _appSettings.PaymentSettings.PaymentMethods
            .Where(x => product.Price >= x.MinimalSum);

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

        var product = await GetProductFromIdAsync(selectedProductId);
        if (product is null)
        {
            await RespondAsync(":no_entry_sign: " + localizer.GetString("setup.invalidProduct"),
                ephemeral: true);
            return;
        }

        var createPaymentRequest = new CreatePaymentRequest(
            product.Id,
            paymentMethod.ServiceName,
            paymentMethod.MethodId,
            Context.Interaction.User.Id,
            Context.Guild.Id);

        var createPaymentOperationResult = await _mediator.Send(createPaymentRequest);
        if (!createPaymentOperationResult.Successful)
        {
            var localizedMessageCode = createPaymentOperationResult.ErrorType switch
            {
                CreatePaymentRequest.Error.ShopNotFound => "shopNotFound",
                CreatePaymentRequest.Error.ShopIsBlocked => "shopIsBlocked",
                CreatePaymentRequest.Error.PaymentServiceNotFound => "paymentServiceNotFound",
                CreatePaymentRequest.Error.ProductNotFound => "productNotFound",
                CreatePaymentRequest.Error.AccessDenied => "accessDenied",
                CreatePaymentRequest.Error.FailedToCreate => "failedToCreate",
                _ => "validationFailed",
            };
            
            var error = string.Format(
                localizer.GetString("setup.paymentError"),
                localizer.GetString(localizedMessageCode));
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
    
    [ComponentInteraction("SetupInteractionModule.OpenComplaintModal")]
    public async Task OpenComplaintModal()
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);
        
        await Context.Interaction.RespondWithModalAsync(
            "SetupInteractionModule.ComplaintModal",
            new ComplaintModal(
                "\ud83d\udea8 " + localizer.GetString("complaintRegistered"),
                localizer.GetString("complaintReason"), 
                localizer.GetString("complaintDetails")));
    }

    [ModalInteraction("SetupInteractionModule.ComplaintModal")]
    public async Task UpdateBanner(ComplaintModal complaintModal)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);
    
        var createShopComplaintRequest = new CreateShopComplaintRequest(
            shop.Id, Context.Interaction.User.Id, complaintModal.Reason, complaintModal.Details);
    
        var result = await _mediator.Send(createShopComplaintRequest);
        if (!result.Successful)
        {
            var localizedMessageCode = result.ErrorType switch
            {
                CreateShopComplaintRequest.Error.NoReasonAndDetails => "noReasonAndDetails",
                CreateShopComplaintRequest.Error.TooManyComplaints => "tooManyComplaints",
                _ => "validationFailed",
            };
            
            await RespondAsync(
                ":no_entry_sign: " + localizer.GetString(localizedMessageCode),
                ephemeral: true);
            return;
        }
        
        await RespondAsync(
            ":white_check_mark: " + localizer.GetString("complaintRegistered"),
            ephemeral: true);
    }
    
    private async Task<Product?> GetProductFromIdAsync(string id)
    {
        if (!long.TryParse(id, out var productId))
        {
            return null;
        }

        var getProductRequest = new GetProductRequest(ShopId, productId);
        var getProductOperationResult = await _mediator.Send(getProductRequest);
        if (getProductOperationResult.Failed)
        {
            return null;
        }

        return getProductOperationResult.Entity;
    }
}
