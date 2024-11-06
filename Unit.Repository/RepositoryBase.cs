using Amazon.DynamoDBv2.DataModel;
using System.Linq.Expressions;
using Unit.Repository.Contracts;

namespace Unit.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly IDynamoDBContext _context;

        public RepositoryBase(IDynamoDBContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<T>> FindAll()
        {
            return await _context.ScanAsync<T>(new List<ScanCondition>()).GetRemainingAsync();
        }


        public async Task<IEnumerable<T>> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return await _context.QueryAsync<T>(expression).GetRemainingAsync();
        }


        public async Task Create(T entity)
        {
            await _context.SaveAsync(entity);
        }


        public async Task Update(T entity)
        {
            await _context.SaveAsync(entity);
        }


        public async Task Delete(T entity)
        {
            await _context.DeleteAsync(entity);
        }
    }
}
