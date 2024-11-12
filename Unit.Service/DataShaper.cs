using Amazon.DynamoDBv2.Model;
using System.Dynamic;
using System.Reflection;
using Unit.Service.Contracts;

namespace Unit.Service
{
    /*
     * SUMMARY:
     *  - DataShaper<T> is a generic class that implements the IDataShaper<T> interface.
     *  - The class is used to shape the data of a given entity by selecting only the required properties.
     */
    public class DataShaper<T> : IDataShaper<T> where T : class
    {
        // An array of PropertyInfo’s that we’re going to pull out of the input type
        public PropertyInfo[] Properties { get; set; }

        /* 
         * Ensuring that whenever a DataShaper<T> object is instantiated, T is a public class type
         * the Properties array is populated with the relevant property information for the type T.
        */
        public DataShaper()
        {
            // Get all public properties of the type T
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public ExpandoObject ShapeData(T entity, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);

            return FetchDataForEntity(entity, requiredProperties);
        }

        public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);

            return FetchData(entities, requiredProperties);
        }

        // fieldsString for each object example: "Id,Name,Description"
        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredProperties = new List<PropertyInfo>();

            if (!string.IsNullOrWhiteSpace(fieldsString))
            {
                var fields = fieldsString.Split(',',
               StringSplitOptions.RemoveEmptyEntries);

                // Check if the property name matches the input field then add to properties list
                foreach (var field in fields)
                {
                    var property = Properties
                    .FirstOrDefault(pi => pi.Name.Equals(field.Trim(),
                   StringComparison.InvariantCultureIgnoreCase));

                    if (property == null)
                        continue;

                    requiredProperties.Add(property);
                }
            }
            else
            {
                requiredProperties = Properties.ToList();
            }

            return requiredProperties;
        }

        private IEnumerable<ExpandoObject> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<ExpandoObject>();

            foreach (var entity in entities)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }

            return shapedData;
        }

        private ExpandoObject FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ExpandoObject();


            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entity);

                if (objectPropertyValue != null)
                shapedObject.TryAdd(property.Name, objectPropertyValue);
            }

            return shapedObject;
        }
    }
}

