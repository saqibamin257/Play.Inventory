namespace Play.Inventory.Service
{
    public record GrantItemsDto(Guid UserId, Guid catalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemId, int Quantity, DateTimeOffset AcquiredDate);
}