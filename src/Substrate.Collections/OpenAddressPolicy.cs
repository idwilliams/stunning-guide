namespace Substrate.Collections;

/// <summary>
/// Policy for open addressing hash table behavior.
/// </summary>
public readonly struct OpenAddressPolicy
{
    public int InitialCapacity { get; init; }
    public double LoadFactor { get; init; }

    public static OpenAddressPolicy Default => new()
    {
        InitialCapacity = 16,
        LoadFactor = 0.7
    };
}
