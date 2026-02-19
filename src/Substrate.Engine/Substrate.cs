using Substrate.Memory;

namespace Substrate.Engine;

/// <summary>
/// Top-level database-like API with table management and memory coordination.
/// </summary>
public sealed class Substrate : IDisposable
{
    private readonly Dictionary<string, Table> _tables = new();
    private readonly MemoryBudget _memoryBudget;
    private readonly SubstrateOptions _options;

    public Substrate() : this(SubstrateOptions.Default)
    {
    }

    public Substrate(SubstrateOptions options)
    {
        _options = options;
        _memoryBudget = new MemoryBudget(options.MemoryBudgetBytes);
    }

    public MemoryBudget MemoryBudget => _memoryBudget;

    public Table CreateTable(string name)
    {
        if (_tables.ContainsKey(name))
        {
            throw new InvalidOperationException($"Table '{name}' already exists");
        }

        var table = new Table(name, _options.DefaultTableCapacity);
        _tables[name] = table;
        return table;
    }

    public Table GetTable(string name)
    {
        if (!_tables.TryGetValue(name, out var table))
        {
            throw new InvalidOperationException($"Table '{name}' not found");
        }
        return table;
    }

    public bool TryGetTable(string name, out Table? table)
    {
        return _tables.TryGetValue(name, out table);
    }

    public IEnumerable<string> TableNames => _tables.Keys;

    public void Dispose()
    {
        foreach (var table in _tables.Values)
        {
            table.Dispose();
        }
        _tables.Clear();
    }
}
