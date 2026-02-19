namespace Substrate.Collections;

/// <summary>
/// Linear-probing hash table with generic codec pattern.
/// Power-of-2 capacity with tombstone-aware rehashing.
/// </summary>
public sealed class OpenAddressTable<TKey, TValue, TCodec>
    where TKey : notnull
    where TCodec : IOpenAddressTableCodec<TKey, TValue>, new()
{
    private TValue[] _entries;
    private int _count;
    private int _tombstones;
    private readonly TCodec _codec;
    private readonly OpenAddressPolicy _policy;

    public OpenAddressTable(OpenAddressPolicy? policy = null)
    {
        _policy = policy ?? OpenAddressPolicy.Default;
        _codec = new TCodec();
        _entries = new TValue[_policy.InitialCapacity];
        _count = 0;
        _tombstones = 0;
    }

    public int Count => _count;

    public bool TryInsert(TKey key, out TValue entry)
    {
        if (NeedsRehash())
        {
            Rehash();
        }

        int hash = _codec.GetHash(key);
        int index = hash & (_entries.Length - 1);
        int probes = 0;

        while (probes < _entries.Length)
        {
            ref TValue slot = ref _entries[index];
            
            if (EqualityComparer<TValue>.Default.Equals(slot, default!))
            {
                slot = _codec.CreateEntry(key);
                entry = slot;
                _count++;
                return true;
            }

            if (_codec.IsTombstone(slot))
            {
                slot = _codec.CreateEntry(key);
                entry = slot;
                _count++;
                _tombstones--;
                return true;
            }

            if (_codec.KeyEquals(_codec.GetKey(slot), key))
            {
                entry = slot;
                return false;
            }

            index = (index + 1) & (_entries.Length - 1);
            probes++;
        }

        throw new InvalidOperationException("Hash table is full");
    }

    public bool TryFind(TKey key, out TValue entry)
    {
        int hash = _codec.GetHash(key);
        int index = hash & (_entries.Length - 1);
        int probes = 0;

        while (probes < _entries.Length)
        {
            ref TValue slot = ref _entries[index];
            
            if (EqualityComparer<TValue>.Default.Equals(slot, default!))
            {
                entry = default!;
                return false;
            }

            if (!_codec.IsTombstone(slot) && _codec.KeyEquals(_codec.GetKey(slot), key))
            {
                entry = slot;
                return true;
            }

            index = (index + 1) & (_entries.Length - 1);
            probes++;
        }

        entry = default!;
        return false;
    }

    private bool NeedsRehash()
    {
        return (_count + _tombstones) >= (int)(_entries.Length * _policy.LoadFactor);
    }

    private void Rehash()
    {
        TValue[] oldEntries = _entries;
        _entries = new TValue[oldEntries.Length * 2];
        _count = 0;
        _tombstones = 0;

        foreach (var entry in oldEntries)
        {
            if (!EqualityComparer<TValue>.Default.Equals(entry, default!) && !_codec.IsTombstone(entry))
            {
                TryInsert(_codec.GetKey(entry), out _);
            }
        }
    }
}
