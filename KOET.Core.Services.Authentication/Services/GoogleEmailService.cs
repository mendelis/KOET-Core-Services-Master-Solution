using System.Net;
using System.Net.Mail;
using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Models;
using Microsoft.Extensions.Options;

public class GoogleEmailService : IEmailSender
{
    private readonly SmtpSettings _settings;

    public GoogleEmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;

    }

    public async Task SendConfirmationEmail(string to, string token)
    {
        string _template = File.ReadAllText("Utils/EmailTemplate.html");
        
        var body = _template.Replace("{{TOKEN}}", token);

        SendEmailAsync(to, "Confirmation Email", body);
        
        Console.WriteLine($"Sending email to {to}:\n{body}");
        
        await Task.CompletedTask;
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Server, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);
    }
}

