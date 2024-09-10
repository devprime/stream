using System.Threading.Tasks;
using DevPrime.Stack.Foundation.Stream;
using Domain.DomainEvents;
namespace Application.EventHandlers;
public class OrderExchangeTopicCreatedEventHandler : DevPrime.Stack.Foundation.Application.EventHandler<OrderExchangeTopicCreated>
{
    public OrderExchangeTopicCreatedEventHandler(IDp dp) : base(dp)
    {
    }
    public override dynamic Handle(OrderExchangeTopicCreated orderExchangeTopicCreated)
    {
        // Getting Initial Information
        var success = false;
        var order = orderExchangeTopicCreated.Get<Domain.Aggregates.Order.Order>();
        var destination = Dp.Settings.Default("stream.orderevents");
        var eventName = "OrderCreated";
        
        // ------------------------------------------------------------
        //  Topic: Exchange, Queue and Bind
        //-------------------------------------------------------------

        //Topic
        var exchangeNameTopic1 = "exchange-topic-1";
  
  
        //Topic with Custom Properties
        var topicCustomExchangeProperties = new CustomExchangeProperties()
        {
            Durable = true, // Explicitly durable
        };
        
        //Create Exchange
        Dp.Stream.AddExchangeRabbitMQ(exchangeNameTopic1, ExchangeType.Topic, topicCustomExchangeProperties);
  
        //Create Queue
        var queueNameTopic1 = "queue-topic-1";
        var queueNameTopic2 = "queue-topic-2";
        Dp.Stream.AddQueueRabbitMQ(queueNameTopic1);
        Dp.Stream.AddQueueRabbitMQ(queueNameTopic2);
 
        
        //Bind Exchange / Queue
        var routingKeyTopicBind1 = "quick.ecommerce.#";
        var routingKeyTopicBind2 = "#.logs";
        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameTopic1, ExchangeType.Topic, queueNameTopic1, routingKeyTopicBind1,null);
        Dp.Stream.AddBindingExchangeQueueRabbitMQ(exchangeNameTopic1, ExchangeType.Topic, queueNameTopic2, routingKeyTopicBind2,null);

        
   
        
        //Publish
  
        var eventDataTopic1 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };

        var eventDataTopic2 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };
        
        
        var eventDataTopic3 = new OrderCreatedEventDTO()
        {
            ID = order.ID,
            CustomerName = order.CustomerName,
            CustomerTaxID = order.CustomerTaxID,
            Total = order.Total
        };
        
        //Change Customer Name
        eventDataTopic1.CustomerName = $"Demo1-{eventDataTopic1.CustomerName}";
        eventDataTopic1.CustomerName = $"Demo2-{eventDataTopic2.CustomerName}";
        eventDataTopic3.CustomerName = $"Demo3-{eventDataTopic3.CustomerName}";
        
       
        //Custom Properties
        var topicCustomMessageProperties = new CustomMessageProperties();
        topicCustomMessageProperties.Expiration = 8000000;
        topicCustomMessageProperties.Persistent = true;
        
        var routingKeyTopic1 = "quick.ecommerce.logs"; // Distribution: queueNameTopic1 and queueNameTopic2
        var routingKeyTopic2 = "quick.ecommerce.approved"; // Distribution: queueNameTopic1
        var routingKeyTopic3 = "quick.financial.logs"; // Distribution: queueNameTopic2

        
        // Publishing with routingKeyTopic1 to distribute to destinations: queueNameTopic1 and queueNameTopic2
        Dp.Stream.PublishRabbitMQ(exchangeNameTopic1, ExchangeType.Topic, routingKeyTopic1, eventName, eventDataTopic1, topicCustomMessageProperties);

        // Publishing with routingKeyTopic2 to distribute to destination: queueNameTopic1
        Dp.Stream.PublishRabbitMQ(exchangeNameTopic1, ExchangeType.Topic, routingKeyTopic2, eventName, eventDataTopic2, null);

        // Publishing with routingKeyTopic3 to distribute to destination: queueNameTopic2
        Dp.Stream.PublishRabbitMQ(exchangeNameTopic1, ExchangeType.Topic, routingKeyTopic3, eventName, eventDataTopic3, null);

        //******************************************************************
        //                              Subscribe
        //******************************************************************
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameTopic1, ExchangeType.Topic, queueNameTopic1);
        Dp.Stream.AddSubscribeRabbitMQ(exchangeNameTopic1, ExchangeType.Topic, queueNameTopic2);

        
        
        Task.Run(async () =>
        {
           
            Dp.Observability.Log("Starting cleanup");
            await Task.Delay(5000); // Waiting 5 seconds before remove
            
            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameTopic1);
            Dp.Stream.RemoveSubscribeRabbitMQ(queueNameTopic2);
            
            Dp.Stream.RemoveQueueRabbitMQ(queueNameTopic1);
            Dp.Stream.RemoveQueueRabbitMQ(queueNameTopic2);

            Dp.Stream.RemoveExchangeRabbitMQ(exchangeNameTopic1);

            Dp.Observability.Log("Finish cleanup");
        });
        
        
        success = true;
        return success;
    }
}