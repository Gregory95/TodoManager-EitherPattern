using System.Net;
using System.Net.Mail;
using NLog;
using GKTodoManager.Application.Abstractions;

namespace GKTodoManager.Application.External_Services;

public class EmailSender(IConfiguration configuration) : IEmailSender
{
    private readonly IConfiguration _configuration = configuration;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Task<bool> SendEmailAsync(List<string> email, string subject, string body)
    {
        return Execute(subject, body, email);
    }

    private Task<bool> Execute(string subject, string body, List<string> email)
    {
        try
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("gko@donotreply.com");
                mail.To.Add(email[0]);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                // mail.Attachments.Add(new Attachment());

                using SmtpClient smtp = new(_configuration["EmailConfiguration:host"], 587);
                smtp.Credentials = new NetworkCredential(_configuration["EmailConfiguration:username"], _configuration["EmailConfiguration:password"]);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return Task.FromResult<bool>(true);
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            return Task.FromResult<bool>(false);
        }
    }
}
