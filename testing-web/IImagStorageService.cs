using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace testing_web
{
    public interface IImageStorageService
    {
        Task<string> UploadAsync(
            Stream stream,
            string fileName,
            string contentType,
            string folder);

        Task<string> GetUrlAsync(
            string imagePath);

        Task DeleteAsync(
            string imagePath);
    }
}
