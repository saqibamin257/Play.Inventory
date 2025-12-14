using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Entities;
namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly Play.Common.IRepository<InventoryItem> itemsRepository;

        public ItemsController(Play.Common.IRepository<InventoryItem> itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }
            var items = (await itemsRepository.GetAllAsync(item => item.UserId == userId))
                       .Select(item => item.AsDto());
            return Ok(items);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await itemsRepository.GetAsync(
                                item => item.UserId == grantItemsDto.UserId && item.CategoryItemId == grantItemsDto.catalogItemId);
            if (inventoryItem is null)
            {
                inventoryItem = new InventoryItem
                {
                    UserId = grantItemsDto.UserId,
                    CategoryItemId = grantItemsDto.catalogItemId,
                    Quantity = grantItemsDto.Quantity,
                    AcquireDate = DateTimeOffset.UtcNow
                };
                await itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity; //increment the quantity
                await itemsRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}