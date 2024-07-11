using AiryPayNew.Application.Requests.Products;
using Discord;
using Discord.Interactions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sqids;

namespace AiryPayNew.Discord.AutocompleteHandlers;

public class ProductAutocompleteHandler : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var sqidsEncoder = scope.ServiceProvider.GetRequiredService<SqidsEncoder<long>>();

        var getProductsFromShop = new GetProductsFromShopRequest(context.Guild.Id);
        var productsOperationResult = await mediator.Send(getProductsFromShop);
        if (!productsOperationResult.Successful)
        {
            throw new InvalidOperationException(productsOperationResult.ErrorMessage);
        }
        
        var autocompleteResults = productsOperationResult.Entity
            .Select(p =>
                new AutocompleteResult($"{p.Name} - {p.Price}\u20bd",
                sqidsEncoder.Encode(p.Id.Value)));

        return AutocompletionResult.FromSuccess(autocompleteResults.Take(25));
    }
}