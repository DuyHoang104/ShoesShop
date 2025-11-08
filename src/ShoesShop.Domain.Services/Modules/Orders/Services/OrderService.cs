using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Crosscutting.Utilities.Exceptions;
using ShoesShop.Domain.Modules.Carts.Entities;
using ShoesShop.Domain.Modules.Carts.Services;
using ShoesShop.Domain.Modules.Commons.Repositories;
using ShoesShop.Domain.Modules.Orders.Dtos;
using ShoesShop.Domain.Modules.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.Orders.Entities;
using ShoesShop.Domain.Modules.Orders.Services;
using ShoesShop.Domain.Modules.Shares.Entities;
using ShoesShop.Domain.Modules.Users.Dtos;
using ShoesShop.Domain.Modules.Users.Entities;
using ShoesShop.Infrastructure.Data.UOW;

namespace ShoesShop.Domain.Services.Modules.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Order, int> _orderRepository;
        private readonly IGenericRepository<User, int> _userRepository;
        private readonly IGenericRepository<Address, int> _addressRepository;
        private readonly IGenericRepository<CartItem, int> _cartItemRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICartService _cartService;

        public OrderService(
            IGenericRepository<Order, int> orderRepository,
            IGenericRepository<User, int> userRepository,
            IGenericRepository<Address, int> addressRepository,
            IHttpContextAccessor httpContextAccessor,
            IGenericRepository<CartItem, int> cartItemRepository,
            ICartService cartService,
            IUnitOfWork unitOfWork
        )
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _addressRepository = addressRepository;
            _httpContextAccessor = httpContextAccessor;
            _cartService = cartService;
            _cartItemRepository = cartItemRepository;
            _unitOfWork = unitOfWork;
        }

        private int GetUserIdFromClaims()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User not logged in");
            return int.Parse(userIdClaim);
        }

        public async Task<List<OrderDto>> GetAllOrderAsync()
        {
            var userId = GetUserIdFromClaims();

            var orders = await _orderRepository.GetAllAsync(
                include: q => q
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Images)
                    .Include(o => o.Address)
            );
            
            var userOrders = orders
                .Where(o => o.UserId == userId);

            var orderDtos = userOrders.Select(order =>
            {
                decimal total = order.OrderDetails.Sum(d => d.Subtotal);
                return new OrderDto
                {
                    Id = order.Id,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    ShippingFee = order.ShippingFee,
                    Discount = order.Discount,
                    TotalAmount = (total* (1 - (order.Discount ?? 0) )) + (order.ShippingFee ?? 0),
                    OrderDetails = order.OrderDetails.Select(d => new OrderDetailItemDto
                    {
                        ProductName = d.Product?.Name,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Subtotal = d.Subtotal,
                        ProductSize = d.Product?.Sizes,
                        ProductImage = d.Product?.Images?.FirstOrDefault()?.Url
                    }).ToList(),
                };
            }).ToList();

            return orderDtos;
        }

        public async Task<OrderDetailDto> CreateOrderAsync(OrderCheckoutDto orderDto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var userId = GetUserIdFromClaims();

                var user = await _userRepository.GetByIdAsync(userId)
                    ?? throw new InvalidOperationException("User not found");

                Address? address;

                if (orderDto.SameAddress)
                {
                    address = (await _addressRepository.GetAllAsync(a => a.UserId == userId && a.IsDefault))
                        .FirstOrDefault()
                        ?? throw new InvalidOperationException("Default address not found");

                    orderDto.ReceiverAddress = address.AddressLine1;
                    orderDto.ReceiverName = user.UserName;
                    orderDto.ReceiverPhone = user.Phone;
                }
                else
                {
                    address = new Address(
                        user,
                        orderDto.ReceiverAddress ?? string.Empty,
                        orderDto.City,
                        orderDto.Country,
                        isDefault: false
                    );
                }

                var order = new Order(
                    user: user,
                    address: address,
                    paymentMethod: orderDto.PaymentMethod,
                    paymentStatus: orderDto.PaymentStatus,
                    receiverName: orderDto.ReceiverName ?? user.UserName,
                    receiverPhone: orderDto.ReceiverPhone ?? user.Phone,
                    receiverAddress: orderDto.ReceiverAddress ?? address?.AddressLine1 ?? string.Empty,
                    note: orderDto.Note,
                    shippingFee: orderDto.ShippingFee,
                    discount: orderDto.Discount
                );

                var cartItems = (await _cartItemRepository.GetAllAsync(
                    include: q => q.Include(c => c.Product).ThenInclude(p => p.Images)))
                    .Where(c => c.UserId == userId)
                    .ToList();

                if (cartItems.Count == 0)
                    throw new InvalidOperationException("Your cart is empty.");

                decimal subtotal = 0;

                foreach (var item in cartItems)
                {
                    if (item.Product == null)
                        throw new InvalidOperationException("Cart item missing product data.");

                    var unitPrice = item.Product.Price;
                    var subtotalItem = unitPrice * item.Quantity;

                    subtotal += subtotalItem;

                    var orderDetail = new OrderDetail(
                        order: order,
                        product: item.Product,
                        quantity: item.Quantity,
                        subtotal: subtotalItem
                    );

                    order.AddOrderDetail(orderDetail);

                    await _cartItemRepository.DeleteAsync(item);
                }

                await _cartItemRepository.SaveChangesAsync();

                var totalAmount = (subtotal + (orderDto.ShippingFee ?? 0)) * (1 - (orderDto.Discount ?? 0));

                await _orderRepository.InsertAsync(order);
                await _orderRepository.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OrderDetailDto
                {
                    Id = order.Id,
                    ReceiverName = order.ReceiverName,
                    ReceiverPhone = order.ReceiverPhone,
                    ReceiverAddress = order.ReceiverAddress,
                    ReceiverCity = address?.City ?? "",
                    ReceiverCountry = address?.Country ?? "",
                    Note = order.Note,
                    ShippingCost = order.ShippingFee ?? 0,
                    DiscountValue = order.Discount ?? 0,
                    PaymentMethod = order.PaymentMethod,
                    PaymentStatus = order.PaymentStatus,
                    OrderDate = order.OrderDate,
                    TotalAmount = totalAmount,
                    OrderDetails = order.OrderDetails.Select(d => new OrderDetailItemDto
                    {
                        ProductName = d.Product?.Name ?? "Unknown",
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Subtotal = d.Subtotal,
                        ProductImage = d.Product?.Images?.FirstOrDefault()?.Url ?? "/images/defaultpng"
                    }).ToList(),
                    Address = address == null ? null : new AddressDto
                    {
                        AddressLine1 = address.AddressLine1,
                        City = address.City,
                        Country = address.Country,
                        IsDefault = address.IsDefault
                    }
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderDetailDto> GetOrderDetailByIdAsync(int orderId)
        {
            var order = (await _orderRepository.GetAllAsync(
                include: q => q
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Images)
                    .Include(o => o.Address)
            )).FirstOrDefault(o => o.Id == orderId)
            ?? throw new BusinessException("Order not found");

            decimal total = order.OrderDetails.Sum(d => d.Subtotal);

            return new OrderDetailDto
            {
                Id = order.Id,
                ReceiverName = order.ReceiverName,
                ReceiverPhone = order.ReceiverPhone,
                ReceiverAddress = order.Address?.AddressLine1 ?? string.Empty,
                ReceiverCity = order.Address?.City ?? string.Empty,
                ReceiverCountry = order.Address?.Country ?? string.Empty,
                Note = order.Note,
                ShippingCost = order.ShippingFee ?? 0,
                DiscountValue = order.Discount ?? 0,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                OrderDate = order.OrderDate,
                TotalAmount = (total + (order.ShippingFee ?? 0)) * (1 - (order.Discount ?? 0)),
                OrderDetails = order.OrderDetails.Select(d => new OrderDetailItemDto
                {
                    ProductName = d.Product?.Name ?? "Unknown",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.Subtotal,
                    ProductImage = d.Product?.Images?.FirstOrDefault()?.Url ?? "/images/default.png"
                }).ToList(),
                Address = order.Address == null ? null : new AddressDto
                {
                    AddressLine1 = order.Address.AddressLine1,
                    City = order.Address.City,
                    Country = order.Address.Country,
                    IsDefault = order.Address.IsDefault
                }
            };
        }
        
        public async Task<decimal> CalculateOrderTotalAsync(decimal shippingFee = 0, decimal discountRate = 0)
        {
            var items = await _cartService.GetByUserIdAsync();
            var subtotal = items.Sum(x => x.TotalPrice);
            var discountAmount = subtotal * discountRate;
            var finalTotal = subtotal - discountAmount + shippingFee;
            return finalTotal;
        }
    }
}
