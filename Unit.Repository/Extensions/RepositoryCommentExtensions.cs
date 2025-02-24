using Unit.Entities.Models;
using Unit.Repository.Extensions.Utility;
using System.Linq.Dynamic.Core;

namespace Unit.Repository.Extensions
{
    public static class RepositoryCommentExtensions
    {
        public static IEnumerable<Comment> Sort(this IEnumerable<Comment> comments,
            string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return comments.OrderBy(c => c.CreatedAt);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Comment>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return comments.OrderBy(c => c.CreatedAt);

            return comments.AsQueryable().OrderBy(orderQuery);
        }
    }
}
