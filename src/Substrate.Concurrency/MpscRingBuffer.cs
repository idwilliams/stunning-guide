namespace Substrate.Concurrency;

/// <summary>
/// Lock-free multiple-producer single-consumer ring buffer.
/// Uses sequence-based synchronization with cache-line padding.
/// </summary>
public sealed class MpscRingBuffer<T> where T : class
{
    private struct Slot
    {
        public T? Value;
        public long Sequence;
        
        // Cache-line padding (assuming 64-byte cache lines)
        private readonly long _p1, _p2, _p3, _p4, _p5, _p6;
    }

    private readonly Slot[] _buffer;
    private readonly int _mask;
    private long _enqueuePos;
    private long _dequeuePos;

    public MpscRingBuffer(int capacity)
    {
        if (!IsPowerOfTwo(capacity))
        {
            throw new ArgumentException("Capacity must be a power of 2", nameof(capacity));
        }

        _buffer = new Slot[capacity];
        _mask = capacity - 1;
        
        for (int i = 0; i < capacity; i++)
        {
            _buffer[i].Sequence = i;
        }
    }

    public bool TryEnqueue(T item)
    {
        long pos = Interlocked.Increment(ref _enqueuePos) - 1;
        int index = (int)(pos & _mask);
        
        ref Slot slot = ref _buffer[index];
        long seq = Interlocked.Read(ref slot.Sequence);
        
        if (seq == pos)
        {
            slot.Value = item;
            Interlocked.Exchange(ref slot.Sequence, pos + 1);
            return true;
        }
        
        return false;
    }

    public bool TryDequeue(out T? item)
    {
        long pos = _dequeuePos;
        int index = (int)(pos & _mask);
        
        ref Slot slot = ref _buffer[index];
        long seq = Interlocked.Read(ref slot.Sequence);
        
        if (seq == pos + 1)
        {
            item = slot.Value;
            slot.Value = null;
            Interlocked.Exchange(ref slot.Sequence, pos + _mask + 1);
            _dequeuePos = pos + 1;
            return true;
        }
        
        item = null;
        return false;
    }

    private static bool IsPowerOfTwo(int x) => (x & (x - 1)) == 0 && x > 0;
}
