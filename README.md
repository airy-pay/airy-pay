<h1>
    <img height="28" src="./assets/logo.png" alt="Logo">
    AiryPay
</h1>

![Main workflow](https://github.com/airy-pay/airy-pay-new/actions/workflows/main.yml/badge.svg)

Monetize, manage and sell roles within your Discord community.

<img src="./assets/banner.png" alt="Banner" width="100%">

### 📦 Manual deployment

1. Create `appsettings.json` and `paymentsettings.yaml` files in project root directory using templates in `./src/AiryPayPay.Discord` and configure them.
2. Add `.env` file using template in project root directory:
```js
DISCORD_TOKEN=""
POSTGRES_DB=""
POSTGRES_USER=""
POSTGRES_PASSWORD=""
POSTGRES_PUBLIC_PORT=""
RABBITMQ_HOST="localhost"
RABBITMQ_USER="airypay_user"
RABBITMQ_PASSWORD="AtCY75F8hoqYcsX9"
RABBITMQ_PORT="5672"
RABBITMQ_WEB_PORT="15672"
```
3. Open 80 port to allow payment callbacks.
4. Update database using [Ef Core commands](https://github.com/airy-pay/airy-pay-new?tab=readme-ov-file#%EF%B8%8F-ef-core-commands)
5. Start the project:
```shell
docker compose up -d
```

### 🗂️ Ef Core commands
> [!NOTE]  
> Run in Docker container CMD when using containers.  

Create migration
```
dotnet ef migrations add --project src/AiryPayNew.Infrastructure/AiryPayNew.Infrastructure.csproj --startup-project src/AiryPayNew.Discord/AiryPayNew.Discord.csproj --context AiryPayNew.Infrastructure.Data.ApplicationDbContext --configuration Debug --verbose <Migration name>
```
Apply migrations
```
dotnet ef database update --project src/AiryPayNew.Infrastructure/AiryPayNew.Infrastructure.csproj --startup-project src/AiryPayNew.Discord/AiryPayNew.Discord.csproj --context AiryPayNew.Infrastructure.Data.ApplicationDbContext --configuration Debug --verbose
```