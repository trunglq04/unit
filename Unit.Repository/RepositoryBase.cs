using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace Unit.Repository
{
    public abstract class RepositoryBase<T>
    {
        protected readonly IDynamoDBContext _dynamoDbContext;

        protected RepositoryBase(IDynamoDBContext dynamoDbContext)
        {
            _dynamoDbContext = dynamoDbContext;
        }

        public async Task<List<T>> FindAllAsync()
            => await _dynamoDbContext.ScanAsync<T>(new List<ScanCondition>()).GetRemainingAsync();

        public async Task<List<T>> FindByConditionAsync(List<ScanCondition> conditions)
            => await _dynamoDbContext.ScanAsync<T>(conditions).GetRemainingAsync();

        public async Task<T> FindByIdAsync(object partitionKey)
            => await _dynamoDbContext.LoadAsync<T>(partitionKey);

        public async Task<T> FindByIdAsync(object partitionKey, object sortKey)
            => await _dynamoDbContext.LoadAsync<T>(partitionKey, sortKey);

        public async Task<List<T>> FindByConditionAsync(object partitionKey, DynamoDBOperationConfig? config = null)
        {
            var query = _dynamoDbContext.QueryAsync<T>(partitionKey, config);
            return await query.GetRemainingAsync();
        }

        public async Task<List<T>> FindByConditionAsync(object partitionKey, QueryOperator op, List<object> listSortKey, DynamoDBOperationConfig? config = null)
        {
            var query = _dynamoDbContext.QueryAsync<T>(partitionKey, op, listSortKey, config);
            return await query.GetRemainingAsync();
        }

        public async Task CreateAsync(T entity)
            => await _dynamoDbContext.SaveAsync(entity);


        public async Task UpdateAsync(T entity)
            => await _dynamoDbContext.SaveAsync(entity);

        public async Task DeleteAsync(object partitionKey)
            => await _dynamoDbContext.DeleteAsync<T>(partitionKey);

        public async Task DeleteAsync(object partitionKey, object sortKey)
            => await _dynamoDbContext.DeleteAsync<T>(partitionKey, sortKey);
    }

}
