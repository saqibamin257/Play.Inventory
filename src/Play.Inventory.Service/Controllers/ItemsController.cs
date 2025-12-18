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
        private readonly Play.Common.IRepository<InventoryItem> inventoryItemsRepository;
        private readonly Play.Common.IRepository<CatalogItem> catalogItemsRepository;

        public ItemsController(Play.Common.IRepository<InventoryItem> inventoryItemsRepository, Play.Common.IRepository<CatalogItem> catalogItemsRepository)
        {
            this.inventoryItemsRepository = inventoryItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
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
            //var catalogItems = await catalogClient.GetCatalogItemsAsync(); //get all catalogItems

            var inventoryItemEntities = await inventoryItemsRepository.GetAllAsync(item => item.UserId == userId); //getInventoryITems by userid
            var itemIds = inventoryItemEntities.Select(item => item.CategoryItemId);
            var catalogItemEntities = await catalogItemsRepository.GetAllAsync(item => itemIds.Contains(item.Id)); //get all catalogItems against Id

            var InventoryItemDtos = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == inventoryItem.CategoryItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });
            return Ok(InventoryItemDtos);

        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await inventoryItemsRepository.GetAsync(
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
                await inventoryItemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity; //increment the quantity
                await inventoryItemsRepository.UpdateAsync(inventoryItem);
            }
            return Ok();
        }
    }
}