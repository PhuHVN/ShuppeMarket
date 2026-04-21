using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using ShuppeMarket.Application.Interfaces;


namespace ShuppeMarket.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            this.configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();

            //Sender
            email.From.Add(new MailboxAddress(
                configuration.GetValue<string>("Email:SenderName"),
                configuration.GetValue<string>("Email:SenderEmail")));

            //Receiver
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(
                    configuration.GetValue<string>("Email:SmtpServer"),
                    configuration.GetValue<int>("Email:SmtpPort"),
                    SecureSocketOptions.StartTls
                    );

                await smtp.AuthenticateAsync(
                    configuration.GetValue<string>("Email:Username"),
                    configuration.GetValue<string>("Email:Password")
                    );

                await smtp.SendAsync(email);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                throw new ArgumentException("Error sending mail: " + ex.Message);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendCodeOtpEmailAsync(string email, string otpCode)
        {
            try
            {
                var objective = "[ShuppeMarket] OTP Verification";
                var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>OTP Verification</title>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", sans-serif; background-color: #f5f5f5;'>
    <div style='background-color: #f5f5f5; padding: 20px; min-height: 100vh;'>
        <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);'>
            
            <!-- Header -->
            <div style='background: linear-gradient(135deg, #fb5d1f 0%, #f05c2c 100%); padding: 40px 20px; text-align: center;'>
                <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600; letter-spacing: 1px;'>ShuppeMarket</h1>
                <p style='color: rgba(255, 255, 255, 0.9); margin: 8px 0 0 0; font-size: 14px;'>Xác Minh Tài Khoản</p>
            </div>

            <!-- Content -->
            <div style='padding: 40px 30px; text-align: center;'>
                <h2 style='color: #222222; margin: 0 0 16px 0; font-size: 20px; font-weight: 600;'>Mã Xác Minh OTP</h2>
                
                <p style='color: #666666; font-size: 15px; line-height: 1.6; margin: 0 0 24px 0;'>
                    Chào mừng bạn đến với <strong style='color: #fb5d1f;'>ShuppeMarket</strong>!<br/>
                    Vui lòng sử dụng mã dưới đây để hoàn tất xác minh tài khoản của bạn.
                </p>

                <p style='color: #999999; font-size: 13px; margin: 0 0 30px 0;'>Mã này có hiệu lực trong <strong>5 phút</strong></p>

                <!-- OTP Code Box -->
                <div style='background-color: #fb5d1f; border-radius: 8px; padding: 20px; margin: 0 0 30px 0;'>
                    <code style='font-family: ""Monaco"", ""Courier New"", monospace; font-size: 40px; font-weight: 700; color: #ffffff; letter-spacing: 6px;'>{otpCode}</code>
                </div>

                <p style='color: #999999; font-size: 13px; line-height: 1.6; margin: 0;'>
                    Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này hoặc liên hệ với bộ phận hỗ trợ.
                </p>
            </div>

            <!-- Security Warning -->
            <div style='background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px 20px; margin: 0 30px 30px 30px; border-radius: 4px;'>
                <p style='color: #856404; font-size: 12px; margin: 0;'>
                    ⚠️ <strong>Lưu ý bảo mật:</strong> Không chia sẻ mã OTP này với bất kỳ ai. ShuppeMarket sẽ không bao giờ yêu cầu mã này qua email.
                </p>
            </div>

            <!-- Footer -->
            <div style='background-color: #f8f8f8; border-top: 1px solid #eeeeee; padding: 20px; text-align: center;'>
                <p style='color: #999999; font-size: 12px; margin: 0 0 8px 0;'>
                    &copy; 2026 ShuppeMarket. Tất cả các quyền được bảo lưu.
                </p>
                <p style='color: #cccccc; font-size: 11px; margin: 0;'>
                    Đây là tin nhắn tự động, vui lòng không trả lời email này.
                </p>
            </div>
        </div>
    </div>
</body>
</html>";
                await SendEmailAsync(email, objective, body);
                _logger.LogInformation("OTP email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP email to {Email}", email);
                throw new ArgumentException("Error sending OTP email: " + ex.Message);
            }
        }
    }

}
