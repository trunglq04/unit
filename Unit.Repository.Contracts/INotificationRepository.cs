using Unit.Entities.Models;

namespace Unit.Repository.Contracts
{
    public interface INotificationRepository
    {
        Task CreateNotification(Notification notification);

        Task DeleteNotification(string ownerId, string createdAt);

        Task UpdateNotification(Notification notification);
    }
}
