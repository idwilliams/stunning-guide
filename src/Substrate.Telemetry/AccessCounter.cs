using System.Diagnostics;

namespace Substrate.Telemetry;

/// <summary>
/// Tracks access rate with EWMA smoothing.
/// </summary>
public sealed class AccessCounter
{
    private long _count;
    private Ewma _rate;
    private long _lastTimestamp;

    public AccessCounter(double alpha = 0.1)
    {
        _count = 0;
        _rate = new Ewma(alpha);
        _lastTimestamp = Stopwatch.GetTimestamp();
    }

    public long Count => _count;
    public double Rate => _rate.Value;

    public void Record()
    {
        Interlocked.Increment(ref _count);
        
        long now = Stopwatch.GetTimestamp();
        long elapsed = now - Interlocked.Read(ref _lastTimestamp);
        
        if (elapsed > Stopwatch.Frequency / 10) // Update rate every 100ms
        {
            double rate = _count / (elapsed / (double)Stopwatch.Frequency);
            _rate.Update(rate);
            Interlocked.Exchange(ref _lastTimestamp, now);
        }
    }

    public void Reset()
    {
        _count = 0;
        _rate.Reset();
        _lastTimestamp = Stopwatch.GetTimestamp();
    }
}
