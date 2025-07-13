using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Infrastructure.Services
{
    public class BackgroundJobService
    {
        private readonly ILogger<BackgroundJobService> _logger;

        public BackgroundJobService(ILogger<BackgroundJobService> logger)
        {
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string email, string firstName)
        {
            _logger.LogInformation("Sending welcome email to {Email} for {FirstName}", email, firstName);

            await Task.Delay(2000);

            _logger.LogInformation("Welcome email sent successfully to {Email}", email);
        }
    }
}
