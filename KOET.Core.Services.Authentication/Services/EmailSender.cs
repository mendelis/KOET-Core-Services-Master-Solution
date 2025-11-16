using KOET.Core.Services.Authentication.Contracts;

namespace KOET.Core.Services.Authentication.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _template;

        public EmailSender()
        {
            _template = File.ReadAllText("Utils/EmailTemplate.html");
        }

        public async Task SendConfirmationEmail(string to, string token)
        {
            var body = _template.Replace("{{TOKEN}}", token);
            Console.WriteLine($"Sending email to {to}:\n{body}");
            await Task.CompletedTask;
        }
    }
}
