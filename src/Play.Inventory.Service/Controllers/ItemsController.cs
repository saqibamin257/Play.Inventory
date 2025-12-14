using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly Play.Common.IRepository<InventoryItem> itemsRepository;
        private readonly CatalogClient catalogClient;

        public ItemsController(Play.Common.IRepository<InventoryItem> itemsRepository, CatalogClient catalogClient)
        {
            this.itemsRepository = itemsRepository;
            this.catalogClient = catalogClient;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }
            // var items = (await itemsRepository.GetAllAsync(item => item.UserId == userId))
            //            .Select(item => item.AsDto());
            // return Ok(items);

            var catalogItems = await catalogClient.GetCatalogItemsAsync(); //get all catalogItems
            var inventoryItemEntities = await itemsRepository.GetAllAsync(item => item.UserId == userId); //getInventoryITems by userid
            var InventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CategoryItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });
            return Ok(InventoryItemDtos);

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