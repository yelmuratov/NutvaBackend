using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using NutvaCms.Persistence.DbContexts;
using NutvaCms.Persistence.Seed;
using NutvaCms.Application.Interfaces;
using NutvaCms.Application.Services;
using NutvaCms.Infrastructure.Repositories;
using NutvaCms.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// =========================
// Connection string logic (Heroku support)
// =========================
var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

string connectionString = !string.IsNullOrWhiteSpace(dbUrl)
    ? ConvertDatabaseUrlToConnectionString(dbUrl)
    : builder.Configuration.GetConnectionString("DefaultConnection");

static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}

// =========================
// Configure Services
// =========================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IBlogRepository, BlogRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
builder.Services.AddScoped<ITrackingPixelRepository, TrackingPixelRepository>();

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IStatisticService, StatisticService>();
builder.Services.AddHttpClient<IBitrixService, BitrixService>();
builder.Services.AddScoped<ITrackingPixelService, TrackingPixelService>();
builder.Services.AddSingleton<ITokenBlacklistService, InMemoryTokenBlacklistService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Nutva CMS API",
        Version = "v1",
        Description = "CMS for managing blogs, products, banners with JWT authentication"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// =========================
// Build and Configure App
// =========================

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nutva CMS API v1");
        c.RoutePrefix = "swagger";
    });
}

// Custom domain redirect to HTTPS version
app.Use(async (context, next) =>
{
    var host = context.Request.Host.Host;
    if (host == "nutvahealth.uz")
    {
        context.Response.Redirect("https://www.nutvahealth.uz" + context.Request.Path + context.Request.QueryString);
        return;
    }
    await next();
});

app.UseMiddleware<JwtBlacklistMiddleware>();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DbSeeder.SeedSuperAdminAsync(db);
}

app.Run();
