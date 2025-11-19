using System.Globalization;
using System.Windows.Media;
using NetQueryBuilder.WPF.Converters;

namespace NetQueryBuilder.Wpf.Tests.Converters;

public class BooleanToBackgroundConverterTests
{
    [Fact]
    public void Convert_TrueValue_ReturnsHighlightBrush()
    {
        // Arrange
        var converter = new BooleanToBackgroundConverter();

        // Act
        var result = converter.Convert(true, typeof(Brush), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result;
        Assert.Equal(Color.FromRgb(232, 244, 253), brush.Color); // Light blue highlight color
    }

    [Fact]
    public void Convert_FalseValue_ReturnsTransparentBrush()
    {
        // Arrange
        var converter = new BooleanToBackgroundConverter();

        // Act
        var result = converter.Convert(false, typeof(Brush), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Brushes.Transparent, result);
    }

    [Fact]
    public void Convert_NullValue_ReturnsTransparentBrush()
    {
        // Arrange
        var converter = new BooleanToBackgroundConverter();

        // Act
        var result = converter.Convert(null, typeof(Brush), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Brushes.Transparent, result);
    }

    [Fact]
    public void Convert_NonBooleanValue_ReturnsTransparentBrush()
    {
        // Arrange
        var converter = new BooleanToBackgroundConverter();

        // Act
        var result = converter.Convert("string", typeof(Brush), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Brushes.Transparent, result);
    }
}
