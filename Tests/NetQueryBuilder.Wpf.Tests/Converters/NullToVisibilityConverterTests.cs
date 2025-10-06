using System.Globalization;
using System.Windows;
using NetQueryBuilder.WPF.Converters;

namespace NetQueryBuilder.Wpf.Tests.Converters;

public class NullToVisibilityConverterTests
{
    [Fact]
    public void Convert_NullValue_ReturnsCollapsed()
    {
        // Arrange
        var converter = new NullToVisibilityConverter();

        // Act
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_NonNullValue_ReturnsVisible()
    {
        // Arrange
        var converter = new NullToVisibilityConverter();

        // Act
        var result = converter.Convert("test", typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_NullValueWithInvert_ReturnsVisible()
    {
        // Arrange
        var converter = new NullToVisibilityConverter { Invert = true };

        // Act
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_NonNullValueWithInvert_ReturnsCollapsed()
    {
        // Arrange
        var converter = new NullToVisibilityConverter { Invert = true };

        // Act
        var result = converter.Convert("test", typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsVisible()
    {
        // Arrange
        var converter = new NullToVisibilityConverter();

        // Act
        var result = converter.Convert("", typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_ZeroValue_ReturnsVisible()
    {
        // Arrange
        var converter = new NullToVisibilityConverter();

        // Act
        var result = converter.Convert(0, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Visible, result);
    }
}
