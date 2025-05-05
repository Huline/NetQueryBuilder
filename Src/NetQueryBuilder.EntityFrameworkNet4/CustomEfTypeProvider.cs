using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace NetQueryBuilder.EntityFrameworkNet4
{
    public class CustomEfTypeProvider : DefaultDynamicLinqCustomTypeProvider
    {
        public CustomEfTypeProvider(ParsingConfig config, bool cache) : base(config, cache)
        {
        }

        public override HashSet<Type> GetCustomTypes()
        {
            var customTypes = base.GetCustomTypes();

            customTypes.Add(typeof(SqlFunctions));
            customTypes.Add(typeof(DbFunctions));

            return customTypes;
        }
    }
}