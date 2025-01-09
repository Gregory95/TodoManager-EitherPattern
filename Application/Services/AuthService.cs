using AutoMapper;
using GKTodoManager.Application.Abstractions;
using GKTodoManager.Domain.Abstractions;
using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Dtos;
using GKTodoManager.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GKTodoManager.Application.Services;

public class AuthService(IAuthRepository authRepository, IMapper mapper, IConfiguration configuration, IEmailSender emailSender) : IAuthService
{
    private readonly IAuthRepository _authRepository = authRepository;
    private readonly IMapper _mapper = mapper;
    private readonly IConfiguration _configuration = configuration;
    private readonly IEmailSender _emailSender = emailSender;

    private const int _maxFailedLoginAttempts = 5;

    public async Task<Either<TokenDto, ErrorResponse>> GetAccessTokenAsync(LoginRequestDto message)
    {
        var user = await _authRepository.FindUserByNameAsync(message.UserName);

        if (user == null)
        {
            return Either<TokenDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "User not found.",
                Description = "Given user name does not exist in the system",
                Status = HttpStatusCode.NotFound
            });
        }

        if (!await _authRepository.CheckUserPasswordAsync(user, message.Password))
        {
            await _authRepository.UpdateLoginFailedAttempt(user);

            return Either<TokenDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Invalid credentials.",
                Description = "Username or password and not valid.",
                Status = HttpStatusCode.BadRequest
            });
        }

        var userRoles = await _authRepository.GetUserRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GetToken(authClaims);

        return Either<TokenDto, ErrorResponse>.Succeeded(new TokenDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn = new DateTimeOffset(token.ValidTo).ToUnixTimeMilliseconds()
        });
    }

    public async Task<Either<LoginResponseDto, ErrorResponse>> LoginUserAsync(LoginRequestDto message)
    {
        var user = await _authRepository.FindUserByNameAsync(message.UserName);

        if (user == null)
        {
            return Either<LoginResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "User not found.",
                Description = "Given user name does not exist in the system",
                Status = HttpStatusCode.NotFound
            });
        }

        if (await _authRepository.IsAccountLock(user))
        {
            return Either<LoginResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Account Locked",
                Description = "Your account is locked due to multiple failed login attempts. Please contact the system administrator.",
                Status = HttpStatusCode.BadRequest
            });
        }

        if (!await _authRepository.CanUserSignInAsync(user))
        {
            return Either<LoginResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Account not confirmed",
                Description = "A confirmation email had been sent to you to confirm your account",
                Status = HttpStatusCode.BadRequest
            });
        }

        if (!await _authRepository.CheckUserPasswordAsync(user, message.Password))
        {
            await _authRepository.UpdateLoginFailedAttempt(user);

            return Either<LoginResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Invalid Credentials",
                Description = "Username or password combination is not correct.",
                Status = HttpStatusCode.BadRequest
            });
        }

        var userRoles = await _authRepository.GetUserRolesAsync(user);

        var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var signInResult = await _authRepository.UserPasswordSignInAsync(message.UserName, message.Password, true, lockoutOnFailure: false);

        if (signInResult.RequiresTwoFactor)
        {
            return Either<LoginResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Two factor authentication required.",
                Description = "Enter the code generated by your authenticator app.",
                Status = HttpStatusCode.BadRequest
            });
        }

        var token = GetToken(authClaims);

        await _authRepository.UpdateUserLastLoginDate(user.Id, DateTime.UtcNow);

        return Either<LoginResponseDto, ErrorResponse>.Succeeded(new LoginResponseDto
        {
            UserName = message.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsExternal = string.IsNullOrEmpty(user.PasswordHash),
            Token = new TokenDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = new DateTimeOffset(token.ValidTo).ToUnixTimeMilliseconds()
            }
        });
    }

    public async Task<Either<RegisterUserResponseDto, ErrorResponse>> RegisterNewUserAsync(RegisterUserRequestDto message)
    {
        if (await _authRepository.FindUserByEmailAsync(message.Email) != null 
            || await _authRepository.FindUserByNameAsync(message.UserName) != null)
        {
            return Either<RegisterUserResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "User already exists",
                Description = "An other user with the same username/email already exists in the system.",
                Status = HttpStatusCode.BadRequest
            });
        }

        var newUser = new ApplicationUser
        {
            UserName = message.UserName,
            NormalizedUserName = message.UserName.ToUpper(),
            FirstName = message.FirstName,
            LastName = message.LastName,
            Email = message.Email,
            NormalizedEmail = message.Email.ToUpper(),
            Created = DateTime.UtcNow,
            HasFirstLogin = false,
            IsDeleted = false,
            PhoneNumber = message.PhoneNumber,
            SecurityStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = false
        };

        if (!await _authRepository.CreateUserAsync(newUser, message.Password))
        {
            return Either<RegisterUserResponseDto, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "New user registration failed",
                Description = "Make sure that all mandatory fields are filled and password satisfies the security policy.",
                Status = HttpStatusCode.BadRequest
            });
        }

        var emailList = new List<string>
        {
            newUser.Email
        };

        string link = $"{_configuration["ConnectionStrings:Uri"]}/login";

        var emailBody = String.Format(@"<h1 style='text-align: center'>Welcome to GKTodoManager!</h1>
                    <br><br><h3 style='text-align: center'>Manage your Tasks like a Pro.  
                    <br>Be responsible!</h3>
                    <br><br><a style='text-align: center;background-color: white;color: black;text-decorator: none;' href={0}>Login</a>", link);

        var emailSend = _emailSender.SendEmailAsync(emailList, "New user", emailBody);
        emailSend.Wait();

        return Either<RegisterUserResponseDto, ErrorResponse>.Succeeded(_mapper.Map<RegisterUserResponseDto>(newUser));
    }

    public async Task<Either<bool, ErrorResponse>> UnlockUserAsync(string userName)
    {
        var user = await _authRepository.FindUserByNameAsync(userName);

        if (user == null)
        {
            return Either<bool, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "User not found.",
                Description = "Given user name does not exist in the system",
                Status = HttpStatusCode.NotFound
            });
        }

        if (!await _authRepository.IsAccountLock(user))
        {
            return Either<bool, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "Account is not Locked",
                Description = "Your account is not locked",
                Status = HttpStatusCode.BadRequest
            });
        }

        try
        {
            await _authRepository.UnlockUserAccount(user);

            return Either<bool, ErrorResponse>.Succeeded(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Either<bool, ErrorResponse>.Succeeded(false);
        }
    }

    public async Task<Either<string, ErrorResponse>> DeleteUserAsync(string userName)
    {
        var user = await _authRepository.FindUserByNameAsync(userName);

        if (user == null)
        {
            return Either<string, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "User not found.",
                Description = "Given user name does not exist in the system",
                Status = HttpStatusCode.NotFound
            });
        }

        try
        {
            await _authRepository.DeleteUser(user);

            return Either<string, ErrorResponse>.Succeeded(user.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            return Either<string, ErrorResponse>.Failure(new ErrorResponse
            {
                Title = "User can not be deleted.",
                Description = $"Error: {ex.Message}",
                Status = HttpStatusCode.BadRequest
            });
        }
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        return token;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512(passwordSalt))
        {
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(passwordHash);
        }
    }
}