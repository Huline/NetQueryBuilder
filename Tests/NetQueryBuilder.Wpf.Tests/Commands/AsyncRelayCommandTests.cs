using NetQueryBuilder.WPF.Commands;

namespace NetQueryBuilder.Wpf.Tests.Commands;

public class AsyncRelayCommandTests
{
    [Fact]
    public async Task AsyncRelayCommand_ExecutesAsyncAction()
    {
        // Arrange
        var executed = false;
        var command = new AsyncRelayCommand(async _ =>
        {
            await Task.Delay(10);
            executed = true;
        });

        // Act
        command.Execute(null);
        await Task.Delay(50); // Wait for async execution

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task AsyncRelayCommand_PreventsMultipleConcurrentExecutions()
    {
        // Arrange
        var executionCount = 0;
        var command = new AsyncRelayCommand(async _ =>
        {
            executionCount++;
            await Task.Delay(100);
        });

        // Act
        command.Execute(null);
        Assert.False(command.CanExecute(null)); // Should be false while executing
        command.Execute(null); // This should not execute
        command.Execute(null); // This should not execute

        await Task.Delay(150); // Wait for execution to complete

        // Assert
        Assert.Equal(1, executionCount); // Should have executed only once
        Assert.True(command.CanExecute(null)); // Should be available again
    }

    [Fact]
    public async Task AsyncRelayCommand_CanExecuteAgainAfterCompletion()
    {
        // Arrange
        var command = new AsyncRelayCommand(async _ => await Task.Delay(10));

        // Act
        Assert.True(command.CanExecute(null)); // Initially can execute
        command.Execute(null);
        Assert.False(command.CanExecute(null)); // Cannot execute while running

        await Task.Delay(50); // Wait for completion

        // Assert
        Assert.True(command.CanExecute(null)); // Can execute again after completion
    }

    [Fact]
    public async Task AsyncRelayCommand_UsesCanExecutePredicate()
    {
        // Arrange
        var canExecute = false;
        var executed = false;
        var command = new AsyncRelayCommand(
            async _ =>
            {
                await Task.Delay(10);
                executed = true;
            },
            _ => canExecute);

        // Act - First attempt (should not execute)
        Assert.False(command.CanExecute(null));
        command.Execute(null);
        await Task.Delay(50);
        Assert.False(executed);

        // Act - Second attempt (should execute)
        canExecute = true;
        Assert.True(command.CanExecute(null));
        command.Execute(null);
        await Task.Delay(50);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void AsyncRelayCommand_ThrowsArgumentNullException_WhenExecuteIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AsyncRelayCommand(null!));
    }
}
