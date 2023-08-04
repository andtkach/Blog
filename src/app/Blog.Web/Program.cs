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
var cacheSingleton = new MemoryArticleCache(new MemoryCache(new MemoryCacheOptions()));
cacheSingleton.CacheSeconds = 600;
cacheSingleton.UseCache = true;

var articleCacheSeconds = Environment.GetEnvironmentVariable("ArticleCache:CacheSeconds");
if (!string.IsNullOrEmpty(articleCacheSeconds))
{
    if (int.TryParse(articleCacheSeconds, out var value))
    {
        cacheSingleton.CacheSeconds = value;
    }
}

var articleUseCache = Environment.GetEnvironmentVariable("ArticleCache:UseCache");
if (!string.IsNullOrEmpty(articleUseCache))
{
    if (bool.TryParse(articleUseCache, out var value))
    {
        cacheSingleton.UseCache = value;
    }
}

builder.Services.AddSingleton<IArticleCache>(cacheSingleton);

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
