using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.WPF.ViewModels;
using NetQueryBuilder.Wpf.Tests.Mocks;

namespace NetQueryBuilder.Wpf.Tests.ViewModels;

public class SimpleConditionViewModelTests
{
    [Fact]
    public void Constructor_InitializesPropertiesFromCondition()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var propertyPath = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName");
        query.Condition.CreateNew<EqualsOperator>(propertyPath, "John");
        var simpleCondition = query.Condition.Conditions.First() as NetQueryBuilder.Conditions.SimpleCondition;

        // Act
        var viewModel = new SimpleConditionViewModel(query, simpleCondition!);

        // Assert
        Assert.NotNull(viewModel.AvailableProperties);
        Assert.NotEmpty(viewModel.AvailableProperties);
        Assert.Equal(propertyPath, viewModel.SelectedProperty);
        Assert.NotNull(viewModel.SelectedOperator);
        Assert.Equal("John", viewModel.Value);
        Assert.Equal(typeof(string), viewModel.PropertyType);
    }

    [Fact]
    public void SelectedProperty_UpdatesAvailableOperators()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var initialProperty = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName");
        query.Condition.CreateNew<EqualsOperator>(initialProperty, "John");
        var simpleCondition = query.Condition.Conditions.First() as NetQueryBuilder.Conditions.SimpleCondition;
        var viewModel = new SimpleConditionViewModel(query, simpleCondition!);

        var initialOperatorCount = viewModel.AvailableOperators.Count;

        // Act
        var newProperty = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Id");
        viewModel.SelectedProperty = newProperty;

        // Assert
        Assert.Equal(newProperty, viewModel.SelectedProperty);
        Assert.Equal(typeof(int), viewModel.PropertyType);
        Assert.NotEmpty(viewModel.AvailableOperators);
        // Different property types may have different operators
        Assert.NotNull(viewModel.SelectedOperator);
    }

    [Fact]
    public void SelectedOperator_UpdatesCondition()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var propertyPath = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Id");
        query.Condition.CreateNew<EqualsOperator>(propertyPath, 1);
        var simpleCondition = query.Condition.Conditions.First() as NetQueryBuilder.Conditions.SimpleCondition;
        var viewModel = new SimpleConditionViewModel(query, simpleCondition!);

        // Act
        var greaterThanOperator = viewModel.AvailableOperators.FirstOrDefault(o => o is GreaterThanOperator);
        viewModel.SelectedOperator = greaterThanOperator;

        // Assert
        Assert.Equal(greaterThanOperator, viewModel.SelectedOperator);
        Assert.Equal(greaterThanOperator, simpleCondition.Operator);
    }

    [Fact]
    public void Value_UpdatesCondition()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var propertyPath = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName");
        query.Condition.CreateNew<EqualsOperator>(propertyPath, "John");
        var simpleCondition = query.Condition.Conditions.First() as NetQueryBuilder.Conditions.SimpleCondition;
        var viewModel = new SimpleConditionViewModel(query, simpleCondition!);

        // Act
        viewModel.Value = "Jane";

        // Assert
        Assert.Equal("Jane", viewModel.Value);
        Assert.Equal("Jane", simpleCondition.Value);
    }

    [Fact]
    public void DeleteCommand_RaisesDeleteRequestedEvent()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var propertyPath = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName");
        query.Condition.CreateNew<EqualsOperator>(propertyPath, "John");
        var simpleCondition = query.Condition.Conditions.First() as NetQueryBuilder.Conditions.SimpleCondition;
        var viewModel = new SimpleConditionViewModel(query, simpleCondition!);

        var eventRaised = false;
        viewModel.DeleteRequested += (s, e) => eventRaised = true;

        // Act
        viewModel.DeleteCommand.Execute(null);

        // Assert
        Assert.True(eventRaised);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenQueryIsNull()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var propertyPath = query.ConditionPropertyPaths.First();
        query.Condition.CreateNew<EqualsOperator>(propertyPath, "test");
        var simpleCondition = query.Condition.Conditions.First() as NetQueryBuilder.Conditions.SimpleCondition;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SimpleConditionViewModel(null!, simpleCondition!));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenConditionIsNull()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SimpleConditionViewModel(query, null!));
    }
}
