using Substrate.Core;

namespace Substrate.Memory;

/// <summary>
/// Column storage with hot→compressed→disk tiering.
/// </summary>
public sealed class TieredColumn<T> : IDisposable
{
    private enum Tier
    {
        Hot,
        Compressed,
        Disk
    }

    private Column<T> _hot;
#pragma warning disable CS0169 // Field is never used - reserved for future tier implementation
    private byte[]? _compressed;
#pragma warning restore CS0169
#pragma warning disable CS0649 // Field is never assigned - reserved for future tier implementation
    private string? _diskPath;
#pragma warning restore CS0649
    private Tier _currentTier;

    public TieredColumn(int initialCapacity = 16)
    {
        _hot = new Column<T>(initialCapacity);
        _currentTier = Tier.Hot;
    }

    public int Count => _currentTier switch
    {
        Tier.Hot => _hot.Count,
        _ => throw new NotImplementedException("Compressed/Disk tiers not yet implemented")
    };

    public ref T this[int index]
    {
        get
        {
            EnsureHot();
            return ref _hot[index];
        }
    }

    public void Add(T value)
    {
        EnsureHot();
        _hot.Add(value);
    }

    private void EnsureHot()
    {
        if (_currentTier == Tier.Hot)
            return;

        // TODO: Implement decompression/loading from disk
        throw new NotImplementedException("Tier migration not yet implemented");
    }

    public void Dispose()
    {
        _hot?.Dispose();
        
        if (_diskPath != null && File.Exists(_diskPath))
        {
            File.Delete(_diskPath);
        }
    }
}
