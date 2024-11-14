using Unit.Entities.Models;
using System.Linq.Dynamic.Core;
using Unit.Repository.Extensions.Utility;

namespace Unit.Repository.Extensions
{
    public static class RepositoryUserExtensions
    {
        public static IEnumerable<User> Sort(this IEnumerable<User> users, string?
orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return users.OrderBy(e => e.UserName);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<User>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return users.OrderBy(e => e.UserName);
            return users.AsQueryable().OrderBy(orderQuery);
        }
    }
}
