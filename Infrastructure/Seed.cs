using GKTodoManager.Domain.Entities;
using GKTodoManager.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace GKTodoManager.Infrastructure;

public class Seed
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public static async System.Threading.Tasks.Task InitializeAsync(IServiceProvider services,
    IWebHostEnvironment hostingEnvironment,
    IConfiguration configuration)
    {
        using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        try
        {
            await CreateAdminRole(scope, configuration);
            await CreateAdminUser(scope, configuration);
            await CreateUserRole(scope, configuration);

            await UpdateTasksStatus(scope, configuration);
        }
        catch (Exception ex)
        {
            _logger.Error($"Error on Seed: {ex}");
        }
    }

    public static async System.Threading.Tasks.Task UpdateTasksStatus(IServiceScope scope, IConfiguration config)
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var updateTaskStatuses = await db.Tasks
            .Where(x => (x.Status == TaskStatusEnum.New || x.Status == TaskStatusEnum.Active)
                && x.EndDate < DateTime.UtcNow)
            .ExecuteUpdateAsync(x => x.SetProperty(b => b.Status, TaskStatusEnum.Expired));
    }

    public static async System.Threading.Tasks.Task CreateAdminUser(IServiceScope scope, IConfiguration config)
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await db.Users.Select(x => x.UserName).ContainsAsync("admin"))
        {
            const string userName = "admin";

            var user = new ApplicationUser
            {
                Email = userName,
                UserName = userName,
                NormalizedUserName = userName.ToUpper(),
                NormalizedEmail = userName.ToUpper(),
                FirstName = "System",
                LastName = "Admin",
                SecurityStamp = Guid.NewGuid().ToString(),
                Created = DateTime.UtcNow,
                HasFirstLogin = true,
                LockoutEnabled = true
            };

            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, config["ConnectionStrings:AdminPassword"]);

            await db.AddRangeAsync(user);
            await db.SaveChangesAsync();            
        }
    }

    public static async System.Threading.Tasks.Task CreateAdminRole(IServiceScope scope, IConfiguration config)
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await db.Roles.Select(x => x.Name).ContainsAsync("admin"))
        {
            const string role = "admin";

            var newRole = new ApplicationRole
            {
                Name = role,
                Description = "Administration role",
                IsAdmin = true,
                NormalizedName = role.ToUpper()
            };

            await db.AddRangeAsync(newRole);
            await db.SaveChangesAsync();
        }
    }

    public static async System.Threading.Tasks.Task CreateUserRole(IServiceScope scope, IConfiguration config)
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = await db.Users.FirstOrDefaultAsync(x => x.UserName == "admin");
        var role = await db.Roles.FirstOrDefaultAsync(x => x.Name == "admin");

        if (!await db.UserRoles.Select(x => x.UserId).ContainsAsync(user?.Id)
            && !await db.UserRoles.Select(x => x.RoleId).ContainsAsync(role?.Id))
        {
            var newUserRole = new ApplicationUserRole
            {
                RoleId = role.Id,
                UserId = user.Id
            };

            await db.AddRangeAsync(newUserRole);
            await db.SaveChangesLightAsync();
        }
    }
}