using Amazon.S3;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Unit.Entities.ConfigurationModels;
using Unit.Repository.Contracts;
using Unit.Service.Contracts;
using Unit.Shared.DataTransferObjects;

namespace Unit.Service
{
    public class PostService : IPostService
    {
        private readonly ILoggerManager _logger;

        private readonly IRepositoryManager _repository;

        private readonly IMapper _mapper;

        private readonly IDataShaper<PostDto> _postShaper;

        private readonly IAmazonS3 _s3Client;

        private readonly S3Configuration _s3Config;

        public PostService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IDataShaper<PostDto> postShaper, IAmazonS3 s3Client, IOptions<AWSConfiguration> configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _postShaper = postShaper;
            _s3Client = s3Client;
            _s3Config = configuration.Value.S3Bucket!;
        }
    }
}
