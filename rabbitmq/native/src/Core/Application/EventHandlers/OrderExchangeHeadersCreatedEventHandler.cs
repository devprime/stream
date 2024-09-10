using System.Threading.Tasks;
using DevPrime.Stack.Foundation.Stream;
using Domain.DomainEvents;
namespace Application.EventHandlers;
public class OrderExchangeHeadersCreatedEventHandler : DevPrime.Stack.Foundation.Application.EventHandler<OrderExchangeHeadersCreated>
{
    public OrderExchangeHeadersCreatedEventHandler(IDp dp) : base(dp)
    {
    }
    public override dynamic Handle(OrderExchangeHeadersCreated orderExchangeHeadersCreated)
    {
      
        // Getting Initial Information
        var success = false;
        var order = orderExchangeHeadersCreated.Get<Domain.Aggregates.Order.Order>();
        var destination = Dp.Settings.Default("stream.orderevents");
        var eventName = "OrderCreated";
        
        // ------------------------------------------------------------
        //  Topic: Exchange, Queue and Bind
        //-------------------------------------------------------------

        //Topic
        var exchangeNameHeaders1 = "exchange-headers-1";
        
        //Create Exchange
        Dp.Stream.AddExchangeRabbitMQ(exchangeNameHeaders1, ExchangeType.Headers, null);
  
        //Create Queue
        var queueNameHeaders1 = "queue-headers-1";
        var queueNameHeaders2 = "queue-headers-2";
        Dp.Stream.AddQueueRabbitMQ(queueNameHeaders1);
        Dp.Stream.AddQueueRabbitMQ(queueNameHeaders2);
        
        //Bind Exchange / Queue

        var argumentsHeaders1  = new Dictionary<string, object> 
        { 
            { "format", "pdf" }, 
            { "type", "report" },
            { "x-match", "all" }
        };  
        
        
        var argumentsHeaders2  = new Dictionary<string, object> 
        { 
            { "format", "zip" }, 
            { "x-match", "any" }
        };

        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameHeaders1, ExchangeType.Headers, queueNameHeaders1, null,argumentsHeaders1);
        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameHeaders1, ExchangeType.Headers, queueNameHeaders2, null,argumentsHeaders2);

        

        //Publish
        var routingKeyHeaders = String.Empty;
        var eventDataHeaders1 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };

        var eventDataHeaders2 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };
        
        
        var eventDataHeaders3 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };
        
        
        //Change Customer Name
        eventDataHeaders1.CustomerName = $"Demo1-{eventDataHeaders1.CustomerName}";
        eventDataHeaders2.CustomerName = $"Demo2-{eventDataHeaders2.CustomerName}";
        eventDataHeaders3.CustomerName = $"Demo3-{eventDataHeaders3.CustomerName}";
        
        
        // Publishing with Headers to distribute to destination: queue-headers-1
        var propertiesHeaders1 = new CustomMessageProperties();
        propertiesHeaders1.Arguments = new Dictionary<string, object> { { "format", "pdf" }, { "type", "report" } };
        propertiesHeaders1.Expiration = 670000000;
        Dp.Stream.PublishRabbitMQ(exchangeNameHeaders1, ExchangeType.Headers, routingKeyHeaders, eventName, eventDataHeaders1, propertiesHeaders1);

        // Publishing with Headers to distribute to destination: queue-headers-2
        var propertiesHeaders2 = new CustomMessageProperties();
        propertiesHeaders2.Arguments = new Dictionary<string, object> { { "format", "zip" } };
        Dp.Stream.PublishRabbitMQ(exchangeNameHeaders1, ExchangeType.Headers, routingKeyHeaders, eventName, eventDataHeaders2, propertiesHeaders2);

        // Publishing with Headers to distribute to destination: queue-headers-1
        var propertiesHeaders3 = new CustomMessageProperties();
        propertiesHeaders3.Arguments = new Dictionary<string, object> { { "format", "zip" }, { "type", "image" } };
        Dp.Stream.PublishRabbitMQ(exchangeNameHeaders1, ExchangeType.Headers, routingKeyHeaders, eventName, eventDataHeaders3, propertiesHeaders3);

        
        //******************************************************************
        //                              Subscribe
        //******************************************************************
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameHeaders1, ExchangeType.Headers, queueNameHeaders1);
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameHeaders1, ExchangeType.Headers, queueNameHeaders2);

        
        
        
       
        Task.Run(async () =>
        {
            
            Dp.Observability.Log("Starting cleanup");
            await Task.Delay(5000); // Waiting 5 seconds before remove
            
            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameHeaders1);
            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameHeaders2);
            
            Dp.Stream.RemoveQueueRabbitMQ(queueNameHeaders1);
            Dp.Stream.RemoveQueueRabbitMQ(queueNameHeaders2);

            Dp.Stream.RemoveExchangeRabbitMQ(exchangeNameHeaders1);

            Dp.Observability.Log("Finish cleanup");
        });
        
        
        success = true;
        return success;
    }
}