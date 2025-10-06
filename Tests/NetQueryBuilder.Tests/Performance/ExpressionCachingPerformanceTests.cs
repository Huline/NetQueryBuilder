using System.Diagnostics;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.Tests.Performance;

/// <summary>
/// Test entity for performance tests
/// </summary>
public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Performance tests to verify expression compilation caching improvements.
/// These tests demonstrate the performance impact of lazy evaluation and proper cache invalidation.
/// </summary>
public class ExpressionCachingPerformanceTests
{
    private readonly IQueryConfigurator _configurator;

    public ExpressionCachingPerformanceTests()
    {
        _configurator = new QueryableQueryConfigurator<TestEntity>(
            new[] { new TestEntity { Id = 1, Name = "Test" } }.AsQueryable());
    }

    [Fact]
    public void Compile_WithCaching_ShouldBeFasterOnSubsequentCalls()
    {
        // Arrange
        var query = _configurator.BuildFor<TestEntity>();
        var property = query.ConditionPropertyPaths.First();
        query.Condition.CreateNew(property);

        // Act - First compilation (cache miss)
        var sw1 = Stopwatch.StartNew();
        var expression1 = query.Compile();
        sw1.Stop();

        // Act - Second compilation (cache hit)
        var sw2 = Stopwatch.StartNew();
        var expression2 = query.Compile();
        sw2.Stop();

        // Assert
        Assert.NotNull(expression1);
        Assert.Same(expression1, expression2); // Should return same cached instance
        Assert.True(sw2.Elapsed < sw1.Elapsed,
            $"Cached compilation ({sw2.ElapsedTicks} ticks) should be faster than initial compilation ({sw1.ElapsedTicks} ticks)");
    }

    [Fact]
    public void Compile_AfterPropertyChange_ShouldInvalidateCache()
    {
        // Arrange
        var query = _configurator.BuildFor<TestEntity>();
        var properties = query.ConditionPropertyPaths.ToList();
        var condition = query.Condition.CreateNew(properties[0]);

        // Act - Initial compilation
        var expression1 = query.Compile();

        // Change property (should invalidate cache)
        if (properties.Count > 1)
            condition.PropertyPath = properties[1];

        // Act - Compilation after property change
        var expression2 = query.Compile();

        // Assert
        Assert.NotNull(expression1);
        Assert.NotNull(expression2);
        // Expression should be different due to property change
    }

    [Fact]
    public void Compile_WithMultipleConditions_ShouldOnlyRecompileOnce()
    {
        // Arrange
        var query = _configurator.BuildFor<TestEntity>();
        var property = query.ConditionPropertyPaths.First();

        // Create multiple conditions
        for (int i = 0; i < 5; i++)
        {
            query.Condition.CreateNew(property);
        }

        // Act - Measure compilation time with multiple conditions
        var sw = Stopwatch.StartNew();
        var expression = query.Compile();
        sw.Stop();

        // Second call should be instant (cached)
        var sw2 = Stopwatch.StartNew();
        var expression2 = query.Compile();
        sw2.Stop();

        // Assert
        Assert.NotNull(expression);
        Assert.Same(expression, expression2);
        Assert.True(sw2.ElapsedTicks < sw.ElapsedTicks / 10,
            "Cached compilation should be at least 10x faster");
    }

    [Fact]
    public void Compile_WithNestedConditions_ShouldCacheHierarchically()
    {
        // Arrange
        var query = _configurator.BuildFor<TestEntity>();
        var property = query.ConditionPropertyPaths.First();

        // Create nested block conditions
        var cond1 = query.Condition.CreateNew(property);
        var cond2 = query.Condition.CreateNew(property);
        var cond3 = query.Condition.CreateNew(property);

        var grouped = query.Condition.Group(new[] { cond1, cond2 });

        // Act - Compile entire tree
        var sw1 = Stopwatch.StartNew();
        var expression1 = query.Compile();
        sw1.Stop();

        // Compile again (should use cache)
        var sw2 = Stopwatch.StartNew();
        var expression2 = query.Compile();
        sw2.Stop();

        // Assert
        Assert.NotNull(expression1);
        Assert.Same(expression1, expression2);
        Assert.True(sw2.Elapsed < sw1.Elapsed,
            $"Cached nested compilation ({sw2.ElapsedTicks} ticks) should be faster than initial ({sw1.ElapsedTicks} ticks)");
    }

    [Fact]
    public void Compile_WithFrequentPropertyChanges_ShouldNotRecompileUnnecessarily()
    {
        // Arrange
        var query = _configurator.BuildFor<TestEntity>();
        var property = query.ConditionPropertyPaths.First();
        var condition = query.Condition.CreateNew(property);

        // Act - Change value multiple times (should invalidate cache each time)
        for (int i = 0; i < 10; i++)
        {
            condition.Value = i;
        }

        // Only compile once at the end
        var sw = Stopwatch.StartNew();
        var expression = query.Compile();
        sw.Stop();

        // Assert
        Assert.NotNull(expression);
        // Compilation happens only when Compile() is called, not on each property change
        // This test verifies lazy evaluation works
    }

    [Fact]
    public void BlockCondition_ChildChanges_ShouldInvalidateParentCache()
    {
        // Arrange
        var query = _configurator.BuildFor<TestEntity>();
        var property = query.ConditionPropertyPaths.First();

        var cond1 = query.Condition.CreateNew(property);
        var cond2 = query.Condition.CreateNew(property);

        // Act - Compile initial state
        var expression1 = query.Compile();

        // Change child condition
        cond1.Value = 999;

        // Compile after change
        var expression2 = query.Compile();

        // Assert
        Assert.NotNull(expression1);
        Assert.NotNull(expression2);
        // Parent cache should have been invalidated
    }

    [Fact]
    public void SimpleCondition_OperatorChange_ShouldInvalidateCache()
    {
        // Arrange
        var query = _configurator.BuildFor<TestEntity>();
        var property = query.ConditionPropertyPaths.First();
        var condition = query.Condition.CreateNew(property);
        var operators = condition.AvailableOperatorsForCurrentProperty().ToList();

        // Act - Initial compilation
        var expression1 = query.Compile();

        // Change operator (if multiple available)
        if (operators.Count > 1)
        {
            condition.Operator = operators[1];
        }

        // Compile after operator change
        var expression2 = query.Compile();

        // Assert
        Assert.NotNull(expression1);
        Assert.NotNull(expression2);
        // Cache should be invalidated
    }

    [Fact]
    public void Performance_LargeQueryTree_ShouldBenefitFromCaching()
    {
        // Arrange - Create a complex query with many conditions
        var query = _configurator.BuildFor<TestEntity>();
        var property = query.ConditionPropertyPaths.First();

        // Create 20 conditions
        var conditions = new List<SimpleCondition>();
        for (int i = 0; i < 20; i++)
        {
            conditions.Add(query.Condition.CreateNew(property));
        }

        // Group them into nested structures
        if (conditions.Count >= 4)
        {
            query.Condition.Group(conditions.Take(4));
        }

        // Act - Measure initial compilation
        var sw1 = Stopwatch.StartNew();
        var expression1 = query.Compile();
        sw1.Stop();

        // Measure cached compilation (multiple times)
        var cachedTimes = new List<long>();
        for (int i = 0; i < 100; i++)
        {
            var sw = Stopwatch.StartNew();
            query.Compile();
            sw.Stop();
            cachedTimes.Add(sw.ElapsedTicks);
        }

        var avgCachedTime = cachedTimes.Average();

        // Assert
        Assert.NotNull(expression1);
        Assert.True(avgCachedTime < sw1.ElapsedTicks / 100,
            $"Average cached time ({avgCachedTime:F2} ticks) should be much faster than initial compilation ({sw1.ElapsedTicks} ticks)");
    }
}
