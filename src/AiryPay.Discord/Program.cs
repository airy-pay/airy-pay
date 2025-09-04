using AiryPay.Discord.Configuration;
using AiryPay.Discord.Localization;
using AiryPay.Shared.Utils;
using Serilog;

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