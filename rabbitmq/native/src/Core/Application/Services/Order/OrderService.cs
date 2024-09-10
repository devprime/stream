namespace Application.Services.Order;
public class OrderService : ApplicationService<IOrderState>, IOrderService
{
    public OrderService(IOrderState state, IDp dp) : base(state, dp)
    {
    }



    public void AddOrderExchangeDirect(Model.Order command)
    {
        Dp.Pipeline(Execute: () =>
        {
            var order = command.ToDomain();
            Dp.Attach(order);
            order.AddOrderExchangeDirect();
        });
    }

    public void AddOrderExchangeFanout(Model.Order command)
    {
        Dp.Pipeline(Execute: () =>
        {
            var order = command.ToDomain();
            Dp.Attach(order);
            order.AddOrderExchangeFanout();
        });
    }

    public void AddOrderExchangeTopic(Model.Order command)
    {
        Dp.Pipeline(Execute: () =>
        {
            var order = command.ToDomain();
            Dp.Attach(order);
            order.AddOrderExchangeTopic();
        });
    }

    public void AddOrderExchangeHeaders(Model.Order command)
    {
        Dp.Pipeline(Execute: () =>
        {
            var order = command.ToDomain();
            Dp.Attach(order);
            order.AddOrderExchangeHeaders();
        });
    }
}