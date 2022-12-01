using System;

namespace Play.Inventory.Service.DTOs
{
    public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemid, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);
    public record CatalogItemDto(Guid Id, string Name, string Description);
}
