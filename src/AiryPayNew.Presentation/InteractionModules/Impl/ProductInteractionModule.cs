using AiryPayNew.Application.Requests.Products;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Presentation.AutocompleteHandlers;
using AiryPayNew.Presentation.Utils;
using Discord;
using Discord.Interactions;
using MediatR;
using Sqids;

namespace AiryPayNew.Presentation.InteractionModules.Impl;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[RequireUserPermission(GuildPermission.Administrator)]
[Group("product", "\ud83d\udecd\ufe0f Working with products")]
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

    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("create", "\ud83d\udd27 Create new product")]
    public async Task Create(
        [Summary("Emoji", "Will be displayed next to the product")] string emojiText,
        [Summary("Name", "The name of the product")] string name,
        [Summary("Price", "The price of the product")] decimal price,
        [Summary("Role", "The role that will be granted to the buyer of the product")] IRole discordRole)

    {
        var validEmojiText = await EmojiParser.GetEmojiText(emojiText);
        if (validEmojiText is null)
        {
            await RespondAsync(":no_entry_sign: Используйте допустимый emoji", ephemeral: true);
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
        var responseMessage = ":white_check_mark: Новый товар был создан."
            + (giveHigherRoleWarning
            ? $"\n\n:warning: Роль бота ({botRoleName}) находится ниже роли товара (<@&{discordRole.Id}>).\n" +
              "Измените позицию роли в настройках сервера, иначе бот не сможет автоматически выдавать её."
            : "");
        
        var createProductRequest = new CreateProductRequest(
            Context.Guild.Id,
            new ProductModel(validEmojiText, name, price, discordRole.Id));
        var operationResult = await _mediator.Send(createProductRequest);
        if (operationResult.Successful)
        {
            await RespondAsync(responseMessage, ephemeral: true);
            return;
        }
        
        await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
    }
    
    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("delete", "\ud83d\udeab Delete product")]
    public async Task Delete(
        [Summary("Product", "The product that will be deleted"),
         Autocomplete(typeof(ProductAutocompleteHandler))] string productHashId)

    {
        var productId = new ProductId(_sqidsEncoder.Decode(productHashId).Single());
        
        var removeProductRequest = new RemoveProductRequest(Context.Guild.Id, productId);
        await _mediator.Send(removeProductRequest);
        
        await RespondAsync(":wastebasket: Товар был удалён.", ephemeral: true);
    }
    
    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("edit", "\ud83d\udd04 Change product")]
    public async Task Edit(
        [Summary("Product", "The product that will be edited"),
         Autocomplete(typeof(ProductAutocompleteHandler))] string productHashId,
        [Summary("Emoji", "Will be displayed next to the product")] string emojiText,
        [Summary("Name", "The name of the product")] string name,
        [Summary("Price", "The price of the product")] decimal price,
        [Summary("Role", "The role that will be granted to the buyer of the product")] IRole discordRole)

    {
        var validEmojiText = await EmojiParser.GetEmojiText(emojiText);
        if (validEmojiText is null)
        {
            await RespondAsync(":no_entry_sign: Используйте допустимый emoji", ephemeral: true);
            return;
        }
        
        var productId = new ProductId(_sqidsEncoder.Decode(productHashId).Single());

        var editProductRequest = new EditProductRequest(
            Context.Guild.Id,
            productId,
            new ProductModel(validEmojiText, name, price, discordRole.Id));
        var operationResult = await _mediator.Send(editProductRequest);
        if (operationResult.Successful)
        {
            await RespondAsync(":recycle: Товар был изменён.", ephemeral: true);
            return;
        }
        
        await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
    }
}