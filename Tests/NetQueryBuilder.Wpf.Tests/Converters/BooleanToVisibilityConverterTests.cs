using System.Globalization;
using System.Windows;
using NetQueryBuilder.WPF.Converters;

namespace NetQueryBuilder.Wpf.Tests.Converters;

public class BooleanToVisibilityConverterTests
{
    [Fact]
    public void Convert_TrueValue_ReturnsVisible()
    {
        // Arrange
        var converter = new BooleanToVisibilityConverter();

        // Act
        var result = converter.Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_FalseValue_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BooleanToVisibilityConverter();

        // Act
        var result = converter.Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_TrueValueWithInvert_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BooleanToVisibilityConverter { Invert = true };

        // Act
        var result = converter.Convert(true, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_FalseValueWithInvert_ReturnsVisible()
    {
        // Arrange
        var converter = new BooleanToVisibilityConverter { Invert = true };

        // Act
        var result = converter.Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_NonBooleanValue_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BooleanToVisibilityConverter();

        // Act
        var result = converter.Convert("string", typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_NullValue_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BooleanToVisibilityConverter();

        // Act
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void ConvertBack_Visible_ReturnsTrue()
    {
        // Arrange
        var converter = new BooleanToVisibilityConverter();

        // Act
        var result = converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(true, result);
    }

    [Fact]
    public void ConvertBack_Collapsed_ReturnsFalse()
    {
        // Arrange
        var converter = new BooleanToVisibilityConverter();

        // Act
        var result = converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }

    [Fact]
    public void ConvertBack_VisibleWithInvert_ReturnsFalse()
    {
        // Arrange
        var converter = new BooleanToVisibilityConverter { Invert = true };

        // Act
        var result = converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(false, result);
    }
}
