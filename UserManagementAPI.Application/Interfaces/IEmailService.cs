namespace UserManagementAPI.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string email, string fullName);
        Task SendPasswordResetEmailAsync(string email, string resetToken);
    }
}
