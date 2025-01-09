namespace GKTodoManager.Application.Abstractions;


public interface IEmailSender
{
    Task<bool> SendEmailAsync(List<string> email, string subject, string body);
}
