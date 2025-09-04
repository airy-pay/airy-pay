using AiryPay.Application.Requests.Products;
using AiryPay.Discord.AutocompleteHandlers;
using AiryPay.Discord.Localization;
using AiryPay.Discord.Utils;
using AiryPay.Domain.Entities.Products;
using AiryPay.Domain.Entities.Shops;
using Discord;
using Discord.Interactions;
using MediatR;
using Sqids;

namespace AiryPay.Discord.InteractionModules.Impl;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[RequireUserPermission(GuildPermission.Administrator)]
[Group("product", "🛍️ Working with products")]
public class ProductInteractionModule : ShopInteractionModuleBase
{
    private readonly IMediator _mediator;
    private readonly SqidsEncoder<long> _sqidsEncoder;

    public ProductInteractionModule(
        IMediator mediator,
        SqidsEncoder<long> sqidsEncoder) : base(mediator)
    {
        _sqidsEncoder = sqidsEncoder;
        _mediator = mediator;
    }

    [SlashCommand("create", "🔧 Create new product")]
    public async Task Create(
        [Summary("Emoji", "Will be displayed next to the product")] string emojiText,
        [Summary("Name", "The name of the product")] string name,
        [Summary("Price", "The price of the product")] decimal price,
        [Summary("Role", "The role that will be granted to the buyer")] IRole discordRole)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var validEmojiText = await EmojiParser.GetEmojiText(emojiText);
        if (validEmojiText is null)
        {
            await RespondAsync(
                $":no_entry_sign: {localizer.GetString("invalidEmoji")}",
                ephemeral: true);
            return;
        }

        bool giveHigherRoleWarning = true;
        var botUser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
        var botMaxPositionRole = botUser.Roles.MaxBy(x => x.Position);
        if (botMaxPositionRole is not null)
        {
            giveHigherRoleWarning = botMaxPositionRole.Position <= discordRole.Position;
        }

        var botRoleName = botMaxPositionRole is null ? "AiryPay" : $"<@&{botMaxPositionRole.Id}>";
        var responseMessage = ":white_check_mark: " + localizer.GetString("productCreated");

        if (giveHigherRoleWarning)
        {
            responseMessage += "\n\n:warning: " + string.Format(
                localizer.GetString("botRoleTooLow"),
                botRoleName,
                $"<@&{discordRole.Id}>");
        }

        var createProductRequest = new CreateProductRequest(
            ShopId,
            new ProductModel(validEmojiText, name, price, discordRole.Id));
        var operationResult = await _mediator.Send(createProductRequest);
        if (operationResult.Successful)
        {
            await RespondAsync(responseMessage, ephemeral: true);
            return;
        }

        var localizedMessageCode = operationResult.ErrorType switch
        {
            CreateProductRequest.Error.ValidationFailed => "validationFailed",
            CreateProductRequest.Error.ShopNotFound => "shopNotFound",
            CreateProductRequest.Error.TooManyProductsCreated => "tooManyProductsCreated",
            CreateProductRequest.Error.ShopIsBlocked => "shopIsBlocked",
            _ => "validationFailed",
        };
        
        await RespondAsync(
            ":no_entry_sign: " + localizer.GetString(localizedMessageCode),
            ephemeral: true);
    }

    [SlashCommand("delete", "🚫 Delete product")]
    public async Task Delete(
        [Summary("Product", "The product that will be deleted"),
         Autocomplete(typeof(ProductAutocompleteHandler))] string productHashId)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var productId = new ProductId(_sqidsEncoder.Decode(productHashId).Single());

        var removeProductRequest = new RemoveProductRequest(
            new ShopId(Context.Guild.Id),
            productId);
        await _mediator.Send(removeProductRequest);

        await RespondAsync(
            $":wastebasket: {localizer.GetString("productDeleted")}",
            ephemeral: true);
    }

    [SlashCommand("edit", "🔄 Change product")]
    public async Task Edit(
        [Summary("Product", "The product that will be edited"),
         Autocomplete(typeof(ProductAutocompleteHandler))] string productHashId,
        [Summary("Emoji", "Will be displayed next to the product")] string emojiText,
        [Summary("Name", "The name of the product")] string name,
        [Summary("Price", "The price of the product")] decimal price,
        [Summary("Role", "The role that will be granted to the buyer")] IRole discordRole)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        var validEmojiText = await EmojiParser.GetEmojiText(emojiText);
        if (validEmojiText is null)
        {
            await RespondAsync(
                $":no_entry_sign: {localizer.GetString("invalidEmoji")}",
                ephemeral: true);
            return;
        }

        var productId = new ProductId(_sqidsEncoder.Decode(productHashId).Single());

        var editProductRequest = new EditProductRequest(
            ShopId,
            productId,
            new ProductModel(validEmojiText, name, price, discordRole.Id));
        var operationResult = await _mediator.Send(editProductRequest);
        if (operationResult.Successful)
        {
            await RespondAsync(
                $":recycle: {localizer.GetString("productEdited")}",
                ephemeral: true);
            return;
        }

        var localizedMessageCode = operationResult.ErrorType switch
        {
            EditProductRequest.Error.ValidationFailed => "validationFailed",
            EditProductRequest.Error.ShopNotFound => "shopNotFound",
            EditProductRequest.Error.ShopIsBlocked => "shopIsBlocked",
            EditProductRequest.Error.ProductNotFound => "productNotFound",
            EditProductRequest.Error.InvalidShopId => "invalidShopId",
            _ => "validationFailed",
        };
        
        await RespondAsync(
            ":no_entry_sign: " + localizer.GetString(localizedMessageCode), ephemeral: true);
    }
}
