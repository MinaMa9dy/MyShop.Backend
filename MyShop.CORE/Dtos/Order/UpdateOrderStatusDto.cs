using MyShop.CORE.Enums;

namespace MyShop.CORE.Dtos.Order
{
    public class UpdateOrderStatusDto
    {
        public DeliveryStatusOptions Status { get; set; }
    }
}
