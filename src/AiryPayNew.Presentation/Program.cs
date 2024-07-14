using AiryPayNew.Discord.Configuration;
using AiryPayNew.Discord.Http;
using AiryPayNew.Discord.Utils;
using AiryPayNew.Shared.Utils;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());
    
    builder.Configuration.AddYamlFile("paymentsettings.yaml");
    var appSettings = builder.Configuration.GetAppSettings();
    
    LanguageChanger.Update(appSettings.Language);
    
    builder.Services
        .AddDiscordHost(appSettings)
        .AddServices(appSettings);
    
    var app = builder.Build();
    
    app.AddHttpEndpoints();
    
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