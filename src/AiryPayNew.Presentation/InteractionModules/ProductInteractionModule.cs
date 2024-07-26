using AiryPayNew.Application.Requests.Products;
using AiryPayNew.Presentation.AutocompleteHandlers;
using AiryPayNew.Domain.Entities.Products;
using AiryPayNew.Presentation.Utils;
using Discord;
using Discord.Interactions;
using MediatR;
using Sqids;

namespace AiryPayNew.Presentation.InteractionModules;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[RequireUserPermission(GuildPermission.Administrator)]
[Group("product", "\ud83d\udecd\ufe0f Работа с товарами")]
public class ProductInteractionModule(
    IMediator mediator,
    SqidsEncoder<long> sqidsEncoder) : InteractionModuleBase<SocketInteractionContext>
{
    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("create", "\ud83d\udd27 Создание нового товара")]
    public async Task Create(
        [Summary("Эмодзи", "Будет отображаться возле товара")] string emojiText,
        [Summary("Название", "Название товара")] string name,
        [Summary("Цена", "Цена товара")] decimal price,
        [Summary("Роль", "Роль, которая будет выдана покупателю товара")] IRole discordRole)
    {
        var validEmojiText = await EmojiParser.GetEmojiText(emojiText);
        if (validEmojiText is null)
        {
            await RespondAsync(":no_entry_sign: Используйте допустимый emoji", ephemeral: true);
            return;
        }

        bool giveHighRoleWarning = true;
        var botUser = Context.Guild.GetUser(Context.Client.CurrentUser.Id);
        var botMaxPositionRole = botUser.Roles.MaxBy(x => x.Position);
        if (botMaxPositionRole is not null)
        {
            giveHighRoleWarning = botMaxPositionRole.Position <= discordRole.Position;
        }

        var botRoleName = botMaxPositionRole is null ? "AiryPay" : $"<@&{botMaxPositionRole.Id}>";
        var responseMessage = ":white_check_mark: Новый товар был создан."
            + (giveHighRoleWarning
            ? $"\n\n:warning: Роль бота ({botRoleName}) находится ниже роли товара (<@&{discordRole.Id}>).\n" +
              "Измените позицию роли в настройках сервера, иначе бот не сможет автоматически выдавать её."
            : "");
        
        var createProductRequest = new CreateProductRequest(
            Context.Guild.Id,
            new ProductModel(validEmojiText, name, price, discordRole.Id));
        var operationResult = await mediator.Send(createProductRequest);
        if (operationResult.Successful)
        {
            await RespondAsync(responseMessage, ephemeral: true);
            return;
        }
        
        await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
    }
    
    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("delete", "\ud83d\udeab Удаление товара")]
    public async Task Delete(
        [Summary("Товар", "Товар, который будет удалён"),
         Autocomplete(typeof(ProductAutocompleteHandler))] string productHashId)
    {
        var productId = new ProductId(sqidsEncoder.Decode(productHashId).Single());
        
        var removeProductRequest = new RemoveProductRequest(Context.Guild.Id, productId);
        await mediator.Send(removeProductRequest);
        
        await RespondAsync(":wastebasket: Товар был удалён.", ephemeral: true);
    }
    
    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("edit", "\ud83d\udd04 Изменение товара")]
    public async Task Edit(
        [Summary("Товар", "Товар, который будет изменён"),
         Autocomplete(typeof(ProductAutocompleteHandler))] string productHashId,
        [Summary("Эмодзи", "Будет отображаться возле товара")] string emojiText,
        [Summary("Название", "Название товара")] string name,
        [Summary("Цена", "Цена товара")] decimal price,
        [Summary("Роль", "Роль, которая будет выдана покупателю товара")] IRole discordRole)
    {
        var validEmojiText = await EmojiParser.GetEmojiText(emojiText);
        if (validEmojiText is null)
        {
            await RespondAsync(":no_entry_sign: Используйте допустимый emoji", ephemeral: true);
            return;
        }
        
        var productId = new ProductId(sqidsEncoder.Decode(productHashId).Single());

        var editProductRequest = new EditProductRequest(
            Context.Guild.Id,
            productId,
            new ProductModel(validEmojiText, name, price, discordRole.Id));
        var operationResult = await mediator.Send(editProductRequest);
        if (operationResult.Successful)
        {
            await RespondAsync(":recycle: Товар был изменён.", ephemeral: true);
            return;
        }
        
        await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
    }
}