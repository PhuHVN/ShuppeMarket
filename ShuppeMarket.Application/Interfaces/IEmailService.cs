namespace ShuppeMarket.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendCodeOtpEmailAsync(string email, string otpCode);
    }
}
