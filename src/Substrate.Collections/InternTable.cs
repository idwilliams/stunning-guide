using Substrate.Core;

namespace Substrate.Collections;

/// <summary>
/// String interning with Id&lt;string&gt; handles for fast comparison.
/// </summary>
public sealed class InternTable
{
    private readonly Dictionary<string, Id<string>> _map = new();
    private readonly List<string> _strings = new();

    public Id<string> Intern(string value)
    {
        if (_map.TryGetValue(value, out var id))
        {
            return id;
        }

        id = new Id<string>(_strings.Count);
        _strings.Add(value);
        _map[value] = id;
        return id;
    }

    public string GetString(Id<string> id)
    {
        return _strings[id.Value];
    }

    public bool TryGetId(string value, out Id<string> id)
    {
        return _map.TryGetValue(value, out id);
    }

    public int Count => _strings.Count;
}
