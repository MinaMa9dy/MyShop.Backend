using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyShop.CORE.Dtos.Order;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.DTOs.Coupon;
using MyShop.CORE.Entities;
using MyShop.CORE.Entities.OrderEntities;
using MyShop.CORE.Helpers;

using MyShop.CORE.Enums;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Identity;
using MyShop.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Implmentations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;
        private readonly ICouponService _couponService;

        public OrderService(IUnitOfWork unitOfWork,IIdentityService identityService, IMapper mapper, INotificationService notificationService, ICacheService cacheService, ICouponService couponService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _mapper = mapper;
            _notificationService = notificationService;
            _cacheService = cacheService;
            _couponService = couponService;

        }

        public async Task<Result<OrderDto>> CreateOrder(Guid CustomerId, AddOrderDto dto)
        {

            var customer = await _unitOfWork.Users.FindAsync(u => u.Id == CustomerId);
            if(await _identityService.IsInRoleAsync(customer.Id.ToString(),"Customer") == false)
                return Result<OrderDto>.Failure("User is not a customer", "400");
            

            if (customer is null)
                return Result<OrderDto>.Failure("User not found", "404");

            var cartItems = (List<CartItem>) await _unitOfWork.CartItems.FindAllAsync(ci => ci.CustomerId == CustomerId, includes: new[] { "ProductVariant.Product", "ProductVariant.Product.productPhotos" });

            if (cartItems == null || !cartItems.Any()) return Result<OrderDto>.Failure("Cart is empty", "400");

            Order order = new Order();
            List<OrderItem> orderItems = new List<OrderItem>();
            Guid sellerId = cartItems[0].ProductVariant.Product.SupplierId;
            CouponResponseDto appliedCouponData = null;
            if (dto.CouponCode.HasValue)
            {
                var coupon = await _unitOfWork.Coupons.FindAsync(c => c.Id == dto.CouponCode.Value);
                if (coupon is null)
                    return Result<OrderDto>.Failure("Coupon not found", "404");

                var couponResult = await _couponService.ValidateCouponAsync(coupon.CouponCode, CustomerId);
                if (!couponResult.IsSuccess)
                {
                    return Result<OrderDto>.Failure(couponResult.Error.Message, "400");
                }
                var userCoupon = await _unitOfWork.UserCoupons.FindAsync(uc => uc.CouponId == coupon.Id && uc.CustomerId == CustomerId);
                if(userCoupon is null)
                    return Result<OrderDto>.Failure("Coupon not assigned to You", "400");
                userCoupon.UserUsageCount++;
                coupon.UsedCount++;


            }

            decimal subTotal = 0;
            foreach (var cartItem in cartItems)
            {
                if (cartItem.ProductVariant.Product.SupplierId != sellerId) return Result<OrderDto>.Failure("Not all orders from the same place", "400");

                decimal originalPrice = cartItem.ProductVariant.NewPrice;
                decimal finalUnitPrice = originalPrice;

                if (appliedCouponData != null && appliedCouponData.ItemPrices != null && appliedCouponData.ItemPrices.TryGetValue(cartItem.ProductVariant.ProductId, out decimal discountedPrice))
                {
                    finalUnitPrice = discountedPrice;
                }

                OrderItem item = new OrderItem
                {
                    ProductVariantId = cartItem.ProductVariantId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = finalUnitPrice,
                    ProductName = cartItem.ProductVariant.Product.Name,
                    ProductPhotoPath = cartItem.ProductVariant.Product.productPhotos?.FirstOrDefault(pp => pp.IsMain)?.RelativePath
                    
                };

                subTotal += originalPrice * cartItem.Quantity;
                orderItems.Add(item);
            }

            order.SellerId = sellerId;
            order.CustomerId = CustomerId;
            order.CreatedAt = DateTime.UtcNow;
            order.Street = dto.Street;
            order.City = dto.City;
            order.BuyerName = customer.FirstName + " " + customer.LastName;
            order.BuyerEmail = customer.Email;
            order.BuyerPhone = dto.PhoneNumber;
            order.Status = DeliveryStatusOptions.Pending;
            order.orderItems = orderItems;
            order.SubTotal = subTotal;
            order.DiscountAmount = appliedCouponData?.TotalDiscount ?? 0;
            order.TotalAmount = appliedCouponData?.FinalSubtotal ?? subTotal;
            order.AppliedCouponCode = appliedCouponData != null ? dto.CouponCode.Value : null;

            _unitOfWork.CartItems.DeleteRange(cartItems);
            await _unitOfWork.Orders.AddAsync(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0) return Result<OrderDto>.Failure("Failed to Save the Order", "500");

            var orderResponse = _mapper.Map<OrderDto>(order);

            try
            {
                await _notificationService.BroadcastNewOrderAsync(sellerId, orderResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting order notification: {ex.Message}");
            }

            return Result<OrderDto>.Success(orderResponse);
        }

        public async Task<Result<List<OrderDto>>> GetOrdersByCustomerId(Guid customerId)
        {
            if (customerId == null) return Result<List<OrderDto>>.Failure("customerId is null", "400");

            var orders = await _unitOfWork.Orders.FindAllAsync(o => o.CustomerId == customerId, orderBy: o => o.CreatedAt, orderByDirection: OrderByOptions.Descending, includes: new[] { nameof(Order.orderItems) });
            var ordersResponse = _mapper.Map<List<OrderDto>>(orders);
            return Result<List<OrderDto>>.Success(ordersResponse);
        }

        public async Task<Result<List<OrderDto>>> GetOrdersBySellerId(Guid SellerId)
        {
            if (SellerId == null) return Result<List<OrderDto>>.Failure("SellerId is Empty", "400");

            string cacheKey = $"orders:sellerId:{SellerId}";
            var cached = await _cacheService.GetAsync<List<OrderDto>>(cacheKey);
            if (cached != null) return Result<List<OrderDto>>.Success(cached);

            var orders = await _unitOfWork.Orders.FindAllAsync(o => o.SellerId == SellerId, orderBy: o => o.CreatedAt, orderByDirection: OrderByOptions.Descending, includes: new[] { nameof(Order.orderItems) });
            var ordersResponse = _mapper.Map<List<OrderDto>>(orders);
            await _cacheService.SetAsync(cacheKey, ordersResponse, TimeSpan.FromMinutes(20));
            return Result<List<OrderDto>>.Success(ordersResponse);
        }

        public async Task<Result<PageResult<OrderDto>>> GetAllOrdersAsync(int page = 1, int pageSize = 10)
        {
            var orders = await _unitOfWork.Orders.FindAllAsync(null, skip: (page - 1) * pageSize, take: pageSize, orderBy: o => o.CreatedAt, orderByDirection: OrderByOptions.Descending, includes: new[] { nameof(Order.orderItems) });
            var totalOrders = await _unitOfWork.Orders.CountAsync();

            var ordersResponse = _mapper.Map<List<OrderDto>>(orders);
            var pageResult = new PageResult<OrderDto>
            {
                Items = ordersResponse,
                TotalItems = totalOrders,
                Page = page,
                PageSize = pageSize
            };

            return Result<PageResult<OrderDto>>.Success(pageResult);
        }

        public async Task<Result<OrderDto>> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _unitOfWork.Orders.FindAsync(o => o.Id == orderId, new[] { nameof(Order.orderItems) });
            if (order == null) return Result<OrderDto>.Failure("Order not found", "404");

            var orderResponse = _mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Success(orderResponse);
        }

        public async Task<Result<OrderDto>> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Orders.FindAsync(o => o.Id == dto.OrderId, new[] { nameof(Order.orderItems) });
            if (order == null) return Result<OrderDto>.Failure("Order not found", "404");

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Orders.Update(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0) return Result<OrderDto>.Failure("Failed to update order status", "500");

            var orderResponse = _mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Success(orderResponse);
        }

        public async Task<Result<OrderDto>> CancelOrderAsync(Guid userId, Guid orderId)
        {
            var order = await _unitOfWork.Orders.FindAsync(o => o.Id == orderId, new[] { nameof(Order.orderItems) });
            var customer = await _unitOfWork.Users.FindAsync(u => u.Id == userId);

            if (order == null)
                return Result<OrderDto>.Failure("Order not found", "404");

            var roles = await _identityService.GetRolesAsync(userId.ToString());
            bool isAdminOrSeller = roles.Contains("Admin") || roles.Contains("Seller");
            if (customer.Email  != order.BuyerEmail && !isAdminOrSeller)
                return Result<OrderDto>.Failure("You can only cancel your own orders", "403");


            if (order.Status == DeliveryStatusOptions.Delivered || order.Status == DeliveryStatusOptions.Canceled)
            {
                return Result<OrderDto>.Failure("Cannot cancel an order that is already delivered or canceled", "400");
            }

            order.Status = DeliveryStatusOptions.Canceled;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Orders.Update(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0) return Result<OrderDto>.Failure("Failed to cancel order", "500");

            var orderResponse = _mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Success(orderResponse);
        }
    }
}
