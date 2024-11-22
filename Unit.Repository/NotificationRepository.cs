using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Unit.Entities.Models;
using Unit.Repository.Contracts;

namespace Unit.Repository
{
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient) : base(dynamoDbContext, dynamoDbClient)
        {
        }

        public async Task CreateNotification(Notification notification)
            => await CreateAsync(notification);

        public async Task DeleteNotification(string ownerId, string createdAt)
            => await DeleteAsync(ownerId, createdAt);

        public async Task UpdateNotification(Notification notification)
            => await UpdateAsync(notification);
    }
}
