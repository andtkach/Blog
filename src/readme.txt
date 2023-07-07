docker-compose -f docker/docker-compose.yml up

Databse migrations

Add-Migration InitialCreate -Context BlogDbContext -OutputDir Migrations\BlogMigrations
Add-Migration InitialCreate -Context AuthDbContext -OutputDir Migrations\AuthMigrations

Update-Database -Context BlogDbContext
Update-Database -Context AuthDbContext

