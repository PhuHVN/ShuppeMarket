using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ShuppeMarket.API;
using ShuppeMarket.API.Middleware;
using ShuppeMarket.Application.DTOs;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Application.Services;
using ShuppeMarket.Application.Validation;
using ShuppeMarket.Domain.Enums.EnumConfig;
using ShuppeMarket.Infrastructure.DatabaseSettings;
using ShuppeMarket.Infrastructure.Seed;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

//Swagger + JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.EnableAnnotations();
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "ShuppeMarket API", Version = "v1" });

    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });

    option.SchemaGeneratorOptions.SchemaIdSelector = type => type.FullName;
}
);
//Jwt Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    var googleSection = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleSection["ClientId"];
    options.ClientSecret = googleSection["ClientSecret"];
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();

            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                IsSuccess = false,
                Message = "Unauthorized or missing token"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },

        OnAuthenticationFailed = async context =>
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var message = context.Exception?.GetType().Name switch
            {
                "SecurityTokenExpiredException" => "Token expired",
                "SecurityTokenInvalidSignatureException" => "Invalid token signature",
                _ => "Invalid token"
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                IsSuccess = false,
                Message = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        },

        OnForbidden = async context =>
        {
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                IsSuccess = false,
                Message = "You do not have permission to access this resource"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    };
});



// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();

}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ExclusiveEnumConverterFactory(excludeFromString: new[] { typeof(StatusCodeHelper) }));
});
builder.Services.AddHttpContextAccessor();


//Auto Mapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

//Add Dependency Injection
builder.Services.AddConfig(builder.Configuration);

//Cloudinary
var cloudName = builder.Configuration["Cloudinary:CloudName"];
var apiKey = builder.Configuration["Cloudinary:ApiKey"];
var apiSecret = builder.Configuration["Cloudinary:ApiSecret"];

var account = new Account(cloudName, apiKey, apiSecret);
var cloudinary = new Cloudinary(account);
builder.Services.AddSingleton<ICloudinary>(sp => cloudinary);
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

//Redis Cache
var redisConfig = builder.Configuration.GetSection("Redis");

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var host = redisConfig["Host"];
    var port = redisConfig["Port"] ?? "6379";
    var password = redisConfig["Token"];

    var options = new ConfigurationOptions
    {
        EndPoints = { $"{host}:{port}" },
        Password = password,
        Ssl = true,
        SslHost = host,
        AbortOnConnectFail = false,
        ConnectTimeout = 10000,
        SyncTimeout = 10000,
        KeepAlive = 30,
        ConnectRetry = 5
    };

    return ConnectionMultiplexer.Connect(options);
});
//Entity Framework + SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

//Auto Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error Migrating Database");
    }
}
// Init Database and Seed Admin Account
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAdminAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
