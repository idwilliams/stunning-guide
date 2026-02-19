namespace Substrate.Migration;

/// <summary>
/// Live migration between storage tiers with watermark-based routing.
/// </summary>
public sealed class ChunkMigrator<T>
{
    private readonly int _chunkSize;
    private readonly Queue<(int Start, int Count)> _pendingChunks = new();

    public ChunkMigrator(int chunkSize = 1024)
    {
        _chunkSize = chunkSize;
    }

    public void ScheduleMigration(int startIndex, int count)
    {
        int chunks = (count + _chunkSize - 1) / _chunkSize;
        
        for (int i = 0; i < chunks; i++)
        {
            int start = startIndex + i * _chunkSize;
            int chunkCount = Math.Min(_chunkSize, count - i * _chunkSize);
            _pendingChunks.Enqueue((start, chunkCount));
        }
    }

    public bool TryGetNextChunk(out int start, out int count)
    {
        if (_pendingChunks.Count > 0)
        {
            (start, count) = _pendingChunks.Dequeue();
            return true;
        }

        start = 0;
        count = 0;
        return false;
    }

    public int PendingChunks => _pendingChunks.Count;

    public void MigrateChunk(
        ReadOnlySpan<T> source,
        Span<T> destination,
        int sourceStart,
        int count)
    {
        source.Slice(sourceStart, count).CopyTo(destination);
    }
}
