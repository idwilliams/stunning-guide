using Substrate.Core;

namespace Substrate.Collections;

/// <summary>
/// Dense mapping from Id&lt;T&gt; to values.
/// </summary>
public sealed class IdMap<TId, TValue>
{
    private readonly Column<TValue> _values;

    public IdMap(int initialCapacity = 16)
    {
        _values = new Column<TValue>(initialCapacity);
    }

    public ref TValue this[Id<TId> id]
    {
        get
        {
            while (id.Value >= _values.Count)
            {
                _values.Add(default!);
            }
            return ref _values[id.Value];
        }
    }

    public bool TryGet(Id<TId> id, out TValue value)
    {
        if (id.Value >= 0 && id.Value < _values.Count)
        {
            value = _values[id.Value];
            return true;
        }
        value = default!;
        return false;
    }
}
