using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Modules.Carts.Entities;
using ShoesShop.Domain.Modules.Carts.Services;
using ShoesShop.Domain.Modules.Commons.Repositories;
using ShoesShop.Domain.Modules.Orders.Dtos;
using ShoesShop.Domain.Modules.Orders.Entities;
using ShoesShop.Domain.Modules.Orders.Services;
using ShoesShop.Domain.Modules.Shares.Entities;
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
                throw new Exception("User not logged in");
            return int.Parse(userIdClaim);
        }

       public async Task CreateOrderAsync(OrderDto orderDto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var userId = GetUserIdFromClaims();

                var user = await _userRepository.GetByIdAsync(userId)
                    ?? throw new Exception("User not found");

                Address? address;

                if (orderDto.SameAddress)
                {
                    address = (await _addressRepository.GetAllAsync(a => a.UserId == userId && a.IsDefault))
                        .FirstOrDefault()
                        ?? throw new Exception("Default address not found");

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

                    await _addressRepository.InsertAsync(address);
                    await _addressRepository.SaveChangesAsync();
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
                    throw new Exception("Your cart is empty.");

                decimal totalWithoutShipping = 0;

                foreach (var item in cartItems)
                {
                    if (item.Product == null)
                        throw new Exception("Cart item missing product data.");

                    var subtotal = item.Quantity * item.Product.Price;

                    var discountedSubtotal = Math.Round(subtotal * (1 - (orderDto.Discount ?? 0)), 2);

                    totalWithoutShipping += discountedSubtotal;

                    var orderDetail = new OrderDetail(
                        order: order,
                        product: item.Product,
                        quantity: item.Quantity,
                        subtotal: discountedSubtotal + (orderDto.ShippingFee ?? 0)
                    );

                    order.AddOrderDetail(orderDetail);

                    await _cartItemRepository.DeleteAsync(item);
                }

                await _cartItemRepository.SaveChangesAsync();
                await _orderRepository.InsertAsync(order);
                await _orderRepository.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<decimal> CalculateOrderTotalAsync(decimal shippingFee = 0, decimal discount = 0)
        {
            var items = await _cartService.GetByUserIdAsync();

            var subtotal = items.Sum(x => x.TotalPrice);
            var discountAmount = subtotal * discount;

            var finalTotal = (subtotal - discountAmount) + shippingFee;

            return finalTotal;
        }
    }
}