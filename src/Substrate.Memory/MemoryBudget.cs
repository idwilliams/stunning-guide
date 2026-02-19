using Substrate.Telemetry;

namespace Substrate.Memory;

/// <summary>
/// Global memory budget enforcement with pressure tracking.
/// </summary>
public sealed class MemoryBudget
{
    private readonly PressureGauge _gauge;
    private readonly long _maxBytes;

    public MemoryBudget(long maxBytes)
    {
        _maxBytes = maxBytes;
        _gauge = new PressureGauge(maxBytes);
    }

    public long Used => _gauge.Current;
    public long Max => _maxBytes;
    public double Pressure => _gauge.Pressure;

    public bool TryAllocate(long bytes)
    {
        if (_gauge.Current + bytes > _maxBytes)
        {
            return false;
        }

        _gauge.Add(bytes);
        return true;
    }

    public void Release(long bytes)
    {
        _gauge.Subtract(bytes);
    }

    public bool IsUnderPressure(double threshold = 0.8)
    {
        return _gauge.IsHigh(threshold);
    }
}
