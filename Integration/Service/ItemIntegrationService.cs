using Integration.Common;
using Integration.Backend;

namespace Integration.Service;

public sealed class ItemIntegrationService
{
    //This is a dependency that is normally fulfilled externally.
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();
    
    private readonly DistributedSemaphore distributedSemaphore;

    public ItemIntegrationService(DistributedSemaphore distributedSemaphore)
    {
        this.distributedSemaphore = distributedSemaphore;
    }

    public Result SaveItem(string itemContent)
    {
        // Acquire a distributed lock to ensure only one server processes items at a time.
        if (!distributedSemaphore.TryAcquire(itemContent))
        {
            return new Result(false, "Unable to acquire distributed semaphore.");
        }

        try
        {
            // Check the backend to see if the content is already saved.
            if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
            {
                return new Result(false, $"Duplicate item received with content {itemContent}.");
            }

            var item = ItemIntegrationBackend.SaveItem(itemContent);

            return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
  
        }
        finally
        {
            // Release the distributed semaphore.
            distributedSemaphore.Release(itemContent);
        }
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}