using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Play.Inventory.Contracts
{
    //CorrelationId refers to the ID in the state machine
    public record GrantItems(Guid UserId, Guid CatalogItemId, int Quantity, Guid CorrelationId);
    public record InventoryItemsGranted(Guid CorrelationId);

    public record SubtracItems(Guid UserId, Guid CatalogItemId, int Quantity, Guid CorrelationId);
    public record InventoryItemsSubtracted(Guid CorrelationId);
}
