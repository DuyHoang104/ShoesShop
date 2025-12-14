using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Orders.Dtos;
using ShoesShop.Domain.Modules.User.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Shares.Entities;
using ShoesShop.Domain.Modules.User.Users.Dtos;
using ShoesShop.Domain.Modules.User.Users.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Users.Entities;
using ShoesShop.Domain.Modules.User.Users.Enums;
using ShoesShop.Domain.Modules.User.Users.Services;

namespace ShoesShop.Domain.Services.Modules.Users.Users.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User, int> _userRepository;

    public UserService(IGenericRepository<User, int> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetAllInfoUsersDto> GetAllInfoUsersAsync(int userId)
    {
        var users = await _userRepository.GetAllAsync(
            include: q => q
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                    .ThenInclude(o => o.OrderDetails)
        );

        var user = users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
            throw new KeyNotFoundException($"User with id {userId} not found.");

        var userDto = new GetAllInfoUsersDto
        {
            ID = user.Id,
            UserName = user.UserName,
            DateOfBirth = user.DateOfBirth,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            Gender = user.Gender,
            AvatarUrl = user.AvatarUrl,
            Status = user.Status,
            Role = user.Role,

            Addresses = user.Addresses?.Select(a => new AddressDto
            {
                AddressLine1 = a.AddressLine1,
                City = a.City,
                Country = a.Country,
                IsDefault = a.IsDefault
            }).ToList() ?? [],

            Orders = user.Orders?.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                ShippingFee = o.ShippingFee,
                Discount = o.Discount,
                TotalAmount = o.TotalAmount,
                PaymentMethod = o.PaymentMethod,
                PaymentStatus = o.PaymentStatus,
                SubTotal = (o.OrderDetails == null || !o.OrderDetails.Any())
                            ? 0m
                            : o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity),

                OrderDetails = o.OrderDetails.Select(od => new OrderDetailItemDto
                {
                    ProductID = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice
                }).ToList()
            }).ToList() ?? []
        };

        return userDto;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync(
            include: q => q.Include(u => u.Addresses)
        );

        return users.Select(user => new UserDto
        {
            ID = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            DateOfBirth = user.DateOfBirth,
            Status = user.Status,
            Role = user.Role,
            AvatarUrl = user.AvatarUrl,
            Addresses = user.Addresses?.Select(a => new AddressDto
            {
                AddressLine1 = a.AddressLine1,
                City = a.City,
                Country = a.Country,
                IsDefault = a.IsDefault
            }).ToList() ?? []
        }).ToList();
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId) 
            ?? throw new KeyNotFoundException($"User with id {userId} not found.");

        return new UserDto
        {
            ID = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            DateOfBirth = user.DateOfBirth,
            Status = user.Status,
            Role = user.Role,
            AvatarUrl = user.AvatarUrl,
            Addresses = user.Addresses?.Select(a => new AddressDto
            {
                AddressLine1 = a.AddressLine1,
                City = a.City,
                Country = a.Country,
                IsDefault = a.IsDefault
            }).ToList() ?? []
        };
    }

    public async Task<UserDto> LoginAsync(LoginCommandDto loginCommandDto)
    {
        var user = await _userRepository.GetAsync(x 
            => x.UserName == loginCommandDto.UserName 
            && x.Password == loginCommandDto.Password);

        if (user != null)
        {
            return new UserDto
            {
                ID = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                Status = user.Status,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
            };
        }

        return null;
    }
    
    public async Task<UserDto> RegisterAsync(RegisterCommandDto createUserDto)
    {
        var user = await _userRepository.GetAsync(x 
            => x.UserName == createUserDto.UserName
            || x.Email == createUserDto.Email
            || x.Phone == createUserDto.Phone);

        if (user != null)
        {
            throw new InvalidOperationException("User already exists with the same username, email, or phone.");
        }

        user = new User
        {
            UserName = createUserDto.UserName,
            Password = createUserDto.Password,
            DateOfBirth = createUserDto.DateOfBirth,
            Email = createUserDto.Email,
            Phone = createUserDto.Phone,
            Status = UserStatus.InConfirm,
            Gender = createUserDto.Gender,
            AvatarUrl = createUserDto.AvatarUrl,
            Role = createUserDto.Role = UserAccountRole.Customer
        };

        if (createUserDto.Addresses != null && createUserDto.Addresses.Count != 0)
        {
            foreach (var addrDto in createUserDto.Addresses)
            {
                var address = new Address(
                    user,
                    addrDto.AddressLine1,
                    addrDto.City,
                    addrDto.Country,
                    isDefault: true
                );

                user.AddAddress(address);
            }
        }

        await _userRepository.InsertAsync(user);
        await _userRepository.SaveChangesAsync();

        return new UserDto
        {
            ID = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role,
            Addresses = user.Addresses.Select(a => new AddressDto
            {
                AddressLine1 = a.AddressLine1,
                City = a.City,
                Country = a.Country,
                IsDefault = a.IsDefault
            }).ToList()
        };
    }

    public async Task UpdateRoleAsync(UserDto user)
    {
        var existingUser = await _userRepository.GetByIdAsync(user.ID) ?? throw new InvalidOperationException("User not found.");
        existingUser.Role = user.Role;
        await _userRepository.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(UserDto user)
    {
        var existingUser = await _userRepository.GetByIdAsync(user.ID) ?? throw new InvalidOperationException("User not found.");
        existingUser.Status = user.Status;
        await _userRepository.SaveChangesAsync();
    }
}