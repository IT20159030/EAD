using System.Text;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Backend.Hubs;
using Backend.Models;
using Backend.Services;
using Backend.Services.notification;
using Backend.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

// MARK: - Authentication
var mongoDBIdentityConfiguration = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = builder.Configuration.GetConnectionString("MongoDB"),
        DatabaseName = new MongoUrl(builder.Configuration.GetConnectionString("MongoDB")).DatabaseName
    },
    IdentityOptionsAction = options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;

        options.User.RequireUniqueEmail = true;
    }
};

builder.Services.ConfigureMongoDbIdentity<User, ApplicationRole, Guid>(mongoDBIdentityConfiguration)
    .AddUserManager<UserManager<User>>()
    .AddSignInManager<SignInManager<User>>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Frontend:Url"],
        ValidAudience = builder.Configuration["Frontend:Url"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing"))),
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//MongoDB singleton service
builder.Services.AddSingleton<MongoDBService>();

// MARK: - Register MongoDB Collections
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoDB"));
    return new MongoClient(settings);
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(new MongoUrl(builder.Configuration.GetConnectionString("MongoDB")).DatabaseName);
    return database.GetCollection<Inventory>("Inventory");
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(new MongoUrl(builder.Configuration.GetConnectionString("MongoDB")).DatabaseName);
    return database.GetCollection<Notification>("Notification");
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(new MongoUrl(builder.Configuration.GetConnectionString("MongoDB")).DatabaseName);
    return database.GetCollection<Product>("Products");
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(new MongoUrl(builder.Configuration.GetConnectionString("MongoDB")).DatabaseName);
    return database.GetCollection<Order>("Orders");
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(new MongoUrl(builder.Configuration.GetConnectionString("MongoDB")).DatabaseName);
    return database.GetCollection<Notification>("Notification");
});

// MARK: - Register StockMonitoringService
builder.Services.AddSingleton<StockMonitoringService>();

// MARK: - Register UserMonitoringService
builder.Services.AddSingleton<UserMonitoringService>();

// MARK: - Register OrderMonitoringService
builder.Services.AddSingleton<OrderMonitoringService>();

// MARK: - SignalR Service
builder.Services.AddSignalR();

// Register hosted service for monitoring stock levels
builder.Services.AddHostedService<MonitoringWorker>();

//Services
builder.Services.AddScoped<WebUserAuthService>();
builder.Services.AddScoped<MobileUserAuthService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<StaffManagementService>();
builder.Services.AddScoped<CustomerManagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseRouting();

// Register NotificationHub
app.MapHub<NotificationHub>("/api/v1/notificationHub");

// Enable CORS
app.UseCors("AllowSpecificOrigin");
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
