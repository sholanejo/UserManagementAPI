using Microsoft.Extensions.Logging;
using UserManagementAPI.Application.Interfaces;

namespace UserManagementAPI.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string email, string fullName)
        {
            _logger.LogInformation("Sending welcome email to {Email} for {FullName}", email, fullName);

            await Task.Delay(1000);

            _logger.LogInformation("Welcome email sent successfully to {Email}", email);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetToken)
        {
            _logger.LogInformation("Sending password reset email to {Email}", email);

            await Task.Delay(1000);

            _logger.LogInformation("Password reset email sent successfully to {Email}", email);
        }
    }
}
