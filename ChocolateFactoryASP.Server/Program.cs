using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ChocolateFactory.Data;
using ChocolateFactory.Repositories;
using ChocolateFactory.Services;
using ChocolateFactory.Helpers;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token. Example: \"Bearer abc123\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure()));

// Configure Authentication (JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Register repositories
builder.Services.AddScoped<MaintenanceRecordRepository>();
builder.Services.AddScoped<FinishedGoodsRepository>();
builder.Services.AddScoped<ProductionScheduleRepository>();
builder.Services.AddScoped<RawMaterialRepository>();
builder.Services.AddScoped<RecipeRepository>();
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddScoped<SalesOrderRepository>();
builder.Services.AddScoped<WarehouseRepository>();
builder.Services.AddScoped<QualityCheckRepository>();
builder.Services.AddScoped<SupplierRepository>();

// Register services
builder.Services.AddScoped<MaintenanceService>();
builder.Services.AddScoped<PackagingService>();
builder.Services.AddScoped<ProductionService>();
builder.Services.AddScoped<RawMaterialService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<SalesService>();
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<QualityControlService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SupplierService>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtHelper>();

builder.Services.AddSingleton<NotificationService>();

var app = builder.Build();

// Configure middleware for error handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var errorResponse = new { message = ex.Message, details = ex.StackTrace };
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
    options.WithOrigins("http://localhost:4200", "https://chococo.vercel.app", "https://ashy-ground-0b7c62500.4.azurestaticapps.net")
    .AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
