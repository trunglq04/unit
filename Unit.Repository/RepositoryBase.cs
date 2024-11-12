using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unit.Shared.RequestFeatures;
using Unit.Repository.Extensions;
using System.Text;

namespace Unit.Repository
{
    public abstract class RepositoryBase<T>
    {
        protected readonly IDynamoDBContext _dynamoDbContext;
        protected readonly IAmazonDynamoDB _dynamoDbClient;

        protected RepositoryBase(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbContext = dynamoDbContext;
            _dynamoDbClient = dynamoDbClient;
        }

        public string GetTableName()
        {
            var tableAttribute = typeof(T).GetCustomAttribute<DynamoDBTableAttribute>();
            return tableAttribute!.TableName;
        }

        public string? FieldsBuilder(string? fields)
        {
            if (!string.IsNullOrWhiteSpace(fields))
            {
                var listFields = fields.Split(',');
                var dynamoDbFields = listFields.Select(field =>
                {
                    var property = typeof(T).GetProperty(field);
                    var attribute = property?.GetCustomAttributes(typeof(DynamoDBPropertyAttribute), false)
                                              .FirstOrDefault() as DynamoDBPropertyAttribute;
                    return attribute?.AttributeName ?? field;
                }).ToList();
                return string.Join(", ", dynamoDbFields);
            }
            return null;
        }

        public async Task<List<T>> FindAllAsync()
            => await _dynamoDbContext.ScanAsync<T>(new List<ScanCondition>()).GetRemainingAsync();

        public async Task<QueryResponse> FindByConditionAsync(QueryRequest query)
            => await _dynamoDbClient.QueryAsync(query);

        public async Task<(IEnumerable<T> listEntity, string pageKey)> FindByConditionAsync(RequestParameters request, StringBuilder? filterExpression = null, Dictionary<string, AttributeValue>? expressionAttributeValues = null)
        {
            var exlusiveStartKey = string.IsNullOrWhiteSpace(request.Page) ? null : JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(Convert.FromBase64String(request.Page));
            var scanRequest = new ScanRequest
            {
                TableName = GetTableName(),
                Limit = request.Size,
                ExclusiveStartKey = exlusiveStartKey,
                FilterExpression = filterExpression?.ToString(),
                ExpressionAttributeValues = expressionAttributeValues,
                ProjectionExpression = FieldsBuilder(request.Fields),
            };

            var response = await _dynamoDbClient.ScanAsync(scanRequest);

            var items = response.Items.Select(u =>
            {
                var doc = Document.FromAttributeMap(u);
                return _dynamoDbContext.FromDocument<T>(doc);
            }).ToList();

            var nextPageKey = response.LastEvaluatedKey.Count == 0 ? null : Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(response.LastEvaluatedKey, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }));

            return (items, nextPageKey);
        }


        public async Task<T> FindByIdAsync(object partitionKey)
            => await _dynamoDbContext.LoadAsync<T>(partitionKey);

        public async Task<T> FindByIdAsync(object partitionKey, object sortKey)
            => await _dynamoDbContext.LoadAsync<T>(partitionKey, sortKey);

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
