using Amazon.CognitoIdentityProvider.Model;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime.Internal;
using Unit.Service.Contracts;

namespace Unit.Service
{
    public sealed class UserService : IUserService
    {
        private readonly ILoggerManager _logger;

        private readonly IDynamoDBContext _context;

        public UserService(IDynamoDBContext context, ILoggerManager logger)
        {
            _context = context;
            _logger = logger;
        }


    }
}
