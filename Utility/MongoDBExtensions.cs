using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Utility
{
    public static class UpdateDefExtensions
    {
        public static UpdateDefinition<TDocument> SetIf<TDocument>(this UpdateDefinition<TDocument> builder, string field, object value, bool condition)
        {
            if (condition)
            {
                return builder.Set(field, value);
            }

            return builder;
        }
       
        public static UpdateDefinition<TDocument> SetIf<TDocument>(this UpdateDefinitionBuilder<TDocument> builder, string field, DateTime? value, bool condition)
        {
            if (condition)
            {
                return builder.Set(field, value);
            }

            return NoopUpdateDefinition<TDocument>.Instance;
        }
        public static UpdateDefinition<TDocument> SetIf<TDocument>(this UpdateDefinitionBuilder<TDocument> builder, string field, BsonDocument value, bool condition)
        {
            if (condition)
            {
                return builder.Set(field, value);
            }

            return NoopUpdateDefinition<TDocument>.Instance;
        }
        public static UpdateDefinition<TDocument> SetIf<TDocument>(this UpdateDefinitionBuilder<TDocument> builder, string field, bool value, bool condition)
        {
            if (condition)
            {
                return builder.Set(field, value);
            }

            return NoopUpdateDefinition<TDocument>.Instance;
        }

        public static UpdateDefinition<TDocument> SetIf<TDocument>(this UpdateDefinitionBuilder<TDocument> builder, string field, string  value, bool condition)
        {
            if (condition)
            {
                return builder.Set(field, value);
            }

            return NoopUpdateDefinition<TDocument>.Instance;
        }

        public static UpdateDefinition<TDocument> SetIf<TDocument>(this UpdateDefinitionBuilder<TDocument> builder, string field, BsonValue value, bool condition)
        {
            if (condition)
            {
                return builder.Set(field, value);
            }

            return NoopUpdateDefinition<TDocument>.Instance;
        }

        public static UpdateDefinition<TDocument> SetIf<TDocument>(this UpdateDefinitionBuilder<TDocument> builder, string field, double value, bool condition)
        {
            if (condition)
            {
                return builder.Set(field, value);
            }

            return NoopUpdateDefinition<TDocument>.Instance;
        }

        public static UpdateDefinition<TDocument> SetIf<TDocument>(this UpdateDefinitionBuilder<TDocument> builder, string field, int value, bool condition)
        {
            if (condition)
            {
                return builder.Set(field, value);
            }

            return NoopUpdateDefinition<TDocument>.Instance;
        }
        public sealed class NoopUpdateDefinition<TDocument> : UpdateDefinition<TDocument>
        {
            private static readonly NoopUpdateDefinition<TDocument> __instance = new NoopUpdateDefinition<TDocument>();

            private NoopUpdateDefinition()
            {
            }

            public static NoopUpdateDefinition<TDocument> Instance
            {
                get { return __instance; }
            }

            public override BsonDocument Render(IBsonSerializer<TDocument> documentSerializer, IBsonSerializerRegistry serializerRegistry)
            {
                return new BsonDocument();
            }
        }
    }
}
