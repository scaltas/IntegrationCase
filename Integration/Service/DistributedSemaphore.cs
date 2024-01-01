using System;
using StackExchange.Redis;

public class DistributedSemaphore
{
    private readonly IDatabase redisDatabase;
    private readonly int token;

    public DistributedSemaphore(string connectionString)
    {
        var connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        redisDatabase = connectionMultiplexer.GetDatabase();
        Random rnd = new Random();
        token = rnd.Next(0, int.MaxValue);
    }

    public bool TryAcquire(string itemContent)
    {
        return redisDatabase.LockTake(itemContent, token, TimeSpan.FromSeconds(10));
    }

    public void Release(string itemContent)
    {
        redisDatabase.LockRelease(itemContent, token);
    }
}