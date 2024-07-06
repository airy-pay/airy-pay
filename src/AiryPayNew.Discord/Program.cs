using AiryPayNew.Discord.Configuration;
using AiryPayNew.Discord.Utils;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

LanguageChanger.Update(builder.Configuration.GetAppSettings().Language);

builder.Services.AddDiscordHost(builder.Configuration);

var host = builder.Build();

await host.RunAsync();