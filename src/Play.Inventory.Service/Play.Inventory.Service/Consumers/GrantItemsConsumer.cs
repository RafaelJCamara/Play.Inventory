using MassTransit;
using Play.Common.Repository;
using Play.Inventory.Contracts;
using Play.Inventory.Service.Entities;
using Play.Inventory.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Play.Inventory.Service.Consumers
{
    public class GrantItemsConsumer : IConsumer<GrantItems>
    {
        private readonly IRepository<InventoryItem> inventoryItemsRepository;
        private readonly IRepository<CatalogItem> catalogItemsRepository;

        public GrantItemsConsumer(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogItemsRepository)
        {
            this.inventoryItemsRepository = inventoryItemsRepository;
            this.catalogItemsRepository = catalogItemsRepository;
        }

        public async Task Consume(ConsumeContext<GrantItems> context)
        {
            var message = context.Message;

            var item = await catalogItemsRepository.GetAsync(message.CatalogItemId);

            if(item == null)
            {
                throw new UnknowItemException(message.CatalogItemId);
            }

            var inventoryItem = await inventoryItemsRepository
                                .GetAsync(item => item.UserId == message.UserId && item.CatalogItemId == message.CatalogItemId);

            if (inventoryItem == null)
            {
                //this user doesn't have this item
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = message.CatalogItemId,
                    UserId = message.UserId,
                    Quantity = message.Quantity,
                    AcquiredDate = DateTimeOffset.Now
                };
                await inventoryItemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += message.Quantity;
                await inventoryItemsRepository.UpdateAsync(inventoryItem);
            }

            await context.Publish(new InventoryItemsGranted(message.CorrelationId));
        }
    }
}
