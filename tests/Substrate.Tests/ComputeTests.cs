using Substrate.Compute;
using Xunit;

namespace Substrate.Tests;

public class ComputeTests
{
    [Fact]
    public void ColumnOps_Sum_Works()
    {
        int[] data = { 1, 2, 3, 4, 5 };
        long sum = ColumnOps.Sum(data);
        Assert.Equal(15, sum);
    }

    [Fact]
    public void ColumnOps_Sum_Large_Array()
    {
        int[] data = new int[1000];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = i + 1;
        }

        long expected = 1000L * 1001 / 2; // Sum of 1 to 1000
        long actual = ColumnOps.Sum(data);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ColumnOps_Min_Works()
    {
        int[] data = { 5, 2, 8, 1, 9, 3 };
        int min = ColumnOps.Min(data);
        Assert.Equal(1, min);
    }

    [Fact]
    public void ColumnOps_Max_Works()
    {
        int[] data = { 5, 2, 8, 1, 9, 3 };
        int max = ColumnOps.Max(data);
        Assert.Equal(9, max);
    }

    [Fact]
    public void ColumnOps_Count_Works()
    {
        int[] data = { 1, 2, 3, 2, 4, 2, 5 };
        int count = ColumnOps.Count(data, 2);
        Assert.Equal(3, count);
    }

    [Fact]
    public void ColumnOps_SIMD_Correctness()
    {
        // Test with size that triggers SIMD paths
        int[] data = new int[256];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = i;
        }

        long expectedSum = 256L * 255 / 2;
        long actualSum = ColumnOps.Sum(data);
        Assert.Equal(expectedSum, actualSum);

        int expectedMin = 0;
        int actualMin = ColumnOps.Min(data);
        Assert.Equal(expectedMin, actualMin);

        int expectedMax = 255;
        int actualMax = ColumnOps.Max(data);
        Assert.Equal(expectedMax, actualMax);
    }
}
