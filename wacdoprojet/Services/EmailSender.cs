using Azure;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace wacdoproject.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
           


            Console.WriteLine($"Tentative d'envoi d'email à {email} avec sujet : {subject}");
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["SendGrid:FromEmail"], "Wacdo gestion");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
           // await client.SendEmailAsync(msg);

          
            var response = await client.SendEmailAsync(msg);
            Console.WriteLine($"Code retour SendGrid : {response.StatusCode}");
            var responseBody = await response.Body.ReadAsStringAsync();
            Console.WriteLine($"Réponse SendGrid : {responseBody}");

            Console.WriteLine($"Code retour SendGrid : {response.StatusCode}");

            Console.WriteLine("Début SendEmailAsync");
            Console.WriteLine($"API Key ? {(string.IsNullOrEmpty(apiKey) ? "Non fournie" : "Ok")}");
            Console.WriteLine($"FromEmail = {_configuration["SendGrid:FromEmail"]}");
            Console.WriteLine($"ToEmail = {email}");

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("La clé API SendGrid est manquante !");
            }



        }
    }
}
