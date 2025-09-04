<h1>
    <img height="30" src="./assets/logo.png" alt="Logo">
    AiryPay
</h1>

![Main workflow](https://github.com/airy-pay/airy-pay-new/actions/workflows/main.yml/badge.svg)
![Main workflow](https://img.shields.io/github/v/release/airy-pay/airy-pay-new)
![Main workflow](https://img.shields.io/github/stars/airy-pay/airy-pay-new.png)
![Main workflow](https://img.shields.io/github/license/airy-pay/airy-pay-new)

Monetize, manage and sell roles within your Discord community.

<img src="./assets/banner.png" alt="Banner" width="100%">

### Manual deployment

1. Create `appsettings.json` and `paymentsettings.yaml` files in project root directory using templates in `./src/AiryPayPay.Discord` and configure them.
2. Add `.env` file using template in project root directory:

<details>
<summary>Environment file sample</summary>

```js
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
</details>

3. Open 80 port to allow payment callbacks.

4. Update database using [Ef Core commands](https://github.com/airy-pay/airy-pay-new?tab=readme-ov-file#%EF%B8%8F-ef-core-commands)

5. Start the project:
```shell
docker compose up -d
```

6. Update database using EF CLI commands.

> [!NOTE]  
> Run in project container terminal.

<details>

<summary>Ef Core commands</summary>

Create migration
```
dotnet ef migrations add --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj --context AiryPay.Infrastructure.Data.ApplicationDbContext --configuration Debug --verbose <Migration name>
```
Apply migrations
```
dotnet ef database update --project src/AiryPay.Infrastructure/AiryPay.Infrastructure.csproj --startup-project src/AiryPay.Discord/AiryPay.Discord.csproj --context AiryPay.Infrastructure.Data.ApplicationDbContext --configuration Debug --verbose
```

</details>