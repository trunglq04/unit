using System.Linq.Dynamic.Core;
using Unit.Entities.Models;
using Unit.Repository.Extensions.Utility;

namespace Unit.Repository.Extensions
{
    public static class RepositoryCommentExtension
    {
        public static IEnumerable<Comment> Sort(this IEnumerable<Comment> comments, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return comments.OrderByDescending(e => e.CreatedAt);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<User>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return comments.OrderBy(e => e.CreatedAt);
            return comments.AsQueryable().OrderBy(orderQuery);
        }
    }
}
