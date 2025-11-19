using NetQueryBuilder.WPF.Commands;

namespace NetQueryBuilder.Wpf.Tests.Commands;

public class RelayCommandTests
{
    [Fact]
    public void RelayCommand_ExecutesAction_WhenCanExecuteReturnsTrue()
    {
        // Arrange
        var executed = false;
        var command = new RelayCommand(_ => executed = true);

        // Act
        command.Execute(null);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public void RelayCommand_CanExecute_ReturnsFalse_WhenPredicateReturnsFalse()
    {
        // Arrange
        var canExecute = false;
        var command = new RelayCommand(_ => { }, _ => canExecute);

        // Act & Assert
        Assert.False(command.CanExecute(null));
    }

    [Fact]
    public void RelayCommand_CanExecute_ReturnsCorrectValue()
    {
        // Arrange
        var canExecute = true;
        var command = new RelayCommand(_ => { }, _ => canExecute);

        // Act & Assert
        Assert.True(command.CanExecute(null));

        canExecute = false;
        Assert.False(command.CanExecute(null));
    }

    [Fact]
    public void RelayCommand_Generic_ExecutesAction_WithParameter()
    {
        // Arrange
        string? receivedParam = null;
        var command = new RelayCommand<string>(param => receivedParam = param);

        // Act
        command.Execute("test");

        // Assert
        Assert.Equal("test", receivedParam);
    }

    [Fact]
    public void RelayCommand_Generic_CanExecute_UsesParameterInPredicate()
    {
        // Arrange
        var command = new RelayCommand<int>(
            param => { },
            param => param > 0);

        // Act & Assert
        Assert.True(command.CanExecute(5));
        Assert.False(command.CanExecute(-1));
        Assert.False(command.CanExecute(0));
    }

    [Fact]
    public void RelayCommand_Generic_CanExecute_UsesPredicateCorrectly()
    {
        // Arrange
        var command = new RelayCommand<int>(
            param => { },
            param => param > 0);

        // Act & Assert
        Assert.True(command.CanExecute(5));
        Assert.False(command.CanExecute(-1));
    }

    [Fact]
    public void RelayCommand_ThrowsArgumentNullException_WhenExecuteIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RelayCommand(null!));
    }

    [Fact]
    public void RelayCommand_Generic_ThrowsArgumentNullException_WhenExecuteIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RelayCommand<string>(null!));
    }
}
