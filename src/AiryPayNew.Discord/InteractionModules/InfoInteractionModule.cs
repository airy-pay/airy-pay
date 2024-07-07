using AiryPayNew.Application.Requests.Shops;
using AiryPayNew.Domain.Entities.Withdrawals;
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
            .WithFooter($"AiryPay \u00a9 {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
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

        var productsEmbed = new EmbedBuilder()
            .WithTitle("\ud83d\udce6 Товары")
            .WithDescription(
                operationResult.Entity.Products.Count == 0 ? "Тут пока пусто.\n" +
                                         "Создайте новый товар при помощи команды `/product create`" : null)
            .WithFields(operationResult.Entity.Products.Select(x => new EmbedFieldBuilder()
                .WithName($"{x.Emoji} {x.Name}")
                .WithValue($"""
                             Стоимость: **{x.Price} ₽**
                             Роль: <@&{x.DiscordRoleId}>
                             """)
                .WithIsInline(true)))
            .WithFooter($"AiryPay \u00a9 {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();
        
        await RespondAsync(
            embed: productsEmbed,
            ephemeral: true);
    }
    
    [ComponentInteraction("InfoInteractionModule.GetWithdrawals")]
    public async Task GetWithdrawals()
    {
        var getShopWithdrawalsRequest = new GetShopWithdrawalsRequest(Context.Guild.Id);
        var withdrawals = await mediator.Send(getShopWithdrawalsRequest);

        var withdrawalStatusesGetter = new Dictionary<WithdrawalStatus, string>()
        {
            { WithdrawalStatus.InProcess, "\ud83d\udfe1 В процессе" },
            { WithdrawalStatus.Paid, "\ud83d\udfe2 Выплачен" },
            { WithdrawalStatus.Canceled, "\ud83d\udd34 Отменён" },
        };
        
        var withdrawalsEmbed = new EmbedBuilder()
            .WithTitle("\ud83d\udcb8 Выводы средств")
            .WithDescription(
                withdrawals.Count == 0 ? "Тут пока пусто.\n" +
                                         "Создайте вывод средств при помощи команды `/withdrawal`" : null)
            .WithFields(withdrawals.Select(x => new EmbedFieldBuilder()
                .WithName($"\ud83d\udcb3 {x.DateTime:0:dd/MM/yy H:mm:ss} MasterCard")
                .WithValue($"""
                            Номер счёта: ||{x.ReceivingAccountNumber}||
                            Сумма: **{x.Amount} ₽**
                            Статус: **{withdrawalStatusesGetter[x.WithdrawalStatus]}**
                            """)
                .WithIsInline(false)))
            .WithFooter($"AiryPay \u00a9 {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();
        
        await RespondAsync(
            embed: withdrawalsEmbed,
            ephemeral: true);
    }
    
    [ComponentInteraction("InfoInteractionModule.GetPurchases")]
    public async Task GetPurchases()
    {
        var getShopPurchasesRequest = new GetShopPurchasesRequest(Context.Guild.Id);
        var purchases = await mediator.Send(getShopPurchasesRequest);
        
        var purchasesEmbed = new EmbedBuilder()
            .WithTitle("\ud83d\udce6 Последние покупки")
            .WithDescription(
                purchases.Count == 0 ? "Тут пока пусто.\n" +
                                       "Пользователи пока не совершали покупки." : null)     
            .WithFields(purchases.Select(x => new EmbedFieldBuilder()
                .WithName(x.Product.Name)
                .WithValue($"""
                            Попупатель: <@{x.Bill.BuyerDiscordId}>
                            Роль: <@&{x.Product.DiscordRoleId}>
                            Прибыль: **{x.Product.Price} ₽**
                            Дата: `{x.DateTime}`
                            """)
                .WithIsInline(true)))
            .WithFooter($"AiryPay \u00a9 {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();
        
        await RespondAsync(
            embed: purchasesEmbed,
            ephemeral: true);
    }
}