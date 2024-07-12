/setup  
/info  
/product create <emoji> <name> <price>  
/product remove <id>  
/product edit <id> <emoji> <name> <price>  
/withdrawal <sum>  

## Ef core commands
Create migration
```
dotnet ef migrations add --project src/AiryPayNew.Infrastructure/AiryPayNew.Infrastructure.csproj --startup-project src/AiryPayNew.Presentation/AiryPayNew.Presentation.csproj --context AiryPayNew.Infrastructure.Data.ApplicationDbContext --configuration Debug --verbose AddBillProperty <Migration name>
```
Apply migrations
```
dotnet ef database update --project src/AiryPayNew.Infrastructure/AiryPayNew.Infrastructure.csproj --startup-project src/AiryPayNew.Presentation/AiryPayNew.Presentation.csproj --context AiryPayNew.Infrastructure.Data.ApplicationDbContext --configuration Debug --verbose
```