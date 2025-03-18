using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.EntityFramework;

public static class QueryBuilderServicesExtensions
{
    public static IServiceCollection AddQueryBuilderServices<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        services.AddTransient<DefaultDynamicLinqCustomTypeProvider>(_ => new CustomEfTypeProvider(new ParsingConfig
        {
            RenameParameterExpression = true
        }, true));

        services.AddTransient<IExpressionStringifier, UpperSeparatorExpressionStringifier>();
        services.AddTransient<IOperatorFactory, EFOperatorFactory>();
        services.AddTransient<IQueryFactory, QueryFactory<TDbContext>>();
        return services;
    }
}