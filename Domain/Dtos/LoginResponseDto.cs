namespace GKTodoManager.Domain.Dtos;

public class LoginResponseDto
{
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsExternal { get; set; }
    public TokenDto Token { get; set; }
}
