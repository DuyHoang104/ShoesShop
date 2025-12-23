using Microsoft.EntityFrameworkCore;
using ShoesShop.Crosscutting.Utilities.Exceptions;
using ShoesShop.Domain.Modules.Shares.Review.Entity;
using ShoesShop.Domain.Modules.User.Carts.Entities;
using ShoesShop.Domain.Modules.User.Carts.Services;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Orders.Dtos;
using ShoesShop.Domain.Modules.User.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Orders.Entities;
using ShoesShop.Domain.Modules.User.Orders.Enums;
using ShoesShop.Domain.Modules.User.Orders.Services;
using ShoesShop.Domain.Modules.User.Shares.Entities;
using ShoesShop.Domain.Modules.User.Users.Dtos;
using ShoesShop.Domain.Modules.User.Users.Entities;
using ShoesShop.Infrastructure.Data.UOW;

namespace ShoesShop.Domain.Services.Modules.Users.Orders.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Order, int> _orderRepository;
    private readonly IGenericRepository<User, int> _userRepository;
    private readonly IGenericRepository<Address, int> _addressRepository;
    private readonly IGenericRepository<Cart, int> _cartRepository;
    private readonly IGenericRepository<Review, int> _reviewRepository = null!;
    private readonly ICartService _cartService;

    public OrderService(
        IGenericRepository<Order, int> orderRepository,
        IGenericRepository<User, int> userRepository,
        IGenericRepository<Address, int> addressRepository,
        IGenericRepository<Cart, int> cartRepository,
        IGenericRepository<Review, int> reviewRepository,
        ICartService cartService,
        IUnitOfWork unitOfWork
    )
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _addressRepository = addressRepository;
        _cartRepository = cartRepository;
        _cartService = cartService;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<OrderDto>> GetAllOrderAsync(int? userId)
    {
        var orders = await _orderRepository.GetAllAsync(
            include: q => q
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.Images)
                .Include(o => o.Address)
                .Include(o => o.User)
        );

        if (userId.HasValue)
        {
            orders = orders.Where(o => o.UserId == userId.Value).ToList();
        }

        var orderDtos = orders.Select(order =>
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
                TotalAmount = (total * (1 - (order.Discount ?? 0))) + (order.ShippingFee ?? 0),
                OrderDetails = order.OrderDetails.Select(d => new OrderDetailItemDto
                {
                    ProductId = d.Product.Id,
                    ProductName = d.Product?.Name,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Subtotal = d.Subtotal,
                    Size = d.Size,
                    ProductImage = d.Product?.Images?.FirstOrDefault()?.Url
                }).ToList(),
            };
        }).ToList();

        return orderDtos;
    }
    
    public async Task<OrderDetailDto> CreateOrderAsync(OrderCheckoutDto orderDto, int userId)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            var user = await _userRepository.GetByIdAsync(userId);

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

            var carts = (await _cartRepository.GetAllAsync(
                include: q => q.Include(c => c.Product).ThenInclude(p => p.Images)))
                .Where(c => c.UserId == userId)
                .ToList();

            if (carts.Count == 0)
                throw new InvalidOperationException("Your cart is empty.");

            decimal subtotal = 0;

            foreach (var item in carts)
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
                    subtotal: subtotalItem,
                    size: item.Size
                );

                order.AddOrderDetail(orderDetail);

                await _cartRepository.DeleteAsync(item);
            }

            await _cartRepository.SaveChangesAsync();

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
            predicate: o => o.Id == orderId,
            include: q => q
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Address)
                .Include(o => o.User)
                    .ThenInclude(u => u.Addresses)
        )).FirstOrDefault()
            ?? throw new BusinessException("Order not found");

        decimal total = order.OrderDetails.Sum(d => d.Subtotal);

        var userId = order.UserId;
        
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
            Status = order.Status,
            OrderDate = order.OrderDate,
            TotalAmount = (total + (order.ShippingFee ?? 0)) * (1 - (order.Discount ?? 0)),

            OrderDetails = order.OrderDetails.Select(d => new OrderDetailItemDto
            {
                Id = d.Id,
                ProductId = d.Product.Id,
                ProductName = d.Product?.Name ?? "Unknown",
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Subtotal = d.Subtotal,
                Size = d.Size,
                ProductImage = d.Product?.Images?.FirstOrDefault()?.Url ?? "/images/default.png",
            }).ToList(),

            Address = order.Address == null ? null : new AddressDto
            {
                AddressLine1 = order.Address.AddressLine1,
                City = order.Address.City,
                Country = order.Address.Country,
                IsDefault = order.Address.IsDefault
            },

            User = order.User == null ? null : new UserDto
            {
                UserName = order.User.UserName,
                Email = order.User.Email,
                Phone = order.User.Phone ?? string.Empty,
                Addresses = order.User.Addresses?.Select(a => new AddressDto
                {
                    AddressLine1 = a.AddressLine1,
                    City = a.City,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                }).ToList() ?? []
            }
        };
    }


    public async Task<decimal> CalculateOrderTotalAsync(int userId, decimal shippingFee = 0, decimal discountRate = 0)
    {
        var items = await _cartService.GetByUserIdAsync(userId);
        var subtotal = items.Sum(x => x.TotalPrice);
        var discountAmount = subtotal * discountRate;
        var finalTotal = subtotal - discountAmount + shippingFee;
        return finalTotal;
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new BusinessException("Order not found");

        order.Status = newStatus;

        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> GetUserInfoForOrderAsync(int userId, int orderId)
    {
        var order = await _orderRepository.GetAsync(o => o.Id == orderId && o.UserId == userId);
        if (order == null)
        {
            return false;
        }
        return true;
    }
}