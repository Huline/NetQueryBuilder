using NetQueryBuilder.Configurations;
using NetQueryBuilder.WPF.ViewModels;
using NetQueryBuilder.Wpf.Tests.Mocks;

namespace NetQueryBuilder.Wpf.Tests.ViewModels;

public class QueryResultViewModelTests
{
    [Fact]
    public async Task Results_SetsCurrentPageAndTotalPages()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var results = await query.Execute(pageSize: 2);
        var viewModel = new QueryResultViewModel();

        // Act
        viewModel.Results = results;

        // Assert
        Assert.NotNull(viewModel.Results);
        Assert.Equal(1, viewModel.CurrentPage); // 1-based
        Assert.True(viewModel.TotalPages >= 1);
        Assert.Equal(3, viewModel.TotalItems);
    }

    [Fact]
    public async Task HasResults_ReturnsTrue_WhenResultsHaveItems()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var results = await query.Execute(pageSize: 10);
        var viewModel = new QueryResultViewModel();

        // Act
        viewModel.Results = results;

        // Assert
        Assert.True(viewModel.HasResults);
        Assert.NotNull(viewModel.Items);
        Assert.NotEmpty(viewModel.Items);
    }

    [Fact]
    public void HasResults_ReturnsFalse_WhenNoResults()
    {
        // Arrange
        var viewModel = new QueryResultViewModel();

        // Act & Assert
        Assert.False(viewModel.HasResults);
        Assert.Null(viewModel.Items);
    }

    [Fact]
    public async Task CanGoToPreviousPage_ReturnsFalse_OnFirstPage()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var results = await query.Execute(pageSize: 1);
        var viewModel = new QueryResultViewModel { Results = results };

        // Act & Assert
        Assert.False(viewModel.CanGoToPreviousPage);
        Assert.False(viewModel.GoToPreviousPageCommand.CanExecute(null));
        Assert.False(viewModel.GoToFirstPageCommand.CanExecute(null));
    }

    [Fact]
    public async Task CanGoToNextPage_ReturnsFalse_OnLastPage()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var results = await query.Execute(pageSize: 10); // All items on one page
        var viewModel = new QueryResultViewModel { Results = results };

        // Act & Assert
        Assert.False(viewModel.CanGoToNextPage);
        Assert.False(viewModel.GoToNextPageCommand.CanExecute(null));
        Assert.False(viewModel.GoToLastPageCommand.CanExecute(null));
    }

    [Fact]
    public async Task CanGoToNextPage_ReturnsTrue_WhenNotOnLastPage()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var results = await query.Execute(pageSize: 1); // Multiple pages
        var viewModel = new QueryResultViewModel { Results = results };

        // Act & Assert
        Assert.True(viewModel.CanGoToNextPage);
        Assert.True(viewModel.GoToNextPageCommand.CanExecute(null));
        Assert.True(viewModel.GoToLastPageCommand.CanExecute(null));
    }

    [Fact]
    public async Task CurrentPage_UpdatesCanGoToPageProperties()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var results = await query.Execute(pageSize: 1);
        var viewModel = new QueryResultViewModel { Results = results };

        // Initially on page 1
        Assert.False(viewModel.CanGoToPreviousPage);
        Assert.True(viewModel.CanGoToNextPage);

        // Move to page 2
        viewModel.CurrentPage = 2;
        Assert.True(viewModel.CanGoToPreviousPage);
        Assert.True(viewModel.CanGoToNextPage);

        // Move to last page (page 3)
        viewModel.CurrentPage = 3;
        Assert.True(viewModel.CanGoToPreviousPage);
        Assert.False(viewModel.CanGoToNextPage);
    }

    [Fact]
    public void Constructor_InitializesCommands()
    {
        // Arrange & Act
        var viewModel = new QueryResultViewModel();

        // Assert
        Assert.NotNull(viewModel.GoToFirstPageCommand);
        Assert.NotNull(viewModel.GoToPreviousPageCommand);
        Assert.NotNull(viewModel.GoToNextPageCommand);
        Assert.NotNull(viewModel.GoToLastPageCommand);
    }

    [Fact]
    public async Task TotalItems_ReflectsCorrectCount()
    {
        // Arrange
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        var query = configurator.BuildFor<Person>();
        var results = await query.Execute(pageSize: 2);
        var viewModel = new QueryResultViewModel();

        // Act
        viewModel.Results = results;

        // Assert
        Assert.Equal(3, viewModel.TotalItems);
    }
}
