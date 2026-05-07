using MyShop.Domain.RepositoryInterfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShop.Application.DTOs.Order;
using MyShop.Application.DTOs.Product;
using MyShop.Application.DTOs.Coupon;
using MyShop.Domain.Entities;
using MyShop.Domain.Entities.OrderEntities;
using MyShop.Application.Common;

using MyShop.Domain.Enums;
using MyShop.Application.Common.ResultPattern;
using MyShop.Domain.Identity;
using MyShop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Services
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

            var customer = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == CustomerId);
            if (await _identityService.IsInRoleAsync(customer.Id.ToString(), nameof(RoleOptions.Customer)) == false)
                return Result<OrderDto>.Failure("User is not a customer", ErrorCode.FORBIDDEN);
            

            if (customer is null)
                return Result<OrderDto>.Failure("User not found", ErrorCode.NOT_FOUND);

            var cartItems = await _unitOfWork.CartItems.Query()
                .Where(ci => ci.CustomerId == CustomerId)
                .Include(ci => ci.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.productPhotos)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any()) return Result<OrderDto>.Failure("Cart is empty", ErrorCode.VALIDATION_ERROR);

            Order order = new Order();
            List<OrderItem> orderItems = new List<OrderItem>();
            Guid sellerId = cartItems[0].ProductVariant.Product.SupplierId;
            CouponResponseDto appliedCouponData = null;
            if (dto.CouponCode.HasValue)
            {
                var coupon = await _unitOfWork.Coupons.Query().FirstOrDefaultAsync(c => c.Id == dto.CouponCode.Value);
                if (coupon is null)
                    return Result<OrderDto>.Failure("Coupon not found", ErrorCode.NOT_FOUND);

                var couponResult = await _couponService.ValidateCouponAsync(coupon.CouponCode, CustomerId);
                if (!couponResult.Success)
                {
                    return Result<OrderDto>.Failure(couponResult.Error!.Message, ErrorCode.VALIDATION_ERROR);
                }
                var userCoupon = await _unitOfWork.UserCoupons.Query(false).FirstOrDefaultAsync(uc => uc.CouponId == coupon.Id && uc.CustomerId == CustomerId);
                if(userCoupon is null)
                    return Result<OrderDto>.Failure("Coupon not assigned to You", ErrorCode.VALIDATION_ERROR);
                userCoupon.UserUsageCount++;
                coupon.UsedCount++;
            }

            decimal subTotal = 0;
            foreach (var cartItem in cartItems)
            {
                if (cartItem.ProductVariant.Product.SupplierId != sellerId) return Result<OrderDto>.Failure("Not all orders from the same place", ErrorCode.VALIDATION_ERROR);

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
            if (result == 0) return Result<OrderDto>.Failure("Failed to Save the Order", ErrorCode.SERVER_ERROR);

            var orderResponse = _mapper.Map<OrderDto>(order);

            try
            {
                await _notificationService.BroadcastNewOrderAsync(sellerId, orderResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting order notification: {ex.Message}");
            }

            return Result<OrderDto>.Created(orderResponse, "Order created successfully");
        }

        public async Task<Result<List<OrderDto>>> GetOrdersByCustomerId(Guid customerId)
        {
            if (customerId == null) return Result<List<OrderDto>>.Failure("customerId is null", ErrorCode.VALIDATION_ERROR);

            var orders = await _unitOfWork.Orders.Query()
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .Include(o => o.orderItems)
                .ToListAsync();
            var ordersResponse = _mapper.Map<List<OrderDto>>(orders);
            return Result<List<OrderDto>>.Ok(ordersResponse);
        }

        public async Task<Result<List<OrderDto>>> GetOrdersBySellerId(Guid SellerId)
        {
            if (SellerId == null) return Result<List<OrderDto>>.Failure("SellerId is Empty", ErrorCode.VALIDATION_ERROR);

            string cacheKey = $"orders:sellerId:{SellerId}";
            var cached = await _cacheService.GetAsync<List<OrderDto>>(cacheKey);
            if (cached != null) return Result<List<OrderDto>>.Ok(cached);

            var orders = await _unitOfWork.Orders.Query()
                .Where(o => o.SellerId == SellerId)
                .OrderByDescending(o => o.CreatedAt)
                .Include(o => o.orderItems)
                .ToListAsync();
            var ordersResponse = _mapper.Map<List<OrderDto>>(orders);
            await _cacheService.SetAsync(cacheKey, ordersResponse, TimeSpan.FromMinutes(20));
            return Result<List<OrderDto>>.Ok(ordersResponse);
        }

        public async Task<Result<IEnumerable<OrderDto>>> GetAllOrdersAsync(int page = 1, int pageSize = 10)
        {
            var orders = await _unitOfWork.Orders.Query()
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.orderItems)
                .ToListAsync();
            var totalOrders = await _unitOfWork.Orders.Query().CountAsync();

            var ordersResponse = _mapper.Map<List<OrderDto>>(orders);

            var meta = new Meta
            {
                Total = totalOrders,
                Page = page,
                PerPage = pageSize
            };

            return Result<IEnumerable<OrderDto>>.Ok(ordersResponse, "Success", meta);
        }

        public async Task<Result<OrderDto>> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _unitOfWork.Orders.Query()
                .Include(o => o.orderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return Result<OrderDto>.Failure("Order not found", ErrorCode.NOT_FOUND);

            var orderResponse = _mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Ok(orderResponse);
        }

        public async Task<Result<OrderDto>> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
        {
            var order = await _unitOfWork.Orders.Query(false)
                .Include(o => o.orderItems)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);
            if (order == null) return Result<OrderDto>.Failure("Order not found", ErrorCode.NOT_FOUND);

            order.Status = dto.Status;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Orders.Update(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0) return Result<OrderDto>.Failure("Failed to update order status", ErrorCode.SERVER_ERROR);

            var orderResponse = _mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Ok(orderResponse);
        }

        public async Task<Result<OrderDto>> CancelOrderAsync(Guid userId, Guid orderId)
        {
            var order = await _unitOfWork.Orders.Query(false)
                .Include(o => o.orderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            var customer = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == userId);

            if (order == null)
                return Result<OrderDto>.Failure("Order not found", ErrorCode.NOT_FOUND);

            var roles = await _identityService.GetRolesAsync(userId.ToString());
            bool isAdminOrSeller = roles.Contains("Admin") || roles.Contains("Seller");
            if (customer.Email  != order.BuyerEmail && !isAdminOrSeller)
                return Result<OrderDto>.Failure("You can only cancel your own orders", ErrorCode.FORBIDDEN);


            if (order.Status == DeliveryStatusOptions.Delivered || order.Status == DeliveryStatusOptions.Canceled)
            {
                return Result<OrderDto>.Failure("Cannot cancel an order that is already delivered or canceled", ErrorCode.VALIDATION_ERROR);
            }

            order.Status = DeliveryStatusOptions.Canceled;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Orders.Update(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result == 0) return Result<OrderDto>.Failure("Failed to cancel order", ErrorCode.SERVER_ERROR);

            var orderResponse = _mapper.Map<OrderDto>(order);
            return Result<OrderDto>.Ok(orderResponse, "Order cancelled successfully");
        }
    }
}
