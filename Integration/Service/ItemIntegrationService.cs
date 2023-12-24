using Integration.Common;
using Integration.Backend;
using System.Collections.Concurrent;

namespace Integration.Service;

public sealed class ItemIntegrationService
{
    // Use a ConcurrentDictionary to store a flag for each item content being processed.
    private readonly ConcurrentDictionary<string, object> contentLocks = new();

    //This is a dependency that is normally fulfilled externally.
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();

    // This is called externally and can be called multithreaded, in parallel.
    // More than one item with the same content should not be saved. However,
    // calling this with different contents at the same time is OK, and should
    // be allowed for performance reasons.
    public Result SaveItem(string itemContent)
    {
        // Use a lock specific to the item content to ensure thread safety.
        lock (contentLocks.GetOrAdd(itemContent, new object()))
        {
            // Check the backend to see if the content is already saved.
            if (ItemIntegrationBackend.FindItemsWithContent(itemContent).Count != 0)
            {
                return new Result(false, $"Duplicate item received with content {itemContent}.");
            }

            var item = ItemIntegrationBackend.SaveItem(itemContent);

            // Remove the lock for the item content.
            contentLocks.TryRemove(itemContent, out _);

            return new Result(true, $"Item with content {itemContent} saved with id {item.Id}");
        }
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}