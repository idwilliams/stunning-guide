using System.Runtime.CompilerServices;

namespace Substrate.Core;

/// <summary>
/// Strongly-typed identifier that compiles to a raw int.
/// Provides compile-time type safety with zero runtime overhead.
/// </summary>
/// <typeparam name="T">Phantom type for compile-time differentiation</typeparam>
public readonly struct Id<T> : IEquatable<Id<T>>, IComparable<Id<T>>
{
    private readonly int _value;

    public Id(int value)
    {
        _value = value;
    }

    public int Value => _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Id<T> other) => _value == other._value;

    public override bool Equals(object? obj) => obj is Id<T> other && Equals(other);

    public override int GetHashCode() => _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Id<T> other) => _value.CompareTo(other._value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Id<T> left, Id<T> right) => left._value == right._value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Id<T> left, Id<T> right) => left._value != right._value;

    public static bool operator <(Id<T> left, Id<T> right) => left._value < right._value;
    public static bool operator >(Id<T> left, Id<T> right) => left._value > right._value;
    public static bool operator <=(Id<T> left, Id<T> right) => left._value <= right._value;
    public static bool operator >=(Id<T> left, Id<T> right) => left._value >= right._value;

    public override string ToString() => _value.ToString();

    public static Id<T> Invalid => new(-1);
}
