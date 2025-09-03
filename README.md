## Database Setup

This solution uses multiple DbContexts. To apply migrations and update the databases, run the following commands in the **Package Manager Console**:

```powershell
Update-Database -Context BillingDbContext
Update-Database -Context CatalogDbContext
Update-Database -Context LeasingDbContext
Update-Database -Context PeopleDbContext
