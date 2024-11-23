﻿using Unit.Entities.Models;
using Unit.Shared.RequestFeatures;

namespace Unit.Repository.Contracts
{
    public interface INotificationRepository
    {
        Task CreateNotification(Notification notification);

        Task DeleteNotification(string ownerId, string createdAt);

        Task UpdateNotification(Notification notification);

        Task<PagedList<Notification>> GetAllNotificationsOfUser(RequestParameters request, string userId);
    }
}
