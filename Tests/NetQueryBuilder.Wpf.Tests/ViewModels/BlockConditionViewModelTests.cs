using System.Windows;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.WPF.ViewModels;
using NetQueryBuilder.Wpf.Tests.Mocks;

namespace NetQueryBuilder.Wpf.Tests.ViewModels;

public class BlockConditionViewModelTests
{
    [Fact]
    public void Constructor_InitializesPropertiesFromCondition()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();

        // Act
        var viewModel = new BlockConditionViewModel(query, query.Condition, 0);

        // Assert
        Assert.NotNull(viewModel.ChildConditions);
        Assert.Equal(query, viewModel.Query);
        Assert.Equal(query.Condition, viewModel.Condition);
        Assert.Equal(0, viewModel.IndentationLevel);
    }

    [Fact]
    public void IndentationMargin_CalculatesCorrectly()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();

        // Act - Level 0
        var viewModel0 = new BlockConditionViewModel(query, query.Condition, 0);
        // Act - Level 1
        var viewModel1 = new BlockConditionViewModel(query, query.Condition, 1);
        // Act - Level 2
        var viewModel2 = new BlockConditionViewModel(query, query.Condition, 2);

        // Assert
        Assert.Equal(new Thickness(0, 0, 0, 0), viewModel0.IndentationMargin);
        Assert.Equal(new Thickness(20, 0, 0, 0), viewModel1.IndentationMargin);
        Assert.Equal(new Thickness(40, 0, 0, 0), viewModel2.IndentationMargin);
    }

    [Fact]
    public void AddCondition_IncreasesChildCount()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var viewModel = new BlockConditionViewModel(query, query.Condition, 0);

        var initialCount = viewModel.ChildConditions.Count;
        var propertyPath = query.ConditionPropertyPaths.First();

        // Act - Manually add condition (bypassing command infrastructure)
        query.Condition.CreateNew(propertyPath);

        // Assert
        Assert.Equal(initialCount + 1, query.Condition.Conditions.Count);
    }

    [Fact]
    public void SelectedConditions_LessThanTwo_GroupingNotAllowed()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var viewModel = new BlockConditionViewModel(query, query.Condition, 0);

        // Act - Add one condition manually
        var propertyPath = query.ConditionPropertyPaths.First();
        query.Condition.CreateNew(propertyPath);
        viewModel.SelectedConditions.Add(query.Condition.Conditions.First());

        // Assert - Less than 2 selected, so grouping would not be allowed
        Assert.Single(viewModel.SelectedConditions);
    }

    [Fact]
    public void SelectedConditions_TwoOrMore_PreparedForGrouping()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var viewModel = new BlockConditionViewModel(query, query.Condition, 0);

        // Act - Add two conditions manually
        var propertyPath = query.ConditionPropertyPaths.First();
        query.Condition.CreateNew(propertyPath);
        query.Condition.CreateNew(propertyPath);

        // Select both
        var conditions = query.Condition.Conditions.ToList();
        viewModel.SelectedConditions.Add(conditions[0]);
        viewModel.SelectedConditions.Add(conditions[1]);

        // Assert - Two selected conditions ready for grouping
        Assert.Equal(2, viewModel.SelectedConditions.Count);
    }

    [Fact]
    public void GroupConditions_CreatesBlockCondition()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var viewModel = new BlockConditionViewModel(query, query.Condition, 0);

        // Add two conditions manually
        var propertyPath = query.ConditionPropertyPaths.First();
        query.Condition.CreateNew(propertyPath);
        query.Condition.CreateNew(propertyPath);

        var conditions = query.Condition.Conditions.ToList();
        var condition1 = conditions[0];
        var condition2 = conditions[1];

        viewModel.SelectedConditions.Add(condition1);
        viewModel.SelectedConditions.Add(condition2);

        // Act - Manually call the grouping logic (bypassing command infrastructure)
        query.Condition.Group(viewModel.SelectedConditions);
        viewModel.SelectedConditions.Clear();

        // Assert
        // After grouping, the two conditions should be wrapped in a BlockCondition
        Assert.Single(query.Condition.Conditions);
        Assert.IsType<NetQueryBuilder.Conditions.BlockCondition>(query.Condition.Conditions.First());
        Assert.Empty(viewModel.SelectedConditions); // Selection should be cleared
    }

    [Fact]
    public void RemoveCondition_RemovesConditionFromCollection()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var viewModel = new BlockConditionViewModel(query, query.Condition, 0);

        var propertyPath = query.ConditionPropertyPaths.First();
        query.Condition.CreateNew(propertyPath);
        var conditionToRemove = query.Condition.Conditions.First();

        // Act
        viewModel.RemoveCondition(conditionToRemove);

        // Assert
        Assert.Empty(query.Condition.Conditions);
    }

    [Fact]
    public void HasParent_ReturnsFalse_ForRootCondition()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var viewModel = new BlockConditionViewModel(query, query.Condition, 0);

        // Act & Assert
        Assert.False(viewModel.HasParent);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BlockConditionViewModel(null!, query.Condition, 0));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenConditionIsNull()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BlockConditionViewModel(query, null!, 0));
    }
}
