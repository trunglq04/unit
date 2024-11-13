using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System.Text;
using Unit.Entities.Exceptions;
using Unit.Entities.Exceptions.Messages;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Repository.Extensions;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient) : base(dynamoDbContext, dynamoDbClient)
        {
        }

        public async Task CreateUserAsync(User user)
            => await CreateAsync(user);

        public async Task<User> GetUserAsync(string userId)
        {
            var user = await FindByIdAsync(userId);

            return user ?? throw new NotFoundException(UserExMsg.UserIsNotExist);
        }


        public async Task<PagedList<User>> GetUsersByIdsAsync(UserParameters userParameters, string[] ids)
        {

            var filterExpression = new StringBuilder();
            var expressionAttributeValues = new Dictionary<string, AttributeValue>();

            expressionAttributeValues[":isActive"] = new AttributeValue { N = "1" };
            for (int i = 0; i < ids.Length; i++)
            {
                var parameterName = $":UserId{i}";
                if (i > 0)
                {
                    filterExpression.Append(" OR ");
                }
                filterExpression.Append($"user_id = {parameterName} AND active = :isActive");
                expressionAttributeValues[parameterName] = new AttributeValue { S = ids[i] };
            }

            var users = await FindByConditionAsync(
                userParameters,
                filterExpression,
                expressionAttributeValues
            );
            var listUser = users.listEntity.Sort(userParameters.OrderBy).ToList();

            return new PagedList<User>(listUser, users.pageKey, userParameters.Size);
        }

        public async Task<PagedList<User>> GetUsersExceptAsync(UserParameters userParameters, string userId)
        {
            var filterExpression = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(userParameters.SearchTerm))
            {
                filterExpression.Append("contains(user_name, :searchTerm)");
            }

            if (filterExpression.Length > 0)
            {
                filterExpression.Append(" AND ");
            }
            filterExpression.Append("user_id <> :excludedUserId AND active = :isActive");


            var expressionAttributeValues = new Dictionary<string, AttributeValue>();

            if (!string.IsNullOrWhiteSpace(userParameters.SearchTerm))
            {
                expressionAttributeValues[":searchTerm"] = new AttributeValue { S = userParameters.SearchTerm };
            }

            expressionAttributeValues[":excludedUserId"] = new AttributeValue { S = userId };
            expressionAttributeValues[":isActive"] = new AttributeValue { N = "1" };


            var users = await FindByConditionAsync(
                userParameters,
                filterExpression,
                expressionAttributeValues
                );
            var listUser = users.listEntity.Sort(userParameters.OrderBy).ToList();


            return new PagedList<User>(listUser, users.pageKey, userParameters.Size);
        }

        public async Task UpdateUserAsync(User user)
        {
            await UpdateAsync(user);
        }
    }
}
