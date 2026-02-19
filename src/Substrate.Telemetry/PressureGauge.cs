namespace Substrate.Telemetry;

/// <summary>
/// Resource pressure tracking (0.0 → 1.0).
/// </summary>
public sealed class PressureGauge
{
    private long _current;
    private readonly long _max;

    public PressureGauge(long maxValue)
    {
        _current = 0;
        _max = maxValue;
    }

    public long Current => Interlocked.Read(ref _current);
    public long Max => _max;

    public double Pressure => Math.Clamp(Current / (double)_max, 0.0, 1.0);

    public void Add(long delta)
    {
        Interlocked.Add(ref _current, delta);
    }

    public void Subtract(long delta)
    {
        Interlocked.Add(ref _current, -delta);
    }

    public void Set(long value)
    {
        Interlocked.Exchange(ref _current, value);
    }

    public bool IsHigh(double threshold = 0.8) => Pressure >= threshold;
}
