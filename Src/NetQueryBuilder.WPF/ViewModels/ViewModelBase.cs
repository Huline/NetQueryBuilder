using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetQueryBuilder.WPF.ViewModels;

/// <summary>
/// Base class for all ViewModels providing INotifyPropertyChanged implementation.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed. Auto-filled by the compiler.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the property value and raises PropertyChanged if the value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="field">Reference to the backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">The name of the property. Auto-filled by the compiler.</param>
    /// <returns>True if the value changed and PropertyChanged was raised; otherwise, false.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Sets the property value, raises PropertyChanged, and executes an action if the value changed.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="field">Reference to the backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="onChanged">Action to execute when the value changes.</param>
    /// <param name="propertyName">The name of the property. Auto-filled by the compiler.</param>
    /// <returns>True if the value changed; otherwise, false.</returns>
    protected bool SetProperty<T>(ref T field, T value, Action onChanged, [CallerMemberName] string? propertyName = null)
    {
        if (!SetProperty(ref field, value, propertyName))
            return false;

        onChanged?.Invoke();
        return true;
    }
}
