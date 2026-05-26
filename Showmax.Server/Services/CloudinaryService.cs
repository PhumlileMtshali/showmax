using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Showmax.Shared.DTOs;

namespace Showmax.Server.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<UploadResultDto> UploadImageAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "showmax/thumbnails",
                    Transformation = new Transformation()
                        .Width(300).Height(450).Crop("fill").Gravity("face")
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                    return new UploadResultDto { IsSuccess = false, Message = result.Error.Message };

                return new UploadResultDto
                {
                    IsSuccess = true,
                    Url = result.SecureUrl.ToString(),
                    PublicId = result.PublicId
                };
            }
            catch (Exception ex)
            {
                return new UploadResultDto { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<UploadResultDto> UploadVideoAsync(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "showmax/videos",
                    EagerTransforms = new List<Transformation>
                    {
                        new Transformation().Quality("auto").FetchFormat("mp4")
                    }
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.Error != null)
                    return new UploadResultDto { IsSuccess = false, Message = result.Error.Message };

                return new UploadResultDto
                {
                    IsSuccess = true,
                    Url = result.SecureUrl.ToString(),
                    PublicId = result.PublicId
                };
            }
            catch (Exception ex)
            {
                return new UploadResultDto { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<bool> DeleteFileAsync(string publicId)
        {
            try
            {
                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);
                return result.Result == "ok";
            }
            catch
            {
                return false;
            }
        }
    }
}