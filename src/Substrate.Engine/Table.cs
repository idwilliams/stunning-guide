using Substrate.Core;

namespace Substrate.Engine;

/// <summary>
/// Named table with typed columns and row-based operations.
/// </summary>
public sealed class Table : IDisposable
{
    private readonly string _name;
    private readonly ColumnSet _columns;

    public Table(string name, int initialCapacity = 1024)
    {
        _name = name;
        _columns = new ColumnSet();
    }

    public string Name => _name;
    public int RowCount => _columns.RowCount;

    public Column<T> AddColumn<T>(string columnName)
    {
        return _columns.AddColumn<T>(columnName);
    }

    public Column<T> GetColumn<T>(string columnName)
    {
        return _columns.GetColumn<T>(columnName);
    }

    public int AddRow()
    {
        return _columns.AddRow();
    }

    public void Dispose()
    {
        _columns.Dispose();
    }
}
