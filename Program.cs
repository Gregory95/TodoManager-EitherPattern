using GKTodoManager.API.Middleware;
using GKTodoManager.Application.Abstractions;
using GKTodoManager.Application.External_Services;
using GKTodoManager.Application.Services;
using GKTodoManager.Domain.Abstractions;
using GKTodoManager.Domain.Dtos;
using GKTodoManager.Domain.Entities;
using GKTodoManager.Infrastructure;
using GKTodoManager.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
var logger = new NLog.Config.LoggingConfiguration();
IWebHostEnvironment host = null;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
    options.UseOpenIddict();
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(5);
    
    options.Password.RequireDigit = true;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/\\";
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddOpenIddict()
        // Register the OpenIddict core components.
        // Configure OpenIddict to use the Entity Framework Core stores and models.
        // Note: call ReplaceDefaultEntities() to replace the default entities.
        .AddCore(options =>
        {
            options
                .UseEntityFrameworkCore()
                .UseDbContext<ApplicationDbContext>();
        })
        // Register the OpenIddict server components.
        .AddServer(options =>
        {
            options.SetTokenEndpointUris("/token");
            options.AllowClientCredentialsFlow();
            options.AllowPasswordFlow();
            options.AcceptAnonymousClients();
            options.SetAccessTokenLifetime(TimeSpan.FromHours(6));
            options.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate();
            // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
            options
                .UseAspNetCore()
                .EnableTokenEndpointPassthrough()
                .DisableTransportSecurityRequirement();
        })
        .AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseAspNetCore();
        });

builder.Services.AddCors(
    options => options.AddPolicy("AllowCors",
    builder =>
    {
        builder.SetIsOriginAllowed(origin => true)
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod();
    })
);

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddTransient<IAuthRepository, AuthRepository>();
builder.Services.AddTransient<ITaskRepository, TaskRepository>();
builder.Services.AddTransient<ITaskGroupRepository, TaskGroupRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskGroupService, TaskGroupService>();

var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

Console.WriteLine("App init..");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseRouting();

// Add middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseMiddleware<LoggerMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();

    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

    if (pendingMigrations.Any())
    {
        Console.WriteLine($"You have {pendingMigrations.Count()} pending migrations to apply.");
        Console.WriteLine("Applying pending migrations now");
        await context.Database.MigrateAsync();
    }

    var lastAppliedMigration = (await context.Database.GetAppliedMigrationsAsync()).Last();

    Console.WriteLine($"You're on schema version: {lastAppliedMigration}");
}

Seed.InitializeAsync(app.Services, host, configuration).GetAwaiter().GetResult();

app.Run();
