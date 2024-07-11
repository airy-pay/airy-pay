using AiryPayNew.Discord.Configuration;
using AiryPayNew.Discord.Utils;
using AiryPayNew.Shared.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddYamlFile("paymentsettings.yaml");
var appSettings = builder.Configuration.GetAppSettings();

LanguageChanger.Update(appSettings.Language);

builder.Services
    .AddDiscordHost(appSettings)
    .AddServices(appSettings);

await builder.Build().RunAsync();