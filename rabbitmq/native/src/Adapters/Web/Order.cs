namespace DevPrime.Web;
public class Order : Routes
{
    public override void Endpoints(WebApplication app)
    {
        //Automatically returns 404 when no result  

      
        app.MapPost("/v1/order-exchange-direct", async (HttpContext http, IOrderService Service, DevPrime.Web.Models.Order.Order command) => await Dp(http).Pipeline(() => Service.AddOrderExchangeDirect(command.ToApplication())));
        app.MapPost("/v1/order-exchange-fanout", async (HttpContext http, IOrderService Service, DevPrime.Web.Models.Order.Order command) => await Dp(http).Pipeline(() => Service.AddOrderExchangeFanout(command.ToApplication())));
        app.MapPost("/v1/order-exchange-topic", async (HttpContext http, IOrderService Service, DevPrime.Web.Models.Order.Order command) => await Dp(http).Pipeline(() => Service.AddOrderExchangeTopic(command.ToApplication())));
        app.MapPost("/v1/order-exchange-headers", async (HttpContext http, IOrderService Service, DevPrime.Web.Models.Order.Order command) => await Dp(http).Pipeline(() => Service.AddOrderExchangeHeaders(command.ToApplication())));

    }
}