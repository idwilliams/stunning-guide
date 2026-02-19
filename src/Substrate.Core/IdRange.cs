namespace Substrate.Core;

/// <summary>
/// Represents a range of IDs [Start, End).
/// </summary>
public readonly struct IdRange<T>
{
    public Id<T> Start { get; }
    public Id<T> End { get; }

    public IdRange(Id<T> start, Id<T> end)
    {
        Start = start;
        End = end;
    }

    public int Count => End.Value - Start.Value;

    public bool Contains(Id<T> id) => id >= Start && id < End;

    public IEnumerable<Id<T>> Enumerate()
    {
        for (int i = Start.Value; i < End.Value; i++)
        {
            yield return new Id<T>(i);
        }
    }
}
