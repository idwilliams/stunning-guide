using System.Threading.Channels;

namespace Substrate.Concurrency;

/// <summary>
/// Channel-based producer-consumer with batching support.
/// </summary>
public sealed class ProducerConsumer<T> : IDisposable
{
    private readonly Channel<T> _channel;
    private readonly CancellationTokenSource _cts;

    public ProducerConsumer(int capacity = 1024)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<T>(options);
        _cts = new CancellationTokenSource();
    }

    public async ValueTask ProduceAsync(T item, CancellationToken ct = default)
    {
        await _channel.Writer.WriteAsync(item, ct);
    }

    public async ValueTask<T?> ConsumeAsync(CancellationToken ct = default)
    {
        if (await _channel.Reader.WaitToReadAsync(ct))
        {
            if (_channel.Reader.TryRead(out T? item))
            {
                return item;
            }
        }
        return default;
    }

    public async ValueTask<List<T>> ConsumeBatchAsync(int maxBatchSize, CancellationToken ct = default)
    {
        var batch = new List<T>(maxBatchSize);
        
        if (await _channel.Reader.WaitToReadAsync(ct))
        {
            while (batch.Count < maxBatchSize && _channel.Reader.TryRead(out T? item))
            {
                batch.Add(item);
            }
        }
        
        return batch;
    }

    public void Complete()
    {
        _channel.Writer.Complete();
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
