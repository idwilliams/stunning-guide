using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Substrate.Memory;

/// <summary>
/// Bump allocator for short-lived data using native memory.
/// </summary>
public sealed unsafe class Arena : IDisposable
{
    private byte* _buffer;
    private int _capacity;
    private int _offset;

    public Arena(int capacity = 1024 * 1024) // 1MB default
    {
        _capacity = capacity;
        _buffer = (byte*)NativeMemory.Alloc((nuint)capacity);
        _offset = 0;
    }

    public int Used => _offset;
    public int Available => _capacity - _offset;

    public Span<T> Allocate<T>(int count) where T : unmanaged
    {
        int size = count * sizeof(T);
        int aligned = (size + 7) & ~7; // 8-byte alignment

        if (_offset + aligned > _capacity)
        {
            throw new OutOfMemoryException("Arena exhausted");
        }

        byte* ptr = _buffer + _offset;
        _offset += aligned;

        return new Span<T>(ptr, count);
    }

    public void Reset()
    {
        _offset = 0;
    }

    public void Dispose()
    {
        if (_buffer != null)
        {
            NativeMemory.Free(_buffer);
            _buffer = null;
        }
    }
}
