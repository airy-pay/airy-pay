using AiryPayNew.Application.Requests.Shops;
using Discord;
using Discord.Interactions;
using MediatR;
using DiscordCommands = Discord.Commands;

namespace AiryPayNew.Discord.InteractionModules;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[DiscordCommands.RequireUserPermission(GuildPermission.Administrator)]
public class InfoInteractionModule(IMediator mediator) : InteractionModuleBase<SocketInteractionContext>
{
    private readonly Color _embedsColor = new(40, 117, 233);
    
    [SlashCommand("info", "\ud83c\udf10 Информация и магазине")]
    public async Task Info()
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
            await RespondAsync(":no_entry_sign: " + "Магазин не найден", ephemeral: true);
            return;
        }
        
        var shopInfoEmbed = new EmbedBuilder()
            .WithTitle("\ud83c\udf10 Информация о магазине")
            .WithFields([
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udcb0 Баланс")
                    .WithValue($"{operationResult.Entity.Balance} \u20bd")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udd04 Статус")
                    .WithValue(operationResult.Entity.Blocked ? "Заблокирован" : "Активен")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udecd\ufe0f Товары")
                    .WithValue(operationResult.Entity.Products.Count)
                    .WithIsInline(true)])
            .WithFooter("AiryPay \u00a9 2024", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();
        
        var productsButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.GetProducts")
            .WithLabel("Товары")
            .WithEmote(new Emoji("\ud83d\udecd\ufe0f"))
            .WithStyle(ButtonStyle.Primary);
        var withdrawalsButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.GetWithdrawals")
            .WithLabel("Выводы средств")
            .WithEmote(new Emoji("\ud83d\udcb8"))
            .WithStyle(ButtonStyle.Primary);
        var lastPurchasesButton = new ButtonBuilder()
            .WithCustomId("InfoInteractionModule.GetPurchases")
            .WithLabel("Последние покупки")
            .WithEmote(new Emoji("\ud83d\udce6"))
            .WithStyle(ButtonStyle.Primary);
        var supportButton = new ButtonBuilder()
            .WithLabel("Поддержка")
            .WithUrl("https://airypay.ru/discord")
            .WithEmote(new Emoji("\ud83d\udcac"))
            .WithStyle(ButtonStyle.Link);
        var termsButton = new ButtonBuilder()
            .WithUrl("https://airypay.ru/terms")
            .WithLabel("Условия предоставления услуг")
            .WithEmote(new Emoji("\ud83d\udcc3"))
            .WithStyle(ButtonStyle.Link);
        
        var messageComponents = new ComponentBuilder()
            .WithRows(new[]
            {
                new ActionRowBuilder()
                    .WithButton(productsButton)
                    .WithButton(withdrawalsButton)
                    .WithButton(lastPurchasesButton),
                new ActionRowBuilder()
                    .WithButton(supportButton)
                    .WithButton(termsButton),
            })
            .Build();
        
        await RespondAsync(
            embed: shopInfoEmbed,
            components: messageComponents,
            ephemeral: true);
    }
    
    [ComponentInteraction("InfoInteractionModule.GetProducts")]
    public async Task GetProducts()
    {
        var productsEmbed = new EmbedBuilder()
            .WithTitle("\ud83d\udce6 Товары")
            .WithFields([
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udc4d Товар 1")
                    .WithValue("100 \u20bd")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udc4d Товар 2")
                    .WithValue("100 \u20bd")
                    .WithIsInline(true)])
            .WithFooter("AiryPay \u00a9 2024", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();
        
        await RespondAsync(
            embed: productsEmbed,
            ephemeral: true);
    }
    
    [ComponentInteraction("InfoInteractionModule.GetWithdrawals")]
    public async Task GetWithdrawals()
    {
        var withdrawalsEmbed = new EmbedBuilder()
            .WithTitle("\ud83d\udcb8 Выводы средств")
            .WithFields([
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udcb3 12.05.2024 MasterCard ")
                    .WithValue($"""
                                Номер счёта: ||1234123412341234||
                                Сумма: **2000 ₽**
                                Статус: **🔴 В процессе**
                                """)
                    .WithIsInline(false),
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udcb3 10.05.2024 MasterCard ")
                    .WithValue($"""
                                Номер счёта: ||1234123412341234||
                                Сумма: **2000 ₽**
                                Статус: **🟢 Выплачено**
                                """)
                    .WithIsInline(false)])
            .WithFooter("AiryPay \u00a9 2024", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();
        
        await RespondAsync(
            embed: withdrawalsEmbed,
            ephemeral: true);
    }
    
    [ComponentInteraction("InfoInteractionModule.GetPurchases")]
    public async Task GetPurchases()
    {
        var withdrawalsEmbed = new EmbedBuilder()
            .WithTitle("\ud83d\udce6 Последние покупки")
            .WithFields([
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udc4d Товар 1")
                    .WithValue($"""
                                Попупатель: <@969204467578830929>
                                Роль: <@&1259503635771953152>
                                Прибыль: **100 ₽**
                                Дата: `12.03.2024 18:20`
                                """)
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udc4d Товар 1")
                    .WithValue($"""
                                Попупатель: <@969204467578830929>
                                Роль: <@&1259503635771953152>
                                Прибыль: **100 ₽**
                                Дата: `12.03.2024 18:20`
                                """)
                    .WithIsInline(true)])
            .WithFooter("AiryPay \u00a9 2024", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();
        
        await RespondAsync(
            embed: withdrawalsEmbed,
            ephemeral: true);
    }
}