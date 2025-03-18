using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.EntityFramework.Operators;

namespace NetQueryBuilder.EntityFramework;

public class CustomEfTypeProvider : DefaultDynamicLinqCustomTypeProvider
{
    public CustomEfTypeProvider(ParsingConfig config, bool cache) : base(config, cache)
    {
    }

    public override HashSet<Type> GetCustomTypes()
    {
        var customTypes = base.GetCustomTypes();

        customTypes.Add(typeof(EF));
        customTypes.Add(typeof(DbFunctions));
        customTypes.Add(typeof(DbFunctionsExtensions));
        customTypes.Add(typeof(EfLikeOperator));

        return customTypes;
    }
}