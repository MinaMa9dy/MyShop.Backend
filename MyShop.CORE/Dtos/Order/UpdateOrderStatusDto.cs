using MyShop.CORE.Enums;

namespace MyShop.CORE.Dtos.Order
{
    public class UpdateOrderStatusDto
    {
        public Guid OrderId { get; set; }
        public DeliveryStatusOptions Status { get; set; }
    }
}
