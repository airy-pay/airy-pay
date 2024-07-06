using AiryPayNew.Application.Requests.Products;
using AiryPayNew.Discord.Utils;
using Discord;
using Discord.Interactions;
using MediatR;
using DiscordCommands = Discord.Commands;

namespace AiryPayNew.Discord.InteractionModules;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[DiscordCommands.RequireUserPermission(GuildPermission.Administrator)]
[Group("product", "Работа с товарами")]
public class ProductInteractionModule(IMediator mediator) : InteractionModuleBase<SocketInteractionContext>
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
        
        var createProductRequest = new CreateProductRequest(Context.Guild.Id, validEmojiText, name, price);
        var operationResult = await mediator.Send(createProductRequest);
        if (operationResult.Successful)
        {
            await RespondAsync(":white_check_mark: Новый товар был создан.", ephemeral: true);
            return;
        }
        
        await RespondAsync(":no_entry_sign: " + operationResult.ErrorMessage, ephemeral: true);
    }
}