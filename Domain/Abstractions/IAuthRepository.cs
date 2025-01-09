using GKTodoManager.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GKTodoManager.Domain.Abstractions;

public interface IAuthRepository
{
    Task<bool> CanUserSignInAsync(ApplicationUser user);
    Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password);
    Task<bool> CreateUserAsync(ApplicationUser user, string password);
    Task<ApplicationUser?> FindUserByEmailAsync(string email);
    Task<ApplicationUser?> FindUserByNameAsync(string userName);
    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    Task<SignInResult> UserPasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
    System.Threading.Tasks.Task SignOutUser(CancellationToken ct);
    System.Threading.Tasks.Task UpdateUserLastLoginDate(string userId, DateTime lastLoginDate);
    System.Threading.Tasks.Task UpdateLoginFailedAttempt(ApplicationUser user);
    System.Threading.Tasks.Task LockUserAccount(ApplicationUser user);
    System.Threading.Tasks.Task UnlockUserAccount(ApplicationUser user);
    Task<bool> IsAccountLock(ApplicationUser user);
    Task<string> DeleteUser(ApplicationUser user);
}