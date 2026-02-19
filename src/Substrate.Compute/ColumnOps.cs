using System.Numerics;
using System.Runtime.Intrinsics;

namespace Substrate.Compute;

/// <summary>
/// Hardware-adaptive SIMD column operations.
/// Automatically detects and uses the widest available vector instruction set.
/// </summary>
public static class ColumnOps
{
    /// <summary>
    /// Vectorized sum of integer column.
    /// </summary>
    public static long Sum(ReadOnlySpan<int> data)
    {
        long sum = 0;
        int i = 0;

        // Vector512 path (AVX-512)
        if (Vector512.IsHardwareAccelerated && data.Length >= Vector512<int>.Count)
        {
            Vector512<long> vsum = Vector512<long>.Zero;
            int vecCount = data.Length - (data.Length % Vector512<int>.Count);

            for (; i < vecCount; i += Vector512<int>.Count)
            {
                var v = Vector512.Create(data.Slice(i, Vector512<int>.Count));
                var v1 = Vector512.WidenLower(v);
                var v2 = Vector512.WidenUpper(v);
                vsum += v1 + v2;
            }

            for (int j = 0; j < Vector512<long>.Count; j++)
            {
                sum += vsum[j];
            }
        }
        // Vector256 path (AVX2)
        else if (Vector256.IsHardwareAccelerated && data.Length >= Vector256<int>.Count)
        {
            Vector256<long> vsum = Vector256<long>.Zero;
            int vecCount = data.Length - (data.Length % Vector256<int>.Count);

            for (; i < vecCount; i += Vector256<int>.Count)
            {
                var v = Vector256.Create(data.Slice(i, Vector256<int>.Count));
                var v1 = Vector256.WidenLower(v);
                var v2 = Vector256.WidenUpper(v);
                vsum += v1 + v2;
            }

            for (int j = 0; j < Vector256<long>.Count; j++)
            {
                sum += vsum[j];
            }
        }
        // Vector128 path (SSE)
        else if (Vector128.IsHardwareAccelerated && data.Length >= Vector128<int>.Count)
        {
            Vector128<long> vsum = Vector128<long>.Zero;
            int vecCount = data.Length - (data.Length % Vector128<int>.Count);

            for (; i < vecCount; i += Vector128<int>.Count)
            {
                var v = Vector128.Create(data.Slice(i, Vector128<int>.Count));
                var v1 = Vector128.WidenLower(v);
                var v2 = Vector128.WidenUpper(v);
                vsum += v1 + v2;
            }

            for (int j = 0; j < Vector128<long>.Count; j++)
            {
                sum += vsum[j];
            }
        }

        // Scalar fallback
        for (; i < data.Length; i++)
        {
            sum += data[i];
        }

        return sum;
    }

    /// <summary>
    /// Find minimum value in column.
    /// </summary>
    public static int Min(ReadOnlySpan<int> data)
    {
        if (data.Length == 0)
            return 0;

        int min = int.MaxValue;
        int i = 0;

        if (Vector256.IsHardwareAccelerated && data.Length >= Vector256<int>.Count)
        {
            Vector256<int> vmin = Vector256.Create(int.MaxValue);
            int vecCount = data.Length - (data.Length % Vector256<int>.Count);

            for (; i < vecCount; i += Vector256<int>.Count)
            {
                var v = Vector256.Create(data.Slice(i, Vector256<int>.Count));
                vmin = Vector256.Min(vmin, v);
            }

            for (int j = 0; j < Vector256<int>.Count; j++)
            {
                if (vmin[j] < min)
                    min = vmin[j];
            }
        }

        for (; i < data.Length; i++)
        {
            if (data[i] < min)
                min = data[i];
        }

        return min;
    }

    /// <summary>
    /// Find maximum value in column.
    /// </summary>
    public static int Max(ReadOnlySpan<int> data)
    {
        if (data.Length == 0)
            return 0;

        int max = int.MinValue;
        int i = 0;

        if (Vector256.IsHardwareAccelerated && data.Length >= Vector256<int>.Count)
        {
            Vector256<int> vmax = Vector256.Create(int.MinValue);
            int vecCount = data.Length - (data.Length % Vector256<int>.Count);

            for (; i < vecCount; i += Vector256<int>.Count)
            {
                var v = Vector256.Create(data.Slice(i, Vector256<int>.Count));
                vmax = Vector256.Max(vmax, v);
            }

            for (int j = 0; j < Vector256<int>.Count; j++)
            {
                if (vmax[j] > max)
                    max = vmax[j];
            }
        }

        for (; i < data.Length; i++)
        {
            if (data[i] > max)
                max = data[i];
        }

        return max;
    }

    /// <summary>
    /// Count elements matching predicate.
    /// </summary>
    public static int Count(ReadOnlySpan<int> data, int target)
    {
        int count = 0;
        int i = 0;

        if (Vector256.IsHardwareAccelerated && data.Length >= Vector256<int>.Count)
        {
            Vector256<int> vtarget = Vector256.Create(target);
            int vecCount = data.Length - (data.Length % Vector256<int>.Count);

            for (; i < vecCount; i += Vector256<int>.Count)
            {
                var v = Vector256.Create(data.Slice(i, Vector256<int>.Count));
                var mask = Vector256.Equals(v, vtarget);
                count += BitOperations.PopCount((uint)Vector256.ExtractMostSignificantBits(mask));
            }
        }

        for (; i < data.Length; i++)
        {
            if (data[i] == target)
                count++;
        }

        return count;
    }
}
