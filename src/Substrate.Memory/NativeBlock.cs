using System.Runtime.InteropServices;

namespace Substrate.Memory;

/// <summary>
/// Represents a block of native memory.
/// </summary>
public readonly unsafe struct NativeBlock : IDisposable
{
    private readonly void* _pointer;
    private readonly int _size;

    public NativeBlock(int size)
    {
        _size = size;
        _pointer = NativeMemory.Alloc((nuint)size);
    }

    public int Size => _size;
    public nint Pointer => (nint)_pointer;

    public Span<byte> AsSpan() => new(_pointer, _size);

    public void Dispose()
    {
        if (_pointer != default)
        {
            NativeMemory.Free(_pointer);
        }
    }
}
