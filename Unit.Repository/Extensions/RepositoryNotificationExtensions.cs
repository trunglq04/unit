using System.Linq.Dynamic.Core;
using Unit.Entities.Models;
using Unit.Repository.Extensions.Utility;

namespace Unit.Repository.Extensions
{
    public static class RepositoryNotificationExtensions
    {
        public static IEnumerable<Notification> Sort(this IEnumerable<Notification> notifications,
            string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return notifications.OrderBy(c => c.CreatedAt);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Comment>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return notifications.OrderBy(c => c.CreatedAt);

            return notifications.AsQueryable().OrderBy(orderQuery);
        }
    }
}
