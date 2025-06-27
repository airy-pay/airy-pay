<img src="./assets/banner.png" alt="Banner" width="100%">

![Main workflow](https://github.com/airy-pay/airy-pay-new/actions/workflows/main.yml/badge.svg)

<h1>
    <img height="28" src="./assets/logo.png" alt="Logo">
    AiryPay
</h1>

Monetize, manage and sell roles within your Discord community.

### 📦 Manual deployment

1. Create `appsettings.json` and `paymentsettings.yaml` files in project root directory using templates in `./src/AiryPayPay.Presentation` and configure them.
2. Add `.env` file using template in project root directory:
```js
DISCORD_TOKEN=""
POSTGRES_DB=""
POSTGRES_USER=""
POSTGRES_PASSWORD=""
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
dotnet ef migrations add --project src/AiryPayNew.Infrastructure/AiryPayNew.Infrastructure.csproj --startup-project src/AiryPayNew.Presentation/AiryPayNew.Presentation.csproj --context AiryPayNew.Infrastructure.Data.ApplicationDbContext --configuration Debug --verbose <Migration name>
```
Apply migrations
```
dotnet ef database update --project src/AiryPayNew.Infrastructure/AiryPayNew.Infrastructure.csproj --startup-project src/AiryPayNew.Presentation/AiryPayNew.Presentation.csproj --context AiryPayNew.Infrastructure.Data.ApplicationDbContext --configuration Debug --verbose
```