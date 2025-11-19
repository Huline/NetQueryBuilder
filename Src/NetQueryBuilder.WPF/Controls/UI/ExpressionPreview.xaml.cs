using System.Windows;
using System.Windows.Controls;

namespace NetQueryBuilder.WPF.Controls.UI;

/// <summary>
/// A read-only display for the generated query expression.
/// </summary>
public partial class ExpressionPreview : UserControl
{
    public static readonly DependencyProperty ExpressionProperty =
        DependencyProperty.Register(
            nameof(Expression),
            typeof(string),
            typeof(ExpressionPreview),
            new PropertyMetadata(string.Empty));

    public ExpressionPreview()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the expression text to display.
    /// </summary>
    public string Expression
    {
        get => (string)GetValue(ExpressionProperty);
        set => SetValue(ExpressionProperty, value);
    }
}
