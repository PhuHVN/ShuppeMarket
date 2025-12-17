
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Services
{
    public class GenerateTokenService : IGenerateTokenService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<GenerateTokenService> logger;

        public GenerateTokenService(IConfiguration configuration, ILogger<GenerateTokenService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public string GenerateToken(Account accounts)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]);
                var role = accounts.Role.ToString();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, accounts.Id),
                        new System.Security.Claims.Claim(ClaimTypes.Email, accounts.Email),
                        new System.Security.Claims.Claim(ClaimTypes.Name, accounts.FullName),
                        new System.Security.Claims.Claim(ClaimTypes.Role, role),
                        new Claim("Status", accounts.Status.ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, accounts.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(configuration["Jwt:Expiration"])),
                    Issuer = configuration["Jwt:Issuer"],
                    Audience = configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating token");
                throw new Exception(ex.Message);

            }

        }

    }
}
