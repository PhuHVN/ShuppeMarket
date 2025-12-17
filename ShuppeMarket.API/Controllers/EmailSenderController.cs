using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShuppeMarket.Application.Interfaces;

namespace ShuppeMarket.API.Controllers
{
    [Route("api/v1/emailsender")]
    [ApiController]
    public class EmailSenderController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailSenderController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail(string toEmail, string subject, string body)
        {
            await _emailService.SendEmailAsync("hovanngocphu2004@gmail.com", "Test Email","Hello OTP email works!");
            return Ok(new { Message = "Email sent successfully" });
        }
    }
}
