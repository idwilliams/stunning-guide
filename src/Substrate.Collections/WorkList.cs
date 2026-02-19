namespace Substrate.Collections;

/// <summary>
/// Stack/queue hybrid for graph traversal with work stealing support.
/// </summary>
public sealed class WorkList<T>
{
    private T[] _items;
    private int _count;

    public WorkList(int capacity = 16)
    {
        _items = new T[capacity];
        _count = 0;
    }

    public int Count => _count;
    public bool IsEmpty => _count == 0;

    public void Push(T item)
    {
        EnsureCapacity(_count + 1);
        _items[_count++] = item;
    }

    public bool TryPop(out T item)
    {
        if (_count == 0)
        {
            item = default!;
            return false;
        }

        item = _items[--_count];
        return true;
    }

    public bool TryDequeue(out T item)
    {
        if (_count == 0)
        {
            item = default!;
            return false;
        }

        item = _items[0];
        Array.Copy(_items, 1, _items, 0, --_count);
        return true;
    }

    public void Clear()
    {
        Array.Clear(_items, 0, _count);
        _count = 0;
    }

    private void EnsureCapacity(int requiredCapacity)
    {
        if (requiredCapacity > _items.Length)
        {
            Array.Resize(ref _items, Math.Max(_items.Length * 2, requiredCapacity));
        }
    }
}
