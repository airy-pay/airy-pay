using AiryPayNew.Discord.Configuration;
using AiryPayNew.Discord.Localization;
using AiryPayNew.Shared.Utils;
using Serilog;

// TODO:
// 1. Add localization strings for new error messages
// 2. User operationResult.Failed instead of !operationResult.Success everywhere
// 3. Create a separate class for creating localizedMessageCode values:
//  var localizedMessageCode = operationResult.ErrorType switch
//  {
//      UpdateShopLanguageRequest.Error.ShopNotFound => "shopNotFound",
//      UpdateShopLanguageRequest.Error.LanguageNotSupported => "languageNotValid",
//         _ => "unknownError"
//  };

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());
    
    builder.Configuration.AddYamlFile("paymentsettings.yaml", optional: false);
    var appSettings = builder.Configuration.GetAppSettings();
    
    LanguageChanger.Update(appSettings.Language);
    
    builder.Services
        .AddDiscordHost(appSettings)
        .AddServices(appSettings);
    
    var app = builder.Build();
    
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}