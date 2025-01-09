namespace GKTodoManager.Domain.Dtos;

public class EmailConfiguration
{
    public string Host { get; set; }
    public string SendEmailFrom { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public bool UseSecureConnection { get; set; }
}