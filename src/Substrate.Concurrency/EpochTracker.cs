using System.Collections.Concurrent;

namespace Substrate.Concurrency;

/// <summary>
/// Epoch-based memory reclamation for lock-free data structures.
/// </summary>
public sealed class EpochTracker
{
    private long _globalEpoch;
    private readonly ConcurrentDictionary<int, long> _threadEpochs = new();

    public long CurrentEpoch => Interlocked.Read(ref _globalEpoch);

    public long EnterEpoch()
    {
        int threadId = Environment.CurrentManagedThreadId;
        long epoch = CurrentEpoch;
        _threadEpochs[threadId] = epoch;
        return epoch;
    }

    public void ExitEpoch()
    {
        int threadId = Environment.CurrentManagedThreadId;
        _threadEpochs.TryRemove(threadId, out _);
    }

    public void AdvanceEpoch()
    {
        Interlocked.Increment(ref _globalEpoch);
    }

    public long GetMinimumEpoch()
    {
        long min = CurrentEpoch;
        foreach (var epoch in _threadEpochs.Values)
        {
            if (epoch < min)
                min = epoch;
        }
        return min;
    }
}
