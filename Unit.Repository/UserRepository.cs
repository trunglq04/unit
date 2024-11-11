using Amazon.DynamoDBv2.DataModel;
using Unit.Entities.Models;
using Unit.Repository.Contracts;

namespace Unit.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(IDynamoDBContext dynamoDbContext) : base(dynamoDbContext)
        {
        }

        public async Task CreateUserAsync(User user)
            => await CreateAsync(user);
    }
}
