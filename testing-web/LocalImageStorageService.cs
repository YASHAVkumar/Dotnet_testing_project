using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace testing_web
{
    public class LocalImageStorageService(
        IWebHostEnvironment environment) : IImageStorageService
    {
        public async Task<string> UploadAsync(
            Stream stream,
            string fileName,
            string contentType,
            string folder)
        {
            var imageFolder = Path.Combine(
                environment.WebRootPath,
                folder);

            Directory.CreateDirectory(imageFolder);

            var newFileName =
                $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

            var filePath = Path.Combine(
                imageFolder,
                newFileName);

            await using var fileStream = new FileStream(
                filePath,
                FileMode.Create);

            await stream.CopyToAsync(fileStream);

            return $"/{folder}/{newFileName}";
        }

        public Task<string> GetUrlAsync(
            string imagePath)
        {
            return Task.FromResult(imagePath);
        }

        public Task DeleteAsync(
            string imagePath)
        {
            var relativePath = imagePath.TrimStart('/');

            var filePath = Path.Combine(
                environment.WebRootPath,
                relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }
    }
}