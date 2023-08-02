docker-compose -f docker/docker-compose.yml up

Databse migrations

Add-Migration InitialCreate -Context BlogDbContext -OutputDir Migrations\BlogMigrations
Add-Migration InitialCreate -Context AuthDbContext -OutputDir Migrations\AuthMigrations

Update-Database -Context BlogDbContext
Update-Database -Context AuthDbContext


------Docker-------
cd app
docker build -f Blog.Web/Dockerfile -t andreytkach/blog .

docker run -p 7001:80 -e "ASPNETCORE_ENVIRONMENT=Production" -e "ContentCache__UseCache=true" --name my-blog andreytkach/blog 