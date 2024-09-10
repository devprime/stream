using System.Threading.Tasks;
using DevPrime.Stack.Foundation.Stream;
using Domain.DomainEvents;
namespace Application.EventHandlers;
public class OrderExchangeFanoutCreatedEventHandler : DevPrime.Stack.Foundation.Application.EventHandler<OrderExchangeFanoutCreated>
{
    public OrderExchangeFanoutCreatedEventHandler(IDp dp) : base(dp)
    {
    }
    public override dynamic Handle(OrderExchangeFanoutCreated orderExchangeFanoutCreated)
    {
        // Getting Initial Information
        var success = false;
        var order = orderExchangeFanoutCreated.Get<Domain.Aggregates.Order.Order>();
        var destination = Dp.Settings.Default("stream.orderevents");
        var eventName = "OrderCreated";
        
        // ------------------------------------------------------------
        //  Fanout: Exchange, Queue and Bind
        //-------------------------------------------------------------

        //Fanout
        var exchangeNameFanout1 = "exchange-fanout-1";
        var exchangeNameFanoutCustom2 = "exchange-fanout-2";
  
        //Fanout with Custom Properties
        var fanoutCustomExchangeProperties = new CustomExchangeProperties()
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
        Dp.Stream.AddExchangeRabbitMQ(exchangeNameFanout1, ExchangeType.Fanout, null);
        Dp.Stream.AddExchangeRabbitMQ(exchangeNameFanoutCustom2, ExchangeType.Fanout, fanoutCustomExchangeProperties);

        
        //Create Queue
        var queueNameFanout1 = "queue-fanout-1";
        var queueNameFanout2 = "queue-fanout-2";
        var queueNameFanoutCustom1 = "queue-fanout-Custom-1";
        var queueNameFanoutCustom2 = "queue-fanout-Custom-2";
        Dp.Stream.AddQueueRabbitMQ(queueNameFanout1);
        Dp.Stream.AddQueueRabbitMQ(queueNameFanout2);
        Dp.Stream.AddQueueRabbitMQ(queueNameFanoutCustom1);
        Dp.Stream.AddQueueRabbitMQ(queueNameFanoutCustom2);
        
        //Bind Exchange / Queue
        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameFanout1, ExchangeType.Fanout, queueNameFanout1, null,null);
        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameFanout1, ExchangeType.Fanout, queueNameFanout2,null,null);
        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameFanoutCustom2, ExchangeType.Fanout, queueNameFanoutCustom1, null,null);
        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameFanoutCustom2, ExchangeType.Fanout, queueNameFanoutCustom2,null,null);

  
        //Publish
        var eventDataFanout1 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };

        var eventDataFanout2 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };

        //Change Customer Name
        eventDataFanout1.CustomerName = $"Demo1-{@eventDataFanout1.CustomerName}";
        eventDataFanout2.CustomerName = $"Demo2-{@eventDataFanout2.CustomerName}";

        //Custom Properties
        var publishCustomMessageProperties = new CustomMessageProperties();
        publishCustomMessageProperties.Persistent = true;
        publishCustomMessageProperties.Expiration = 300000; // TTL 

        Dp.Stream.PublishRabbitMQ(exchangeNameFanout1, ExchangeType.Fanout, null, eventName,
            eventDataFanout1,null);
        Dp.Stream.PublishRabbitMQ(exchangeNameFanoutCustom2, ExchangeType.Fanout, null, eventName,
            eventDataFanout2, publishCustomMessageProperties);
        
        //******************************************************************
        //                              Subscribe
        //******************************************************************
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameFanout1, ExchangeType.Fanout, queueNameFanout1);
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameFanout1, ExchangeType.Fanout, queueNameFanout2);
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameFanoutCustom2, ExchangeType.Fanout, queueNameFanoutCustom1);
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameFanoutCustom2, ExchangeType.Fanout, queueNameFanoutCustom2);

        
        Task.Run(async () =>
        {
            
            Dp.Observability.Log("Starting cleanup");
            await Task.Delay(5000); // Waiting 5 seconds before remove

            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameFanout1);
            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameFanout2);
            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameFanoutCustom1);
            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameFanoutCustom2);

            Dp.Stream.RemoveQueueRabbitMQ(queueNameFanout1);
            Dp.Stream.RemoveQueueRabbitMQ(queueNameFanout2);
            Dp.Stream.RemoveQueueRabbitMQ(queueNameFanoutCustom1);
            Dp.Stream.RemoveQueueRabbitMQ(queueNameFanoutCustom2);

            Dp.Stream.RemoveExchangeRabbitMQ(exchangeNameFanout1);
            Dp.Stream.RemoveExchangeRabbitMQ(exchangeNameFanoutCustom2);

            Dp.Observability.Log("Finish cleanup");
        });
        
        success = true;
        return success;
    }
}