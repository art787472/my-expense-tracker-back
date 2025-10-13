using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog.Events;
using Serilog;
using ExpenseTracker.Cashe;
using ExpenseTracker.Contract.Cache;
using ExpenseTracker.DbAccess;
using ExpenseTracker.MiddleWares;
using ExpenseTracker.Repository;
using ExpenseTracker.Service;
using Amazon.S3;
using Amazon.Runtime;
using Microsoft.Extensions.DependencyInjection;
Serilog.Debugging.SelfLog.Enable(Console.Error);



var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration["Serilog:WriteTo:2:Args:databaseUrl"];

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("WebDatabase"))
    );
builder.Services.AddScoped<HttpClient, HttpClient>();
builder.Services.AddScoped<ICacheService, MemoryService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IImageStorageService, AmazonStorageService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ChartRepository, ChartRepository>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ClientAuthentication:SecurityKey"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["ClientAuthentication:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["ClientAuthentication:Audience"]
    };
});

var logPath = Path.Combine(AppContext.BaseDirectory, "log-.txt");
// 設定Logger
// 替換內建 logging，使用 Serilog
builder.Host.UseSerilog((ctx, lc) => lc
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(path: logPath, rollingInterval: RollingInterval.Day)
        .WriteTo.MongoDB(
            databaseUrl: mongoConnectionString,
            collectionName: "applogs")
    );

builder.Services.AddHttpContextAccessor();
// 設定CORS，讓前端存取
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercelFrontend", policy =>
    {
        policy
            .WithOrigins("https://expensetracker.yichengchen.idv.tw") // 前端網域
            .AllowAnyHeader()
            .AllowAnyMethod()
        .AllowCredentials();
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLLocalFrontend", policy =>
    {
        policy
            .WithOrigins("https://localhost:3000") // 前端網域
            .AllowAnyHeader()
            .AllowAnyMethod()
        .AllowCredentials();
    });
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("*")
                .WithMethods("*")
                .WithHeaders("*");
        });
});

builder.Services.AddMemoryCache();
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();                    // 顯式添加
app.UseCors("AllowVercelFrontend");
app.UseCors("AllowLLocalFrontend");
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


