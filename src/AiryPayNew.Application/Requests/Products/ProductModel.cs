namespace AiryPayNew.Application.Requests.Products;

public record ProductModel(string DiscordEmoji, string Name, decimal Price, ulong DiscordRoleId);