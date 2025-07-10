using AiryPayNew.Application.Requests.Withdrawals;
using AiryPayNew.Presentation.Localization;
using AiryPayNew.Presentation.Utils;
using AiryPayNew.Shared.Settings.AppSettings;
using Discord;
using Discord.Interactions;
using MediatR;

namespace AiryPayNew.Presentation.InteractionModules.Impl;

[RequireContext(ContextType.Guild)]
[CommandContextType(InteractionContextType.Guild)]
[Group("withdrawal", "\ud83d\udcb8 Withdrawal")]
public class WithdrawalInteractionModule : ShopInteractionModuleBase
{
    private readonly Color _embedsColor;
    private readonly IMediator _mediator;

    public WithdrawalInteractionModule(
        IMediator mediator,
        AppSettings appSettings) : base(mediator)
    {
        _mediator = mediator;
        _embedsColor = ColorMapper.Map(appSettings.Discord.EmbedMessageColor);
    }

    [SlashCommand("create", "\ud83d\udcb8 Creating a withdrawal")]
    public async Task Create(
        [Summary("Amount", "The amount of money to withdraw")] decimal withdrawalSum,
        [Summary("Card", "Card number to which the money will be sent")] long withdrawalAccount)
    {
        var shop = await GetShopOrRespondAsync();
        var localizer = new Localizer(shop.Language);

        if (withdrawalAccount.ToString().Length != 16)
        {
            await RespondAsync(localizer.GetString("withdrawal.cardInvalid"), ephemeral: true);
            return;
        }

        var createWithdrawalRequest = new CreateWithdrawalRequest(
            Context.Guild.Id, withdrawalSum, "card", withdrawalAccount.ToString());
        var createWithdrawalRequestResult = await _mediator.Send(createWithdrawalRequest);

        if (!createWithdrawalRequestResult.Successful)
        {
            await RespondAsync(
                string.Format(
                    localizer.GetString("withdrawal.create.error"),
                    createWithdrawalRequestResult.ErrorMessage), ephemeral: true);
            return;
        }

        var verifyWithdrawalEmbed = new EmbedBuilder()
            .WithTitle(localizer.GetString("withdrawal.created.title"))
            .WithDescription(localizer.GetString("withdrawal.created.description"))
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName(localizer.GetString("withdrawal.created.amountField"))
                    .WithValue($"{withdrawalSum} \u20bd")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName(localizer.GetString("withdrawal.created.cardField"))
                    .WithValue($"`{CardFormatter.Format(withdrawalAccount.ToString())}`")
                    .WithIsInline(true)
            )
            .WithFooter(
                string.Format(
                    localizer.GetString("withdrawal.created.footer"),
                    DateTime.UtcNow.Year),
                Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        await RespondAsync(
            localizer.GetString("withdrawal.created.warning"),
            embed: verifyWithdrawalEmbed,
            ephemeral: true);
    }
}
