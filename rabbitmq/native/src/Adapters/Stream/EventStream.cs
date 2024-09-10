using Application.Interfaces.Services;
using Application.Services.Order.Model;

namespace DevPrime.Stream;
public class EventStream : EventStreamBase, IEventStream
{
    public override void StreamEvents()
    {
       // Mapping and receiving the "OrderEvents" event
        Subscribe<IOrderService, OrderCreatedEventDTO>("Stream1", "OrderCreated", (dto, orderService, Dp) =>
        {
          Dp.Observability.Log($"[Stream] Receive data: Customer:{dto.CustomerName}; TaxId:{dto.CustomerTaxID}; Total:{dto.Total}");    
        });
        
        // Receiving generic event
        Subscribe<IOrderService>((dto, orderService, Dp) =>
        {
            Dp.Observability.Log(dto);
        });
    }
}