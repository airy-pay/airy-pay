using System.Data;
using AiryPayNew.Application.Requests.Products;
using AiryPayNew.Discord.AutocompleteHandlers;
using AiryPayNew.Discord.Utils;
using AiryPayNew.Domain.Entities.Products;
using Discord;
using Discord.Interactions;
using MediatR;
using Sqids;
using DiscordCommands = Discord.Commands;

namespace AiryPayNew.Discord.InteractionModules;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[DiscordCommands.RequireUserPermission(GuildPermission.Administrator)]
[Group("product", "Работа с товарами")]
public class ProductInteractionModule(
    IMediator mediator,
    SqidsEncoder<long> sqidsEncoder) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("create", "Создание нового товара")]
    public async Task Create(
        [Summary("Эмодзи", "Будет отображаться возле товара")] string emojiText,
        [Summary("Название", "Название товара")] string name,
        [Summary("Цена", "Цена товара")] decimal price)
    {
        var validEmojiText = await EmojiParser.GetEmojiText(emojiText);
        if (validEmojiText is null)
        {
            await RespondAsync(":no_entry_sign: Используйте допустимый emoji", ephemeral: true);
            return;
        }
        
        var createProductRequest = new CreateProductRequest(
            Context.Guild.Id,
            new ProductModel(validEmojiText, name, price));
        var operationResult = await mediator.Send(createProductRequest);
        if (operationResult.Successful)
        {
            await RespondAsync(":white_check_mark: Новый товар был создан.", ephemeral: true);
            return;
        }
        
        await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
    }
    
    [SlashCommand("delete", "Удаление товара")]
    public async Task Delete(
        [Summary("Товар", "Товар, который будет удалён"),
         Autocomplete(typeof(ProductAutocompleteHandler))] string productHashId)
    {
        var productId = new ProductId(sqidsEncoder.Decode(productHashId).Single());
        
        var removeProductRequest = new RemoveProductRequest(Context.Guild.Id, productId);
        await mediator.Send(removeProductRequest);
        
        await RespondAsync(":wastebasket: Товар был удалён.", ephemeral: true);
    }
    
    [SlashCommand("edit", "Изменение товара")]
    public async Task Edit(
        [Summary("Товар", "Товар, который будет изменён"),
         Autocomplete(typeof(ProductAutocompleteHandler))] string productHashId,
        [Summary("Эмодзи", "Будет отображаться возле товара")] string emojiText,
        [Summary("Название", "Название товара")] string name,
        [Summary("Цена", "Цена товара")] decimal price)
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
            new ProductModel(validEmojiText, name, price));
        var operationResult = await mediator.Send(editProductRequest);
        if (operationResult.Successful)
        {
            await RespondAsync(":recycle: Товар был изменён.", ephemeral: true);
            return;
        }
        
        await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
    }
}