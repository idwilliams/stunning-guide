namespace Substrate.Compute;

/// <summary>
/// Batch column operations with work stealing support.
/// </summary>
public static class BatchDispatch
{
    public static void ParallelSum(int[][] columns, long[] results)
    {
        if (columns.Length != results.Length)
        {
            throw new ArgumentException("Column and result counts must match");
        }

        Parallel.For(0, columns.Length, i =>
        {
            results[i] = ColumnOps.Sum(columns[i]);
        });
    }

    public static void ParallelMap<TIn, TOut>(
        TIn[] input,
        TOut[] output,
        Func<TIn, TOut> mapper)
        where TIn : struct
        where TOut : struct
    {
        if (input.Length != output.Length)
        {
            throw new ArgumentException("Input and output lengths must match");
        }

        int chunkSize = Math.Max(1, input.Length / Environment.ProcessorCount);

        Parallel.For(0, (input.Length + chunkSize - 1) / chunkSize, chunkIndex =>
        {
            int start = chunkIndex * chunkSize;
            int end = Math.Min(start + chunkSize, input.Length);

            for (int i = start; i < end; i++)
            {
                output[i] = mapper(input[i]);
            }
        });
    }
}
