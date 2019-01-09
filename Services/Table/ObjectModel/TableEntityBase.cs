using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Azure.Storage;
using Services.Util;
using DateTime = System.DateTime;

namespace Services.Table.ObjectModel
{
    public class TableEntityBase : TableEntity
    {
        public TableEntityBase()
        {
            this.ID = SequentialGuid.NewGuid();
            this.CreatedAt = DateTime.UtcNow;

            PartitionKey = CreatedAt.ToString("yyyyMMddHH");
            RowKey = ID.ToString().ToLower();
        }

        public TableEntityBase(Guid id, DateTime? createdAt)
        {
            this.ID = id;
            this.CreatedAt = createdAt ?? DateTime.UtcNow;

            PartitionKey = CreatedAt.ToString("yyyyMMddHH");
            RowKey = ID.ToString().ToLower();
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var parentType = this.GetType();
            var result = base.WriteEntity(operationContext);
            var parentProperties = parentType.GetProperties().Where(t => t.DeclaringType == parentType &&
                                                                         !t.PropertyType.IsPrimitive &&
                                                                         t.PropertyType != typeof(string) &&
                                                                         t.PropertyType != typeof(DateTime) &&
                                                                         t.PropertyType != typeof(DateTime?) &&
                                                                         t.PropertyType != typeof(DateTimeOffset) &&
                                                                         t.PropertyType != typeof(DateTimeOffset?));

            foreach (var property in parentProperties)
            {
                var parentName = property.Name;
                var propertyInfos = property.PropertyType.GetProperties();
                var parent = property.GetValue(this);

                foreach (var propertyInfo in propertyInfos)
                {
                    object propertyValue = null;

                    if (parent != null)
                    {
                        propertyValue = propertyInfo.GetValue(parent);

                        result.Add($"{parentName}_{propertyInfo.Name}", propertyValue == null ? EntityProperty.GeneratePropertyForString(null) : EntityProperty.CreateEntityPropertyFromObject(propertyValue));
                    }
                }
            }

            return result;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            var parentType = this.GetType();
            var parentProperties = parentType.GetProperties().Where(t => t.DeclaringType == parentType && 
                                                                         !t.PropertyType.IsPrimitive &&
                                                                         t.PropertyType != typeof(string) &&
                                                                         t.PropertyType != typeof(DateTime) &&
                                                                         t.PropertyType != typeof(DateTime?) &&
                                                                         t.PropertyType != typeof(DateTimeOffset) &&
                                                                         t.PropertyType != typeof(DateTimeOffset?));

            foreach (var property in parentProperties)
            {
                var parentName = property.Name;
                var propertyInfos = property.PropertyType.GetProperties();
                var parent = Activator.CreateInstance(property.PropertyType);
                bool flagIsNull = false;

                foreach (var propertyInfo in propertyInfos)
                {
                    var storePropName = $"{parentName}_{propertyInfo.Name}";

                    if (properties.ContainsKey(storePropName))
                    {
                        flagIsNull = true;
                        var storePropValue = properties[storePropName];
                        propertyInfo.SetValue(parent, storePropValue.PropertyAsObject); 
                    }
                }

                if (flagIsNull)
                {
                    property.SetValue(this, parent);
                }
            }
        }

        public DateTime CreatedAt { get; set; }
        public Guid ID { get; set; }
    }
}
