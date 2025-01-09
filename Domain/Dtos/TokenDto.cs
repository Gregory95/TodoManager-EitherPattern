
namespace GKTodoManager.Domain.Dtos;

public class TokenDto
{
    public string AccessToken { get; set; }
    public long ExpiresIn { get; set; }
}