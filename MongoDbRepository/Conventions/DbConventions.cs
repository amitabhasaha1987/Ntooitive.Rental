using System.Collections.Generic;
using MongoDB.Bson.Serialization.Conventions;

namespace Core.Conventions
{
    internal class DbConventions : IConventionPack
    {
        public IEnumerable<IConvention> Conventions
        {
            get
            {
                return new List<IConvention>
                {
                    { new IgnoreIfNullConvention(true) },
                    { new IgnoreExtraElementsConvention(true) },
                };
            }
        }
    }
}
