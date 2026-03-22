using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ShuppeMarket.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Application.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly ICloudinary _cloudinary; 

        public CloudinaryService(ICloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty", nameof(file));
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds the limit of 5MB", nameof(file));
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "products"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result == null || result.Error != null)
            {
                throw new Exception(result?.Error?.Message ?? "Unknown error occurred during Cloudinary upload");
            }
            return result.SecureUrl.ToString();
        }
    }
}
