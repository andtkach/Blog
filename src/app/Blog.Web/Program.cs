using Blog.Web.BackgroundServices;
using Blog.Web.Cache;
using Blog.Web.Data;
using Blog.Web.Repositories;
using Blog.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDbContext<BlogDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("BlogDbConnectionString")));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AuthDbConnectionString")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
});

builder.Services.AddSingleton<ArticleCacheProcessingChannel>();
builder.Services.AddHostedService<TimeCacheService>();
builder.Services.AddHostedService<RequestCacheService>();

builder.Services.AddMemoryCache();
var cacheSingleton = new MemoryContentCache(new MemoryCache(new MemoryCacheOptions()));
cacheSingleton.ContentCacheSeconds = 60;
cacheSingleton.UseCache = false;

var contentCacheSeconds = Environment.GetEnvironmentVariable("ContentCache:CacheSeconds");
if (!string.IsNullOrEmpty(contentCacheSeconds))
{
    if (int.TryParse(contentCacheSeconds, out var value))
    {
        cacheSingleton.ContentCacheSeconds = value;
    }
}

var contentUseCache = Environment.GetEnvironmentVariable("ContentCache:UseCache");
if (!string.IsNullOrEmpty(contentUseCache))
{
    if (bool.TryParse(contentUseCache, out var value))
    {
        cacheSingleton.UseCache = value;
    }
}

builder.Services.AddSingleton<IContentCache>(cacheSingleton);

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IImageRepository, CloudinaryImageRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ILikeRepository, LikeRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IArticleService, ArticleService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
