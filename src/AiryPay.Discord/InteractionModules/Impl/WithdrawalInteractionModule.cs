using AiryPay.Application.Requests.Withdrawals;
using AiryPay.Discord.Localization;
using AiryPay.Discord.Utils;
using AiryPay.Shared.Settings;
using Discord;
using Discord.Interactions;
using MediatR;

namespace AiryPay.Discord.InteractionModules.Impl;

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
        var (_, localizer) = await GetShopAndLocalizerAsync();

        if (withdrawalAccount.ToString().Length != 16)
        {
            await RespondAsync(localizer.Withdrawal_CardInvalid, ephemeral: true);
            return;
        }

        var createWithdrawalRequest = new CreateWithdrawalRequest(
            ShopId, withdrawalSum, "card", withdrawalAccount.ToString());
        var createWithdrawalRequestResult = await _mediator.Send(createWithdrawalRequest);

        if (!createWithdrawalRequestResult.Successful)
        {
            var localizedMessageCode = createWithdrawalRequestResult.ErrorType switch
            {
                CreateWithdrawalRequest.Error.InvalidWithdrawalAmount => "invalidWithdrawalAmount",
                CreateWithdrawalRequest.Error.InvalidWithdrawalDetails => "invalidWithdrawalDetails",
                CreateWithdrawalRequest.Error.InvalidWithdrawalMethod => "invalidWithdrawalMethod",
                CreateWithdrawalRequest.Error.ShopNotFound => "shopNotFound",
                CreateWithdrawalRequest.Error.WithdrawalAmountTooLow => "withdrawalAmountTooLow",
                CreateWithdrawalRequest.Error.InsufficientFunds => "insufficientFunds",
                _ => "validationFailed",
            };

            await RespondAsync(
                string.Format(
                    localizer.Withdrawal_Create_Error,
                    localizer.GetString(localizedMessageCode)), ephemeral: true);
            return;
        }

        var verifyWithdrawalEmbed = new EmbedBuilder()
            .WithTitle(localizer.Withdrawal_Created_Title)
            .WithDescription(localizer.Withdrawal_Created_Description)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName(localizer.Withdrawal_Created_AmountField)
                    .WithValue($"{withdrawalSum} \u20bd")
                    .WithIsInline(true),
                new EmbedFieldBuilder()
                    .WithName(localizer.Withdrawal_Created_CardField)
                    .WithValue($"`{CardFormatter.Format(withdrawalAccount.ToString())}`")
                    .WithIsInline(true)
            )
            .WithFooter(
                string.Format(
                    localizer.Withdrawal_Created_Footer,
                    DateTime.UtcNow.Year),
                Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(_embedsColor)
            .Build();

        await RespondAsync(
            localizer.Withdrawal_Created_Warning,
            embed: verifyWithdrawalEmbed,
            ephemeral: true);
    }
}
