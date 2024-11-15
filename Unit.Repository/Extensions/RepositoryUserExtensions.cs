using System.Reflection;
using System.Text;
using Unit.Entities.Models;
using System.Linq.Dynamic.Core;
using Unit.Repository.Extensions.Utility;

namespace Unit.Repository.Extensions
{
    public static class RepositoryUserExtensions
    {
        public static IEnumerable<User> Sort(this IEnumerable<User> users, 
            string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return users.OrderBy(e => e.UserName);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<User>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return users.OrderBy(e => e.UserName);
            return users.AsQueryable().OrderBy(orderQuery);
        }

        public static IEnumerable<Comment> Sort(this IEnumerable<Comment> comments, 
            string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return comments.OrderBy(e => e.CreatedAt);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Comment>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return comments.OrderBy(e => e.CreatedAt);
            return comments.AsQueryable().OrderBy(orderQuery);
        }
    }
}
