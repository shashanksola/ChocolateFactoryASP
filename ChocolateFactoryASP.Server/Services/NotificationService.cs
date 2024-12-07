using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChocolateFactory.Services
{
    public class NotificationService
    {
        private readonly string _sendGridApiKey;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger)
        {
            _sendGridApiKey = configuration["SendGrid:ApiKey"];
            _logger = logger;

            if (string.IsNullOrEmpty(_sendGridApiKey))
            {
                _logger.LogError("SendGrid API key is not set.");
                throw new Exception("SendGrid API key is not set.");
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var client = new SendGridClient(_sendGridApiKey);
                var from = new EmailAddress("shashanksola1010@gmail.com", "Choco.co");
                var to = new EmailAddress(toEmail);
                var plainTextContent = message;
                var htmlContent = $"<strong>{message}</strong>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                // Log response for debugging or further error handling
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError($"Failed to send email to {toEmail}. Response: {response.StatusCode} - {responseBody}");
                    throw new Exception($"Failed to send email to {toEmail}. Response: {response.StatusCode} - {responseBody}");
                }

                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {toEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task SendEmailToMultipleAsync(IEnumerable<string> recipientEmails, string subject, string message)
        {
            var tasks = new List<Task>();

            foreach (var email in recipientEmails)
            {
                tasks.Add(SendEmailAsync(email, subject, message));
            }

            await Task.WhenAll(tasks);
        }
    }
}
