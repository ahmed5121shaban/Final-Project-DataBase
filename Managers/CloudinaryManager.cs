using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Managers
{
    public class CloudinaryManager
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryManager(IConfiguration config)
        {
            var cloudName = config["Cloudinary:CloudName"];
            var apiKey = config["Cloudinary:ApiKey"];
            var apiSecret = config["Cloudinary:ApiSecret"];

            Account account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }

            return null;
        }
    }
}