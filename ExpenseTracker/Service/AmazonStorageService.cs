using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ExpenseTracker.Dto;

namespace ExpenseTracker.Service
{
    public class AmazonStorageService : IImageStorageService
    {
        private readonly IAmazonS3 _s3Client;

        private const string bucketName = "imagebucket-32165465";
        public AmazonStorageService()
        {
            _s3Client = new AmazonS3Client();
        }

        public Task DeleteAsync(string storageKey)
        {
            throw new NotImplementedException();
        }

        public async Task<ImageUploadResponse> UploadAsync(Stream stream, string fileName)
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = $"images/{fileName}",
                BucketName = bucketName,
                ContentType = "image/jpeg",
                
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);


            ImageUploadResponse response = new ImageUploadResponse
            {
                Success = true,
                Url = $"images/{fileName}",
                ErrorMessage = null
            };
            return response;
        }
    }
}
