using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System.Text;
using Unit.Entities.Models;
using Unit.Repository.Contracts;
using Unit.Repository.Extensions;
using Unit.Shared.RequestFeatures;

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

        public async Task<PagedList<Notification>> GetAllNotificationsOfUser(NotificationParameters request, string userId)
        {
            var keyConditionExpression = new StringBuilder();

            keyConditionExpression.Append(" owner_id = :owner_id");

            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":owner_id", new AttributeValue { S = userId } },
            };

            var notifications = await FindByConditionAsync(
                requestParameters: request,
                keyConditionExpression: keyConditionExpression,
                expressionAttributeValues: expressionAttributeValues
                );
            var listNotifications = notifications
                .listEntity
                .Sort(request.OrderBy)
                .Skip((request.PageNumber - 1) * request.Size)
                .Take(request.Size)
                .ToList();

            return new PagedList<Notification>(listNotifications, notifications.pageKey, request.Size, request.PageNumber);
        }

        public async Task<Notification> GetNotificationById(string userId, string createdAt)
        {
            return await FindByIdAsync(userId, DateTime.Parse(createdAt));
        }
    }
}
