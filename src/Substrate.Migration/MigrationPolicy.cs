namespace Substrate.Migration;

/// <summary>
/// Policy for triggering data migration between tiers.
/// </summary>
public readonly struct MigrationPolicy
{
    public double HotThreshold { get; init; }
    public double ColdThreshold { get; init; }
    public TimeSpan MinIdleTime { get; init; }

    public static MigrationPolicy Default => new()
    {
        HotThreshold = 0.8,
        ColdThreshold = 0.2,
        MinIdleTime = TimeSpan.FromMinutes(5)
    };

    public bool ShouldMigrateToHot(double accessRate, double memoryPressure)
    {
        return accessRate > HotThreshold && memoryPressure < HotThreshold;
    }

    public bool ShouldMigrateToCold(double accessRate, TimeSpan idleTime)
    {
        return accessRate < ColdThreshold && idleTime > MinIdleTime;
    }
}
