using Microsoft.AspNetCore.Http;

namespace ShuppeMarket.Application.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
