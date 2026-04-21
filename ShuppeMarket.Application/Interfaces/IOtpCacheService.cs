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
