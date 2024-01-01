using Integration.Service;

namespace Integration;

public abstract class Program
{
    public static void Main(string[] args)
    {
        // Replace these values with your actual Redis connection string and key.
        string redisConnectionString = "<Redis-Connection-String>";

        var distributedSemaphore = new DistributedSemaphore(redisConnectionString);
        var service = new ItemIntegrationService(distributedSemaphore);

        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("d"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("e"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("f"));

        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}