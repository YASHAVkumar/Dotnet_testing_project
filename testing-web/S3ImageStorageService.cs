using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace testing_web;

public class S3ImageStorageService(
    IAmazonS3 s3,
    IConfiguration configuration) : IImageStorageService
{
    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        string folder)
    {
        var bucketName =
            configuration["S3:BucketName"];

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new InvalidOperationException(
                "S3 bucket name is not configured.");
        }

        //var extension =
        //    Path.GetExtension(fileName);

        var newFileName =
            $"{fileName}";

        var key =
            $"{folder}/{newFileName}";

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType
        };

        await s3.PutObjectAsync(request);

        return key;
    }

    public async Task<string> GetUrlAsync(
        string imagePath)
    {
        var bucketName =
            configuration["S3:BucketName"];

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new InvalidOperationException(
                "S3 bucket name is not configured.");
        }

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = imagePath,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };

        return await s3.GetPreSignedURLAsync(request);
    }

    public async Task DeleteAsync(
        string imagePath)
    {
        var bucketName =
            configuration["S3:BucketName"];

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            throw new InvalidOperationException(
                "S3 bucket name is not configured.");
        }

        var request = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = imagePath
        };

        await s3.DeleteObjectAsync(request);
    }
}
