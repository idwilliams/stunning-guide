using System.Runtime.CompilerServices;

namespace Substrate.Telemetry;

/// <summary>
/// Exponentially weighted moving average with time-decay.
/// </summary>
public struct Ewma
{
    private double _value;
    private readonly double _alpha;

    public Ewma(double alpha = 0.1)
    {
        _value = 0.0;
        _alpha = alpha;
    }

    public double Value => _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update(double sample)
    {
        _value = _alpha * sample + (1.0 - _alpha) * _value;
    }

    public void Reset()
    {
        _value = 0.0;
    }
}
