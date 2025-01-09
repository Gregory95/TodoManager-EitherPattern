using Microsoft.AspNetCore.Identity;
using GKTodoManager.Domain.Entities;
using GKTodoManager.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;


namespace GKTodoManager.Infrastructure.Repositories;

public class AuthRepository(UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager, ApplicationDbContext context) : IAuthRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> CanUserSignInAsync(ApplicationUser user)
    {
        return await _signInManager.CanSignInAsync(user);
    }

    public async Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> CreateUserAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded;
    }

    public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> FindUserByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<SignInResult> UserPasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        return await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
    }

    public async System.Threading.Tasks.Task SignOutUser(CancellationToken ct)
    {
        await _signInManager.SignOutAsync();
    }

    public async System.Threading.Tasks.Task UpdateUserLastLoginDate(string userId, DateTime lastLoginDate)
    {
        var user = await _userManager.FindByIdAsync(userId);

        user.LastLoginDate = lastLoginDate;

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task UpdateLoginFailedAttempt(ApplicationUser user)
    {
        await _userManager.AccessFailedAsync(user);
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task LockUserAccount(ApplicationUser user)
    {
        await _userManager.SetLockoutEnabledAsync(user, true);
        await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddHours(5));
        await _context.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task UnlockUserAccount(ApplicationUser user)
    {
        await _userManager.SetLockoutEnabledAsync(user, false);
        await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsAccountLock(ApplicationUser user)
    {
        return await _userManager.IsLockedOutAsync(user);
    }

    public async Task<string> DeleteUser(ApplicationUser user)
    {
        await _userManager.DeleteAsync(user);
        return user.Id;
    }
}
