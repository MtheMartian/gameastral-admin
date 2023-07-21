using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using GameStarBackend.Api.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Sprache;

namespace GameStarBackend.Api.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary cloudinary;

        public CloudinaryService(IOptions<CloudinaryGSSettings> cloudinaryGSSettings)
        {
            cloudinary = new Cloudinary(cloudinaryGSSettings.Value.CloudinaryUrl);
        }
        
        public async Task UploadImage(string fileInfo)
        {
            cloudinary.Api.Secure = true;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileInfo),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true,
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            Console.WriteLine(uploadResult.JsonObj);
        }
    }
}
