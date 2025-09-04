using Discord;
using Discord.Addons.ChainHandlers.ChainHandlers;
using Discord.Addons.ChainHandlers.Common;
using Discord.Interactions;
using Discord.WebSocket;
using IResult = Discord.Interactions.IResult;

namespace AiryPay.Discord.ChainHandlers;

public class CommandAuthChainHandler(
    IServiceProvider provider,
    InteractionService interactionService,
    DiscordSocketClient socketClient)
    : ChainHandler(provider, interactionService, socketClient)
{
    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        if (interaction.Type is not InteractionType.ApplicationCommand)
            return await base.Handle(interaction);
        if (interaction.User.IsBot)
            return InteractionResult.UnhandledException;
        
        var user = interaction.User as SocketGuildUser;
        if (user is null)
            return InteractionResult.UnhandledException;
        
        if (user.GuildPermissions.Administrator)
        {
            return await base.Handle(interaction);
        }
        
        await interaction.RespondAsync(":no_entry_sign: No rights for command execution", ephemeral: true);
        return InteractionResult.Success;
    }
}