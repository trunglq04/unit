using Unit.Entities.Models;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository.Contracts
{
    public interface INotificationRepository
    {
        Task CreateNotification(Notification notification);

        Task DeleteNotification(string ownerId, string createdAt);

        Task UpdateNotification(Notification notification);

        Task<PagedList<Notification>> GetAllNotificationsOfUser(NotificationParameters request, string userId);

        Task<Notification> GetNotificationById(string userId, string createdAt);

        Task<PagedList<Notification>> GetNotificationsOfUser(NotificationParameters request, string userId, string affectedObjectId, string actionType, string lastestActionUserId);
    }
}
