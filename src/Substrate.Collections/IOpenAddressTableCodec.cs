namespace Substrate.Collections;

/// <summary>
/// Generic codec pattern for hash table entry layout.
/// </summary>
public interface IOpenAddressTableCodec<TKey, TValue>
{
    int GetHash(TKey key);
    bool KeyEquals(TKey a, TKey b);
    TKey GetKey(TValue entry);
    TValue CreateEntry(TKey key);
    bool IsTombstone(TValue entry);
    TValue CreateTombstone();
}
