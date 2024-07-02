using AiryPayNew.Discord.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordHost(builder.Configuration);

var host = builder.Build();

await host.RunAsync();