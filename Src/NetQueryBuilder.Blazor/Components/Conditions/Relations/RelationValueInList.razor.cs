using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace NetQueryBuilder.Blazor.Components.Conditions.Relations;

public partial class RelationValueInList<T>
{
    [Parameter] public MethodCallExpression MethodCallExpression { get; set; } = null!;
    [Parameter] public Action<T> OnUpdateValue { get; set; } = null!;
    [Parameter] public Action<IEnumerable<T>> OnUpdateValues { get; set; } = null!;
    private T? _listItemInputValue;
    private string _stringInputValue = string.Empty;
    private int _intInputValue;
    private List<T> _items = new();
    private bool _showListItemDialog;
    private IEnumerable<T> Items => _items;

    protected override void OnParametersSet()
    {
        // Si l'argument est un ConstantExpression, on récupère sa valeur
        if (MethodCallExpression.Arguments[0] is ConstantExpression constantExpression)
        {
            _items = ((IEnumerable<T>)constantExpression.Value! ?? Array.Empty<T>()).ToList();
        }
        // Si c'est un NewExpression, on part du principe que c'est une liste vide
        else if (MethodCallExpression.Arguments[0] is NewExpression)
        {
            // La liste est déjà initialisée comme vide
        }
    }

    private void OpenDialog()
    {
        _showListItemDialog = true;
    }

    private void UpdateStringValue(string value)
    {
        _stringInputValue = value;
        if (typeof(T) == typeof(string))
        {
            _listItemInputValue = (T)(object)value;
        }
    }

    private void UpdateIntValue(int value)
    {
        _intInputValue = value;
        if (typeof(T) == typeof(int))
        {
            _listItemInputValue = (T)(object)value;
        }
    }

    private void AddListItem()
    {
        if (_listItemInputValue is not null)
            _items.Add(_listItemInputValue);
        _showListItemDialog = false;

        // Réinitialiser les valeurs
        _listItemInputValue = default;
        _stringInputValue = string.Empty;
        _intInputValue = 0;

        UpdateValues(_items);
    }

    private void RemoveListItem(T item)
    {
        if (item is null) return;
        var chip = _items.FirstOrDefault(i => i?.Equals(item) == true);
        if (chip is null)
            return;
        _items.Remove(chip);
        UpdateValues(_items);
    }

    private void UpdateValue(T value)
    {
        OnUpdateValue.Invoke(value);
    }

    private void UpdateValues(IEnumerable<T> value)
    {
        OnUpdateValues.Invoke(value);
    }
}