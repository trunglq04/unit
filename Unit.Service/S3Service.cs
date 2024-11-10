using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;

namespace Unit.Service
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AWSConfiguration _awsConfig;

        public S3Service(IOptions<AWSConfiguration> awsOptions)
        {
            _awsConfig = awsOptions.Value;
            _s3Client = new AmazonS3Client(_awsConfig.AccessKey, _awsConfig.SecretKey, RegionEndpoint.GetBySystemName(_awsConfig.Region));
        }

        public async Task<string> UploadFileAsync(string bucketName, string filePath, Stream fileStream)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = filePath,
                InputStream = fileStream,
                ContentType = "application/octet-stream"
            };

            var response = await _s3Client.PutObjectAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK ? "Upload successful" : "Upload failed";
        }
    }
}
