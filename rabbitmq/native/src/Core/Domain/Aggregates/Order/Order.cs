using Domain.DomainEvents;

namespace Domain.Aggregates.Order;
public class Order : AggRoot
{
    public string CustomerName { get; private set; }
    public string CustomerTaxID { get; private set; }
    public IList<Item> Items { get; private set; }
    public double Total { get; private set; }
    

    public virtual void AddOrderExchangeDirect()
    {
        Dp.Pipeline(Execute: () =>
        {
            Dp.Attach(Items);
            ValidFields();
            ID = Guid.NewGuid();
            IsNew = true;
            var success = Dp.ProcessEvent<bool>(new OrderExchangeDirectCreated());
            if (success)
            {
                Dp.ProcessEvent(new OrderCreated());
            }

        });
    }
    
    public virtual void AddOrderExchangeFanout()
    {
        Dp.Pipeline(Execute: () =>
        {
            Dp.Attach(Items);
            ValidFields();
            ID = Guid.NewGuid();
            IsNew = true;
            var success = Dp.ProcessEvent<bool>(new OrderExchangeFanoutCreated());
            if (success)
            {
                Dp.ProcessEvent(new OrderCreated());
            }

        });
    }
    
    
    public virtual void AddOrderExchangeTopic()
    {
        Dp.Pipeline(Execute: () =>
        {
            Dp.Attach(Items);
            ValidFields();
            ID = Guid.NewGuid();
            IsNew = true;
            var success = Dp.ProcessEvent<bool>(new OrderExchangeTopicCreated());
            if (success)
            {
                Dp.ProcessEvent(new OrderCreated());
            }
     
        });
    }
    
    public virtual void AddOrderExchangeHeaders()
    {
        Dp.Pipeline(Execute: () =>
        {
            Dp.Attach(Items);
            ValidFields();
            ID = Guid.NewGuid();
            IsNew = true;
            var success = Dp.ProcessEvent<bool>(new OrderExchangeHeadersCreated());
            if (success)
            {
                Dp.ProcessEvent(new OrderCreated());
            }
        });
    }
    
    public void AddItem(Item item)
    {
        if (item != null && Items != null)
        {
            var myItems = Items.Where(p => p.SKU == item.SKU).FirstOrDefault();
            if (myItems != null)
                myItems.Sum(item.Amount);
            else
                Items.Add(item);
        }
    }
    public Order(Guid id, string customerName, string customerTaxID, IEnumerable<Domain.Aggregates.Order.Item> items, double total)
    {
        ID = id;
        CustomerName = customerName;
        CustomerTaxID = customerTaxID;
        Items = items?.ToList();
        Total = total;
    }
    public Order()
    {
    }
   
    private void ValidFields()
    {
        if (String.IsNullOrWhiteSpace(CustomerName))
            Dp.Notifications.Add("CustomerName is required");
        if (String.IsNullOrWhiteSpace(CustomerTaxID))
            Dp.Notifications.Add("CustomerTaxID is required");
        Dp.Notifications.ValidateAndThrow();
    }
}