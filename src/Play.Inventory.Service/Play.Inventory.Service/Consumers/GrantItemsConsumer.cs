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

                inventoryItem.MessageIds.Add(context.MessageId.Value);

                await inventoryItemsRepository.CreateAsync(inventoryItem);
            }
            else
            {

                if (inventoryItem.MessageIds.Contains(context.MessageId.Value))
                {
                    //this message as already been seen by this consumer, so we don't need to do a thing
                    await context.Publish(new InventoryItemsGranted(message.CorrelationId));
                    return;
                }

                inventoryItem.Quantity += message.Quantity;
                inventoryItem.MessageIds.Add(context.MessageId.Value);
                await inventoryItemsRepository.UpdateAsync(inventoryItem);
            }

            await context.Publish(new InventoryItemsGranted(message.CorrelationId));
        }
    }
}
