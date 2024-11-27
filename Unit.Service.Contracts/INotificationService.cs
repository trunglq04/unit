using Unit.Shared.DataTransferObjects.Notification;
using Unit.Shared.RequestFeatures;

namespace Unit.Service.Contracts
{
    public interface INotificationService
    {
        public Task DeleteNotificationById(string token, string createdAt);
      
        public Task UpdateNotificationById(string token, string createdAt);

        public Task<(List<NotificationDto> notificationDtos, MetaData metaData)> GetAllNotificationOfUser(NotificationParameters parameters, string token);


    }
}
