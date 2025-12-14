namespace Play.Inventory.Service
{
    public record GrantItemsDto(Guid UserId, Guid catalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);
    public record CatalogItemDto(Guid Id, string Name, string Description);
}