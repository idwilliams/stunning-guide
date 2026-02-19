using System.Buffers;
using System.Runtime.CompilerServices;

namespace Substrate.Core;

/// <summary>
/// Growable structure-of-arrays storage backed by ArrayPool.
/// Provides ref-returning indexer and Span access for SIMD operations.
/// </summary>
public sealed class Column<T> : IDisposable
{
    private T[] _data;
    private int _count;

    public Column(int initialCapacity = 16)
    {
        _data = ArrayPool<T>.Shared.Rent(initialCapacity);
        _count = 0;
    }

    public int Count => _count;
    public int Capacity => _data.Length;

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Contracts.Require(index >= 0 && index < _count, "Index out of range");
            return ref _data[index];
        }
    }

    public void Add(T value)
    {
        EnsureCapacity(_count + 1);
        _data[_count++] = value;
    }

    public ReadOnlySpan<T> AsSpan() => _data.AsSpan(0, _count);
    
    public Span<T> AsSpanMutable() => _data.AsSpan(0, _count);

    public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            Array.Clear(_data, 0, _count);
        }
        _count = 0;
    }

    private void EnsureCapacity(int requiredCapacity)
    {
        if (requiredCapacity <= _data.Length)
            return;

        int newCapacity = Math.Max(_data.Length * 2, requiredCapacity);
        T[] newData = ArrayPool<T>.Shared.Rent(newCapacity);
        Array.Copy(_data, newData, _count);

        T[] oldData = _data;
        _data = newData;

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            Array.Clear(oldData, 0, _count);
        }
        ArrayPool<T>.Shared.Return(oldData);
    }

    public void Dispose()
    {
        if (_data != null)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_data, 0, _count);
            }
            ArrayPool<T>.Shared.Return(_data);
            _data = null!;
            _count = 0;
        }
    }
}
