using AiryPayNew.Discord.Configuration;
using AiryPayNew.Discord.Http.Middlewares;
using AiryPayNew.Discord.Utils;
using AiryPayNew.Shared.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddYamlFile("paymentsettings.yaml");
var appSettings = builder.Configuration.GetAppSettings();

LanguageChanger.Update(appSettings.Language);

builder.Services
    .AddDiscordHost(appSettings)
    .AddServices(appSettings);

var app = builder.Build();

app.UseMiddleware<IpWhitelistMiddleware>();

app.Run();