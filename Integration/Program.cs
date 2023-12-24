using Integration.Service;

namespace Integration;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var service = new ItemIntegrationService();
        
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

        Thread.Sleep(500);

        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("a"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("b"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("c"));

        // the implemented solution allows parallel storage of items with different content.
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("d"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("e"));
        ThreadPool.QueueUserWorkItem(_ => service.SaveItem("f"));

        Thread.Sleep(5000);

        Console.WriteLine("Everything recorded:");

        service.GetAllItems().ForEach(Console.WriteLine);

        Console.ReadLine();
    }
}