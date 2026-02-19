using System.Buffers;
using System.Runtime.CompilerServices;

namespace Substrate.Memory;

/// <summary>
/// Single-owner growable buffer backed by ArrayPool.
/// </summary>
public sealed class AdaptiveBuffer<T> : IDisposable
{
    private T[] _buffer;
    private int _count;

    public AdaptiveBuffer(int initialCapacity = 16)
    {
        _buffer = ArrayPool<T>.Shared.Rent(initialCapacity);
        _count = 0;
    }

    public int Count => _count;
    public int Capacity => _buffer.Length;

    public void Add(T item)
    {
        EnsureCapacity(_count + 1);
        _buffer[_count++] = item;
    }

    public Span<T> AsSpan() => _buffer.AsSpan(0, _count);

    public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            Array.Clear(_buffer, 0, _count);
        }
        _count = 0;
    }

    private void EnsureCapacity(int requiredCapacity)
    {
        if (requiredCapacity <= _buffer.Length)
            return;

        T[] newBuffer = ArrayPool<T>.Shared.Rent(Math.Max(_buffer.Length * 2, requiredCapacity));
        Array.Copy(_buffer, newBuffer, _count);

        T[] oldBuffer = _buffer;
        _buffer = newBuffer;

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            Array.Clear(oldBuffer, 0, _count);
        }
        ArrayPool<T>.Shared.Return(oldBuffer);
    }

    public void Dispose()
    {
        if (_buffer != null)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_buffer, 0, _count);
            }
            ArrayPool<T>.Shared.Return(_buffer);
            _buffer = null!;
        }
    }
}
