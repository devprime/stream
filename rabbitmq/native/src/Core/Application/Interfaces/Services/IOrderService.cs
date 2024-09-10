namespace Application.Interfaces.Services;
public interface IOrderService
{
     void AddOrderExchangeDirect(Application.Services.Order.Model.Order command);
     void AddOrderExchangeFanout(Application.Services.Order.Model.Order command);
     void AddOrderExchangeTopic(Application.Services.Order.Model.Order command);
     void AddOrderExchangeHeaders(Application.Services.Order.Model.Order command);
}