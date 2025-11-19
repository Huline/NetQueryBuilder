using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace NetQueryBuilder.WPF.Behaviors;

/// <summary>
/// Attached behavior to synchronize ListBox.SelectedItems with a bindable collection.
/// </summary>
public static class ListBoxSelectionBehavior
{
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(IList),
            typeof(ListBoxSelectionBehavior),
            new PropertyMetadata(null, OnSelectedItemsChanged));

    public static IList GetSelectedItems(DependencyObject obj)
    {
        return (IList)obj.GetValue(SelectedItemsProperty);
    }

    public static void SetSelectedItems(DependencyObject obj, IList value)
    {
        obj.SetValue(SelectedItemsProperty, value);
    }

    private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ListBox listBox)
            return;

        // Unsubscribe from old collection
        if (e.OldValue is INotifyCollectionChanged oldCollection)
        {
            oldCollection.CollectionChanged -= (s, args) => OnCollectionChanged(listBox, args);
        }

        // Clear current selections
        listBox.SelectedItems.Clear();

        // Subscribe to new collection
        if (e.NewValue is INotifyCollectionChanged newCollection)
        {
            newCollection.CollectionChanged += (s, args) => OnCollectionChanged(listBox, args);

            // Sync initial items
            if (e.NewValue is IList list)
            {
                foreach (var item in list)
                {
                    if (!listBox.SelectedItems.Contains(item))
                        listBox.SelectedItems.Add(item);
                }
            }
        }

        // Subscribe to ListBox selection changes
        listBox.SelectionChanged -= OnListBoxSelectionChanged;
        listBox.SelectionChanged += OnListBoxSelectionChanged;
    }

    private static void OnCollectionChanged(ListBox listBox, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (!listBox.SelectedItems.Contains(item))
                    listBox.SelectedItems.Add(item);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                listBox.SelectedItems.Remove(item);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            listBox.SelectedItems.Clear();
        }
    }

    private static void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox)
            return;

        // Only handle events that originated from the ListBox itself, not from child controls
        if (e.OriginalSource != listBox)
            return;

        var boundCollection = GetSelectedItems(listBox);
        if (boundCollection == null)
            return;

        // Add newly selected items to bound collection
        foreach (var item in e.AddedItems)
        {
            if (!boundCollection.Contains(item))
                boundCollection.Add(item);
        }

        // Remove deselected items from bound collection
        foreach (var item in e.RemovedItems)
        {
            boundCollection.Remove(item);
        }
    }
}
