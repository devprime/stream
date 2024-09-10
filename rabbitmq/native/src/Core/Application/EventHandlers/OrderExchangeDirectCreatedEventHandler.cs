using System.Threading.Tasks;
using DevPrime.Stack.Foundation.Stream;
using Domain.DomainEvents;
namespace Application.EventHandlers;
public class OrderExchangeDirectCreatedEventHandler : DevPrime.Stack.Foundation.Application.EventHandler<OrderExchangeDirectCreated>
{
    public OrderExchangeDirectCreatedEventHandler(IDp dp) : base(dp)
    {
    }

    public override dynamic Handle(OrderExchangeDirectCreated orderExchangeDirectCreated)
    {
      // Getting Initial Information
        var success = false;
        var order = orderExchangeDirectCreated.Get<Domain.Aggregates.Order.Order>();
        var destination = Dp.Settings.Default("stream.orderevents");
        var eventName = "OrderCreated";


        // ------------------------------------------------------------
        //  Direct: Exchange, Queue and Bind
        //-------------------------------------------------------------

        //Direct
        var exchangeNameDirect1 = "exchange-direct-1";
        var exchangeNameDirect2 = "exchange-direct-2";

        //Direct with Custom Properties
        var directCustomExchangeProperties = new CustomExchangeProperties()
        {
            Durable = true, // Explicitly durable
            AutoDelete = false, // Do not auto-delete
            Arguments = new Dictionary<string, object>
            {
                { "x-message-ttl", 60000 }, // Message time-to-live in milliseconds
                { "x-max-length", 10000 } // Maximum number of messages in the queue
            }
        };


        //Create Exchange
        Dp.Stream.AddExchangeRabbitMQ(exchangeNameDirect1, ExchangeType.Direct, null);
        Dp.Stream.AddExchangeRabbitMQ(exchangeNameDirect2, ExchangeType.Direct, directCustomExchangeProperties);

        //Create Queue
        var queueNameDirect1 = "queue-direct-1";
        var routingKeyDirect1 = "queue-direct-1";

        var queueNameDirect2 = "queue-direct-2";
        var routingKeyDirect2 = "queue-direct-2";

        var queueCustomProperties = new CustomQueueProperties()
        {
            Durable = true,
            Exclusive = false,
            AutoDelete = false,
            Arguments = new Dictionary<string, object>
            {
                { "x-message-ttl", 900000 } // TTL
            }
        };

       
        Dp.Stream.AddQueueRabbitMQ(queueNameDirect1);
        Dp.Stream.AddQueueRabbitMQ(queueNameDirect2, queueCustomProperties);

        //Bind Exchange / Queue
        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameDirect1, ExchangeType.Direct, queueNameDirect1,
            routingKeyDirect1);
        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameDirect2, ExchangeType.Direct, queueNameDirect2,
            routingKeyDirect2);


        //Publish
        var eventDataDirect1 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };

        var eventDataDirect2 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };
        
        
        //Change Customer Name
        eventDataDirect1.CustomerName = $"Demo1-{@eventDataDirect1.CustomerName}";
        eventDataDirect2.CustomerName = $"Demo2-{@eventDataDirect2.CustomerName}";

        //Custom Properties
        var publishCustomMessageProperties = new CustomMessageProperties();
        publishCustomMessageProperties.Persistent = true;
        publishCustomMessageProperties.Expiration = 300000; // TTL 

        Dp.Stream.PublishRabbitMQ(exchangeNameDirect1, ExchangeType.Direct, routingKeyDirect1, eventName,
            eventDataDirect1, null);
        Dp.Stream.PublishRabbitMQ(exchangeNameDirect2, ExchangeType.Direct, routingKeyDirect2, eventName,
            eventDataDirect2, publishCustomMessageProperties);

        
        //******************************************************************
        //                              Subscribe
        //******************************************************************
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameDirect1, ExchangeType.Direct, queueNameDirect1);
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameDirect2, ExchangeType.Direct, queueNameDirect2);

        
        Task.Run(async () =>
        {
            
            Dp.Observability.Log("Starting cleanup");
            await Task.Delay(5000);// Waiting 5 seconds before remove
            
            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameDirect1);
            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameDirect2);

            Dp.Stream.RemoveQueueRabbitMQ(queueNameDirect1);
            Dp.Stream.RemoveQueueRabbitMQ(queueNameDirect2);

            Dp.Stream.RemoveExchangeRabbitMQ(exchangeNameDirect1);
            Dp.Stream.RemoveExchangeRabbitMQ(exchangeNameDirect2);

            Dp.Observability.Log("Finish cleanup");
        });

        success = true;

        return success;
    }
}