using AiryPayNew.Application.Requests.Withdrawals;
using AiryPayNew.Presentation.Utils;
using Discord;
using Discord.Interactions;
using MediatR;

namespace AiryPayNew.Presentation.InteractionModules.Impl;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[Group("withdrawal", "\ud83d\udcb8 Withdrawal")]
public class WithdrawalInteractionModule : ShopInteractionModuleBase
{
    private readonly Color _embedsColor = new(40, 117, 233);
    private readonly IMediator _mediator;

    public WithdrawalInteractionModule(
        IMediator mediator) : base(mediator)
    {
        _mediator = mediator;
    }

    [SlashCommand("create", "\ud83d\udcb8 Creating a withdrawal")]
    public async Task Create(
        [Summary("Amount", "The amount of money to withdraw")] decimal withdrawalSum,
        [Summary("Card", "Card number to which the money will be sent")] long withdrawalAccount)
    {
        if (withdrawalAccount.ToString().Length != 16)
        {
            await RespondAsync(":no_entry_sign: Указан неверный номер карты", ephemeral: true);
            return;
        }
        
        var createWithdrawalRequest = new CreateWithdrawalRequest(
            Context.Guild.Id, withdrawalSum, "card", withdrawalAccount.ToString());
        var createWithdrawalRequestResult = await _mediator.Send(createWithdrawalRequest);
        if (!createWithdrawalRequestResult.Successful)
        {
            await RespondAsync(":no_entry_sign: " + createWithdrawalRequestResult.ErrorMessage,
                ephemeral: true);
            return;
        }
        
        var verifyWithdrawalEmbed = new EmbedBuilder()
            .WithTitle("\ud83d\udcb8 Вывод средств создан")
            .WithDescription("Если средства не пришли на указанный счёт в течение 24 часов, пожалуйста, " +
                             "обратитесь в [поддержку](https://discord.gg/Arn9RsRqD9).")
            .WithFields([
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udcb5 Сумма")
                    .WithValue($"{withdrawalSum} \u20bd")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName("\ud83d\udcb3 Номер карты")
                    .WithValue($"`{CardFormatter.Format(withdrawalAccount.ToString())}`")
                    .WithIsInline(true)])
            .WithFooter($"AiryPay \u00a9 {DateTime.UtcNow.Year}", Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();
        
        await RespondAsync(
            "-# В случае ошибки во время вывода средства будут возвращены на счёт магазина.",
            embed: verifyWithdrawalEmbed, ephemeral: true);
    }
}