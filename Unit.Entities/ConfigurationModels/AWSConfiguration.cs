
namespace Unit.Entities.ConfigurationModels
{
    public class AWSConfiguration
    {
        public string Section { get; set; } = "AWS";
        public string? Region { get; set; }
        public string? AccessKey { get; set; }
        public string? SecretKey { get; set; }

        public CognitoConfiguration? Cognito { get; set; }
        public S3Configuration? S3Bucket { get; set; }
    }
}
