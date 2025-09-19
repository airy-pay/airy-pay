<h1>
    <img height="30" src="./assets/logo.png" alt="Logo">
    AiryPay
</h1>

[![.NET Version](https://img.shields.io/badge/.NET-8.0-512BD)](https://docs.abblix.com/docs/technical-requirements)
![WorkFlow](https://github.com/airy-pay/airy-pay-new/actions/workflows/main.yml/badge.svg)
![Version](https://img.shields.io/github/v/release/airy-pay/airy-pay-new)
![Stars](https://img.shields.io/github/stars/airy-pay/airy-pay-new.png)
![License](https://img.shields.io/github/license/airy-pay/airy-pay-new)

Monetize, manage and sell roles within your Discord community.

## Starting the project

1. Create `appsettings.json` and `paymentsettings.yaml` files in project root directory.
Template for `appsettings.json`: `/src/AiryPay.Discord/appsettings.json`
Template for `paymentsettings.yaml`: `/paymentsettings.samle.yaml`
Then configure these files.

2. Add `.env` file in project root directory:

```shell
DISCORD_TOKEN=""
POSTGRES_DB=""
POSTGRES_USER=""
POSTGRES_PASSWORD=""
POSTGRES_PUBLIC_PORT=""
RABBITMQ_HOST=""
RABBITMQ_USER=""
RABBITMQ_PASSWORD=""
RABBITMQ_PORT=""
RABBITMQ_WEB_PORT=""
```

3. If you run in release environment open `80` and `443` ports to allow incoming payment callbacks.

4. Update database using Entity Framework commands:
```cs
dotnet ef database update \
    --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj \
    --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj \
    --context AiryPay.Infrastructure.Data.ApplicationDbContext \
    --configuration Debug \
    --verbose \
/
```

> [!NOTE]  
> Run in project container terminal.

5. Start the project:
```shell
docker compose up -d
```

## Basic bot setup

> [!IMPORTANT]  
> Payment systems need to be configured in `paymentsettings.yaml` before setup.

<img src="./assets/setup-instruction.gif" alt="Setup">

## Commands

Create Entity Framework migration 
```
dotnet ef migrations add \
    --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj \
    --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj \
    --context AiryPay.Infrastructure.Data.ApplicationDbContext \
    --configuration Debug \
    --verbose <Migration name> \
/
```
Apply Entity Framework migration
```
dotnet ef database update \
    --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj \
    --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj \
    --context AiryPay.Infrastructure.Data.ApplicationDbContext \
    --configuration Debug \
    --verbose \
/
```
