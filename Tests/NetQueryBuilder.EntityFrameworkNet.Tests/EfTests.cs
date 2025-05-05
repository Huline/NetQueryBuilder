using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.EntityFrameworkNet.Tests.Data;
using NetQueryBuilder.EntityFrameworkNet.Tests.Data.Models;
using NetQueryBuilder.EntityFrameworkNet4;
using NetQueryBuilder.Operators;
using Xunit;

namespace NetQueryBuilder.EntityFrameworkNet.Tests
{
    public class EfTests
    {
        private readonly MyDbContext _dbContext;
        private readonly IQueryConfigurator _queryConfigurator;

        public EfTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IQueryConfigurator, EfQueryConfigurator<MyDbContext>>();
            Database.SetInitializer(new TestDbInitializer());
            _dbContext = new MyDbContext();
            serviceCollection.AddScoped(_ => _dbContext);
            var provider = serviceCollection.BuildServiceProvider();
            _queryConfigurator = provider.GetRequiredService<IQueryConfigurator>();
        }

        [Fact]
        public async Task GetAll()
        {
            var query = _queryConfigurator.BuildFor<Person>();
            var results = (await query.Execute()).ToList();

            Assert.NotNull(results);
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task GetAddressFromPersonName()
        {
            var query = _queryConfigurator.BuildFor<Address>();
            query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Person.FirstName"), "Alice");
            var enumerable = await query.Execute();
            var results = enumerable.ToList();

            Assert.NotNull(results);
            Assert.Single(results);
        }
    }
}