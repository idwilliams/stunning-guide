using System.Buffers;
using System.Runtime.CompilerServices;

namespace Substrate.Memory;

/// <summary>
/// Stackalloc with ArrayPool fallback for temporary buffers.
/// </summary>
public ref struct ScratchBuffer<T>
{
    private Span<T> _buffer;
    private T[]? _rented;

    public ScratchBuffer(Span<T> stackBuffer, int requiredSize)
    {
        if (requiredSize <= stackBuffer.Length)
        {
            _buffer = stackBuffer[..requiredSize];
            _rented = null;
        }
        else
        {
            _rented = ArrayPool<T>.Shared.Rent(requiredSize);
            _buffer = _rented.AsSpan(0, requiredSize);
        }
    }

    public Span<T> Span => _buffer;

    public void Dispose()
    {
        if (_rented != null)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_rented, 0, _buffer.Length);
            }
            ArrayPool<T>.Shared.Return(_rented);
            _rented = null;
        }
    }
}
