using System.Linq.Dynamic.Core;
using Unit.Entities.Models;
using Unit.Repository.Extensions.Utility;

namespace Unit.Repository.Extensions
{
    public static class RepositoryPostExtensions
    {
        public static IEnumerable<Post> Sort(this IEnumerable<Post> posts, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return posts.OrderByDescending(e => e.CreatedAt);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<User>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return posts.OrderBy(e => e.CreatedAt);
            return posts.AsQueryable().OrderBy(orderQuery);
        }
    }
}
