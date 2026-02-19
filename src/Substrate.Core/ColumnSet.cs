namespace Substrate.Core;

/// <summary>
/// Collection of heterogeneous columns with synchronized row counts.
/// </summary>
public sealed class ColumnSet : IDisposable
{
    private readonly Dictionary<string, object> _columns = new();
    private int _rowCount;

    public int RowCount => _rowCount;

    public Column<T> AddColumn<T>(string name)
    {
        if (_columns.ContainsKey(name))
        {
            throw new InvalidOperationException($"Column '{name}' already exists");
        }

        var column = new Column<T>(_rowCount);
        _columns[name] = column;
        
        // Pad existing rows with default values
        for (int i = 0; i < _rowCount; i++)
        {
            column.Add(default!);
        }
        
        return column;
    }

    public Column<T> GetColumn<T>(string name)
    {
        if (!_columns.TryGetValue(name, out var column))
        {
            throw new InvalidOperationException($"Column '{name}' not found");
        }

        if (column is not Column<T> typedColumn)
        {
            throw new InvalidOperationException($"Column '{name}' is not of type {typeof(T).Name}");
        }

        return typedColumn;
    }

    public int AddRow()
    {
        int rowId = _rowCount++;
        
        // Extend all columns
        foreach (var column in _columns.Values)
        {
            var addMethod = column.GetType().GetMethod("Add");
            addMethod?.Invoke(column, new object?[] { GetDefault(column.GetType().GetGenericArguments()[0]) });
        }
        
        return rowId;
    }

    private static object? GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    public void Dispose()
    {
        foreach (var column in _columns.Values)
        {
            if (column is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _columns.Clear();
    }
}
