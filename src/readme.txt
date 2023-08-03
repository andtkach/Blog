docker-compose -f docker/docker-compose.yml up

Databse migrations

Add-Migration InitialCreate -Context BlogDbContext -OutputDir Migrations\BlogMigrations
Add-Migration InitialCreate -Context AuthDbContext -OutputDir Migrations\AuthMigrations

Update-Database -Context BlogDbContext
Update-Database -Context AuthDbContext


------Database-----

server:
demoblogsrv.database.windows.net

database:
BlogDbData
BlogDbAuth


u: sabloguser
p: PO34sdf_fd00


Server=tcp:demoblogsrv.database.windows.net,1433;Initial Catalog=BlogDbData;Persist Security Info=False;User ID=sabloguser;Password=PO34sdf_fd00;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;


Server=tcp:demoblogsrv.database.windows.net,1433;Initial Catalog=BlogDbAuth;Persist Security Info=False;User ID=sabloguser;Password=PO34sdf_fd00;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;




------Docker-------
cd app
docker build -f Blog.Web/Dockerfile -t andreytkach/blog .

docker run -p 7001:80 -e "ASPNETCORE_ENVIRONMENT=Production" -e "ContentCache:UseCache=true" --name my-blog andreytkach/blog

docker push andreytkach/blog

------Azure-----
webapp-demoblog.azurewebsites.net

