<img src="./assets/banner.png" alt="Banner" width="100%">

![Main workflow](https://github.com/airy-pay/airy-pay-new/actions/workflows/main.yml/badge.svg)

<h1>
    <img height="28" src="./assets/logo.png" alt="Logo">
    Swagger themes
</h1>

Discord bot which allows server owners to create a shop where users can purchase roles for real money. Simple yet powerful way to monetize and manage and sell roles within your community.

### 🤖 Bot commands

| Command | Description |
| - | - |
| `/info` | Get current shop information |
| `/product create` | Create product |
| `/product delete` | Delete product |
| `/product edit` | Edit product |
| `/setup` | Set up shop in the server |
| `/withdrawal create` | Create money withdrawal request |

### 📦 Manual deployment

1. Create `appsettings.json` and `paymentsettings.yaml` files in project root directory using templates in `./src/AiryPayPay.Presentation` and configure them.
2. Create `.env` file using template in project root directory:
```js
DISCORD_TOKEN=""
POSTGRES_DB=""
POSTGRES_USER=""
POSTGRES_PASSWORD=""
```
3. Open 80 port to allow payment callbacks.
4. Start the project:
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