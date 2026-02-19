namespace Substrate.Engine;

/// <summary>
/// Configuration options for Substrate engine.
/// </summary>
public sealed class SubstrateOptions
{
    public long MemoryBudgetBytes { get; set; } = 1024L * 1024 * 1024; // 1GB default
    public int DefaultTableCapacity { get; set; } = 1024;
    public bool EnableTelemetry { get; set; } = true;

    public static SubstrateOptions Default => new();
}
