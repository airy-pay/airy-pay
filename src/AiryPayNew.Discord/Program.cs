using AiryPayNew.Discord.Configuration;
using AiryPayNew.Discord.Utils;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var appSettings = builder.Configuration.GetAppSettings();

LanguageChanger.Update(appSettings.Language);

builder.Services
    .AddDiscordHost(appSettings)
    .AddServices(appSettings);

var host = builder.Build();

await host.RunAsync();