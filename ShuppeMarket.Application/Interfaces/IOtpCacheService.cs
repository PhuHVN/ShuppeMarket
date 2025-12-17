using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Interfaces
{
    public interface IOtpCacheService
    {
        Task StoreOtpAsync(string key, string otp, TimeSpan expiration);
        Task<string?> RetrieveOtpAsync(string key);
        Task RemoveOtpAsync(string key);
        string GenerateOTP();
    }
}
