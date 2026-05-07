using MyShop.Domain.Enums;

namespace MyShop.Application.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        public Guid OrderId { get; set; }
        public DeliveryStatusOptions Status { get; set; }
    }
}
