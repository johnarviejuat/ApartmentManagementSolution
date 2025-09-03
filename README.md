## Database Setup

This solution uses multiple DbContexts. To apply migrations and update the databases, run the following commands in the **Package Manager Console & Developer PowerShell**:

```powershell
Update-Database -Context BillingDbContext
Update-Database -Context CatalogDbContext
Update-Database -Context LeasingDbContext
Update-Database -Context PeopleDbContext


dotnet ef database update --context BillingDbContext
dotnet ef database update --context CatalogDbContext
dotnet ef database update --context LeasingDbContext
dotnet ef database update --context PeopleDbContext
