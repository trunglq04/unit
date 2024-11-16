using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Unit.Shared.RequestFeatures;
using System.Text;

namespace Unit.Repository
{
    public abstract class RepositoryBase<T>
    {
        protected readonly IDynamoDBContext _dynamoDbContext;
        protected readonly IAmazonDynamoDB _dynamoDbClient;

        private static readonly Dictionary<string, string?> PropertyCache = typeof(T)
            .GetProperties()
            .ToDictionary(
                prop => prop.Name.ToLower(),
                prop => prop.GetCustomAttributes(typeof(DynamoDBPropertyAttribute), false)
                            .FirstOrDefault() is DynamoDBPropertyAttribute attribute ? attribute.AttributeName : null
            );

        protected RepositoryBase(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbContext = dynamoDbContext;
            _dynamoDbClient = dynamoDbClient;
        }


        public string? FieldsBuilder(string? fields)
        {
            if (!string.IsNullOrWhiteSpace(fields))
            {
                var listFields = fields.ToLower().Split(',');

                var dynamoDbFields = listFields.Select(field =>
                {
                    return PropertyCache.ContainsKey(field) ? PropertyCache[field] : null;
                })
                .Where(attrName => attrName != null)
                .ToList();

                if (dynamoDbFields.Count == 0)
                {
                    return null;
                }

                return string.Join(", ", dynamoDbFields);
            }

            return null;
        }

        private string GetTableName()
        {
            var tableAttribute = typeof(T).GetCustomAttribute<DynamoDBTableAttribute>();
            return tableAttribute!.TableName;
        }

        public async Task<List<T>> FindAllAsync()
            => await _dynamoDbContext.ScanAsync<T>(new List<ScanCondition>()).GetRemainingAsync();


        public async Task<(IEnumerable<T> listEntity, string pageKey)> FindByConditionAsync(RequestParameters request, StringBuilder keyConditionExpression, StringBuilder? filterExpression = null, Dictionary<string, AttributeValue>? expressionAttributeValues = null)
        {
            var exlusiveStartKey = string.IsNullOrWhiteSpace(request.Page) ? null : JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(Convert.FromBase64String(request.Page));

            var queryRequest = new QueryRequest
            {
                TableName = GetTableName(),
                Limit = request.Size,
                ExclusiveStartKey = exlusiveStartKey,
                KeyConditionExpression = keyConditionExpression.ToString(),
                FilterExpression = filterExpression?.ToString(),
                ExpressionAttributeValues = expressionAttributeValues,
                ProjectionExpression = FieldsBuilder(request.Fields),
            };

            var response = await _dynamoDbClient.QueryAsync(queryRequest);

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

        public async Task<(IEnumerable<T> listEntity, string pageKey)> FindByConditionAsync(
            RequestParameters request, 
            StringBuilder? filterExpression = null, 
            Dictionary<string, AttributeValue>? expressionAttributeValues = null)
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
