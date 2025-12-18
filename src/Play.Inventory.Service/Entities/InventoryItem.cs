using Play.Common;

namespace Play.Inventory.Service.Entities
{
    public class InventoryItem : Play.Common.IEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryItemId { get; set; } //this is ID of Catalog item, relationship b/w Catalog and inventory
        public int Quantity { get; set; }
        public DateTimeOffset AcquireDate { get; set; }
    }
}