using System.Runtime.CompilerServices;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace CatalogItemUpdatedConsumer
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemUpdated>
    {
        private readonly IRepository<CatalogItem> repository;
        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {
            var message = context.Message;
            var item = await repository.GetAsync(message.ItemId);
            if (item is null)
            {
                return;
            }
            await repository.RemoveAsync(item.Id);
        }
    }
}