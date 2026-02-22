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

## Table of Contents

- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
  - [appsettings.json](#appsettingsjson)
  - [paymentsettings.yaml](#paymentsettingsyaml)
  - [Environment variables](#environment-variables)
- [Deployment](#deployment)
  - [Docker (recommended)](#docker-recommended)
  - [Manual](#manual)
- [Database migrations](#database-migrations)
- [Basic bot setup](#basic-bot-setup)
- [Payment providers](#payment-providers)
- [Supported languages](#supported-languages)
- [Running tests](#running-tests)
- [Commands reference](#commands-reference)

---

## Architecture

AiryPay follows **Clean Architecture** with a CQRS pattern via MediatR. The solution is split into six projects:

```
src/
├── AiryPay.Domain          # Entities, value objects, repository interfaces — no external dependencies
├── AiryPay.Application     # Use cases (MediatR handlers), validators, payment service abstractions
├── AiryPay.Infrastructure  # EF Core repositories, payment provider implementations, RabbitMQ
├── AiryPay.Shared          # AppSettings models, YAML config binding, shared utilities
├── AiryPay.Discord         # Discord.Net bot, interaction modules, slash commands, localization
└── AiryPay.Web             # ASP.NET Core webhook receiver for payment callbacks
```

```
tests/
├── AiryPay.Tests.Domain          # Domain entity and value object unit tests
├── AiryPay.Tests.Application     # Handler and validator unit tests (Moq + FluentAssertions)
├── AiryPay.Tests.Discord         # Discord interaction module tests
├── AiryPay.Tests.Infrastructure  # Repository and payment service integration tests
└── AiryPay.Tests.Architecture    # Architecture constraint tests (NetArchTest)
```

**Runtime services (Docker Compose):**

| Service | Image | Purpose |
|---|---|---|
| `airypay.discord` | airypay.discord | Discord bot process |
| `airypay.web` | airypay.web | HTTP server for payment callbacks |
| `postgres` | postgres:17 | Primary database |
| `rabbitmq` | rabbitmq:3.13-management | Async messaging between Web and Discord |

Payment flow: a buyer initiates a payment → `airypay.web` receives the provider callback → publishes a message to RabbitMQ → `airypay.discord` consumes the message and assigns the Discord role.

---

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose v2
- .NET 8 SDK (for local development and migrations)
- A Discord application with a bot token ([Discord Developer Portal](https://discord.com/developers/applications))
- At least one configured payment provider (see [Payment providers](#payment-providers))

---

## Configuration

### appsettings.json

Create `appsettings.json` in the project root using `/src/AiryPay.Discord/appsettings.json` as a template.

```jsonc
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "./logs/log_.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  },

  // Default culture for the bot's own UI (not per-shop language)
  "Language": "en-US",

  // Languages available to shop owners for their storefronts
  // Supported values: "en", "ru", "es", "pt", "fr", "de"
  "BotSupportedLanguages": ["en", "ru", "es", "pt", "fr", "de"],

  "Kestrel": {
    // Whitelist of IPs allowed to POST payment callbacks to /api/...
    // Add your payment provider's IP ranges here
    "AllowedIPs": ["127.0.0.1", "::1"],
    "Endpoints": {
      "Http": { "Url": "http://*:80" }
    }
  },

  "Links": {
    "SupportUrl": "https://discord.gg/your-invite",
    "TermsUrl": "https://yoursite.com/terms"
  },

  "Discord": {
    // Set to true during development to restrict slash commands to one server
    "UseStagingServer": false,
    "StagingServerId": 0,

    // RGB color used for all embed messages
    "EmbedMessageColor": { "R": 40, "G": 120, "B": 230 },

    // Tiered rate limiting: multiple rules are all enforced simultaneously
    "RateLimiters": [
      { "Limit": 3,    "Period": "1s",  "BanPeriod": "1m"  },
      { "Limit": 220,  "Period": "10m", "BanPeriod": "1h"  },
      { "Limit": 1000, "Period": "2h",  "BanPeriod": "2d"  }
    ]
  }
}
```

### paymentsettings.yaml

Create `paymentsettings.yaml` in the project root using `/paymentsettings.samle.yaml` as a template.

```yaml
# Default commission (%) applied to new shops. Can be overridden per shop.
defaultShopCommission: 10.0

# Minimum amount (in your base currency) a shop owner can withdraw
minimalWithdrawalAmount: 500.0

# --- Payment provider credentials ---
# Only fill in the providers you intend to use.
# Unused providers can be left with empty strings.

ruKassaSettings:
  merchantId: 0
  token: ""
  userEmail: ""
  userPassword: ""

finPaySettings:
  shopId: 0
  key1: ""
  key2: ""

stripeSettings:
  apiKey: ""
  successUrl: "https://yoursite.com/success"
  cancelUrl: "https://yoursite.com/cancel"

squareSettings:
  accessToken: ""
  sandbox: true          # Set to false in production

payPalSettings:
  clientId: ""
  secret: ""
  sandbox: true          # Set to false in production

# Payment methods shown to buyers in Discord
# methodId must match the provider's internal identifier
paymentMethods:
  - serviceName: "Checkout"
    methodId: "card"
    discordEmoji: ":credit_card:"
    name: "Bank card"
    description: "Pay with Visa / Mastercard"
    minimalSum: 100.00
```

### Environment variables

Create a `.env` file in the project root:

```shell
# Discord bot token from the Developer Portal
DISCORD_TOKEN=""

# PostgreSQL
POSTGRES_DB=""
POSTGRES_USER=""
POSTGRES_PASSWORD=""
POSTGRES_PUBLIC_PORT="5432"     # Host port mapped to the container

# RabbitMQ
RABBITMQ_HOST="rabbitmq"
RABBITMQ_USER=""
RABBITMQ_PASSWORD=""
RABBITMQ_PORT="5672"
RABBITMQ_WEB_PORT="15672"       # Management UI port
```

> [!WARNING]
> Never commit `.env`, `appsettings.json`, or `paymentsettings.yaml` to version control. They contain secrets.

---

## Deployment

### Docker (recommended)

**1. Prepare config files**

Complete `appsettings.json`, `paymentsettings.yaml`, and `.env` as described above.

**2. Build images**

```shell
docker compose build
```

**3. Apply database migrations**

Start only the database first, then run migrations from inside the Discord container:

```shell
docker compose up -d postgres
docker compose run --rm airypay.discord dotnet ef database update \
    --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj \
    --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj \
    --context AiryPay.Infrastructure.Data.ApplicationDbContext
```

> [!NOTE]
> Alternatively, exec into a running container: `docker compose exec airypay.discord bash`

**4. Start all services**

```shell
docker compose up -d
```

**5. Open firewall ports**

In a production environment, ports `80` and `443` must be open to receive inbound payment callbacks from providers.

**6. Verify**

```shell
docker compose ps          # All services should show "healthy" or "running"
docker compose logs -f     # Tail combined logs
```

**Updating to a new version:**

```shell
git pull
docker compose build
docker compose up -d
```

If the new version includes database migrations, run step 3 again before step 4.

---

### Manual

For running without Docker (development or debugging):

**1.** Start PostgreSQL and RabbitMQ locally (or point to remote instances via connection strings in environment variables).

**2.** Set environment variables in your shell or in `launchSettings.json`:

```shell
export DISCORD_TOKEN="your-token"
export POSTGRES_DB="airpay"
export POSTGRES_USER="postgres"
export POSTGRES_PASSWORD="secret"
export RABBITMQ_HOST="localhost"
export RABBITMQ_USER="guest"
export RABBITMQ_PASSWORD="guest"
```

**3.** Apply migrations:

```shell
dotnet ef database update \
    --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj \
    --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj \
    --context AiryPay.Infrastructure.Data.ApplicationDbContext \
    --configuration Debug
```

**4.** Run both projects (in separate terminals):

```shell
dotnet run --project src/AiryPay.Discord/AiryPay.Discord.csproj
dotnet run --project src/AiryPay.Web/AiryPay.Web.csproj
```

---

## Database migrations

**Create a new migration** after changing domain entities:

```shell
dotnet ef migrations add <MigrationName> \
    --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj \
    --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj \
    --context AiryPay.Infrastructure.Data.ApplicationDbContext \
    --configuration Debug \
    --verbose
```

**Apply pending migrations:**

```shell
dotnet ef database update \
    --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj \
    --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj \
    --context AiryPay.Infrastructure.Data.ApplicationDbContext \
    --configuration Debug \
    --verbose
```

**Roll back to a specific migration:**

```shell
dotnet ef database update <MigrationName> \
    --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj \
    --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj \
    --context AiryPay.Infrastructure.Data.ApplicationDbContext
```

**List applied migrations:**

```shell
dotnet ef migrations list \
    --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj \
    --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj \
    --context AiryPay.Infrastructure.Data.ApplicationDbContext
```

---

## Basic bot setup

> [!IMPORTANT]
> Payment systems must be configured in `paymentsettings.yaml` before running the setup command.

<img src="./assets/setup-instruction.gif" alt="Setup">

1. Invite the bot to your server with the `bot` and `applications.commands` scopes, and grant it the **Manage Roles** permission.
2. Run the `/setup` slash command in your server.
3. Configure your shop name, language, and available payment methods.
4. Add products via `/product create`.
5. Share the generated purchase link with your community.

> [!NOTE]
> The bot must have a role **higher** than the roles it assigns. Drag the bot's role above any roles it needs to grant in your server's role list.

---

## Payment providers

| Provider | Region | Sandbox | Config key |
|---|---|---|---|
| RuKassa | Russia | No | `ruKassaSettings` |
| FinPay | Russia | No | `finPaySettings` |
| Stripe | Global | Yes | `stripeSettings` |
| Square | US / Global | Yes | `squareSettings` |
| PayPal | Global | Yes | `payPalSettings` |

Each provider must have a corresponding entry in `paymentMethods` in `paymentsettings.yaml` to appear in the Discord UI.

**Webhook URLs** (register these in your provider's dashboard):

| Provider | Callback URL |
|---|---|
| RuKassa | `http://your-domain/api/rukassa` |
| FinPay | `http://your-domain/api/finpay` |
| Stripe | `http://your-domain/api/stripe` |
| Square | `http://your-domain/api/square` |
| PayPal | `http://your-domain/api/paypal` |

Replace `your-domain` with your server's public IP or domain name.

---

## Supported languages

Shop owners can set their storefront language independently. The following locales are supported:

| Code | Language |
|---|---|
| `en` | English |
| `ru` | Russian |
| `es` | Spanish |
| `pt` | Portuguese |
| `fr` | French |
| `de` | German |

To add a new language, add a `.resx` resource file under `src/AiryPay.Discord/Localization/` and include the language code in `BotSupportedLanguages` in `appsettings.json`.

---

## Running tests

Run all test suites:

```shell
dotnet test
```

Run a specific project:

```shell
dotnet test tests/AiryPay.Tests.Application/AiryPay.Tests.Application.csproj
```

Run with code coverage:

```shell
dotnet test --collect:"XPlat Code Coverage"
```

| Test project | Scope |
|---|---|
| `Tests.Domain` | Value objects, entity invariants |
| `Tests.Application` | MediatR handlers, FluentValidation rules |
| `Tests.Discord` | Interaction modules, embed builders |
| `Tests.Infrastructure` | Repositories, payment service adapters |
| `Tests.Architecture` | Dependency direction, naming rules |

---

## Commands reference

| Command | Description |
|---|---|
| `/setup` | Initial shop configuration wizard |
| `/product create` | Add a new product to your shop |
| `/product edit` | Edit an existing product |
| `/product remove` | Remove a product |
| `/shop info` | View shop statistics and balance |
| `/shop settings` | Change shop language and settings |
| `/withdrawal create` | Request a balance withdrawal |
