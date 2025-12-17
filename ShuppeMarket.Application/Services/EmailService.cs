using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using ShuppeMarket.Application.Interfaces;


namespace ShuppeMarket.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending mail: " + ex.Message);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
