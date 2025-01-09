using GKTodoManager.Domain.Base;
using GKTodoManager.Domain.Dtos;

namespace GKTodoManager.Application.Abstractions;

public interface IAuthService
{
    Task<Either<TokenDto, ErrorResponse>> GetAccessTokenAsync(LoginRequestDto userName);
    Task<Either<RegisterUserResponseDto, ErrorResponse>> RegisterNewUserAsync(RegisterUserRequestDto message);
    Task<Either<LoginResponseDto, ErrorResponse>> LoginUserAsync(LoginRequestDto message);
    Task<Either<bool, ErrorResponse>> UnlockUserAsync(string userName);
    Task<Either<string, ErrorResponse>> DeleteUserAsync(string userId);
}
