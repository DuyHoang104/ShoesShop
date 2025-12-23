using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Modules.Shares.Image.Entities;
using ShoesShop.Domain.Modules.Shares.Image.Enums;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Orders.Dtos;
using ShoesShop.Domain.Modules.User.Orders.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Shares.Entities;
using ShoesShop.Domain.Modules.User.Users.Dtos;
using ShoesShop.Domain.Modules.User.Users.Dtos.Commands;
using ShoesShop.Domain.Modules.User.Users.Entities;
using ShoesShop.Domain.Modules.User.Users.Enums;
using ShoesShop.Domain.Modules.User.Users.Services;
using ShoesShop.Infrastructure.Data.UOW;

namespace ShoesShop.Domain.Services.Modules.Users.Users.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User, int> _userRepository;
    private readonly IGenericRepository<ImageUser, int> _imageUserRepository;
    private readonly CloudinaryService _cloudinaryService;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IGenericRepository<User, int> userRepository,
        IGenericRepository<ImageUser, int> imageUserRepository,
        CloudinaryService cloudinaryService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _imageUserRepository = imageUserRepository;
        _cloudinaryService = cloudinaryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllInfoUsersDto> GetAllInfoUsersAsync(int userId)
    {
        var users = await _userRepository.GetAllAsync(
            predicate: u => u.Id == userId,
            include: q => q
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                    .ThenInclude(o => o.OrderDetails)
        );

        var user = users.FirstOrDefault()
            ?? throw new KeyNotFoundException($"User {userId} not found");

        var avatar = await _imageUserRepository.GetAsync(i =>
            i.OwnerType == OwnerType.User &&
            i.OwnerId == user.Id
        );

        return new GetAllInfoUsersDto
        {
            ID = user.Id,
            UserName = user.UserName,
            DateOfBirth = user.DateOfBirth,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            Gender = user.Gender,
            Status = user.Status,
            Role = user.Role,
            AvatarUrl = avatar?.Url ?? "/images/default.png",

            Addresses = user.Addresses.Select(a => new AddressDto
            {
                AddressLine1 = a.AddressLine1,
                City = a.City,
                Country = a.Country,
                IsDefault = a.IsDefault
            }).ToList(),

            Orders = user.Orders.Select(o =>
            {
                var subTotal = o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity);

                return new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    ShippingFee = o.ShippingFee,
                    Discount = o.Discount,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod,
                    PaymentStatus = o.PaymentStatus,
                    SubTotal = subTotal,

                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailItemDto
                    {
                        ProductId = od.ProductId,
                        Quantity = od.Quantity,
                        UnitPrice = od.UnitPrice
                    }).ToList()
                };
            }).ToList()
        };
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync(
            include: q => q.Include(u => u.Addresses)
        );

        var userIds = users.Select(u => u.Id).ToList();

        var avatars = await _imageUserRepository.GetAllAsync(i =>
            i.OwnerType == OwnerType.User &&
            userIds.Contains(i.OwnerId)
        );

        return users.Select(user =>
        {
            var avatar = avatars.FirstOrDefault(a => a.OwnerId == user.Id);

            return new UserDto
            {
                ID = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone ?? string.Empty,
                DateOfBirth = user.DateOfBirth,
                Status = user.Status,
                Role = user.Role,
                AvatarUrl = avatar?.Url ?? "/images/default.png",
                Addresses = user.Addresses.Select(a => new AddressDto
                {
                    AddressLine1 = a.AddressLine1,
                    City = a.City,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                }).ToList()
            };
        }).ToList();
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException($"User {userId} not found");

        var avatar = await _imageUserRepository.GetAsync(i =>
            i.OwnerType == OwnerType.User &&
            i.OwnerId == user.Id
        );

        return new UserDto
        {
            ID = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            DateOfBirth = user.DateOfBirth,
            Status = user.Status,
            Role = user.Role,
            AvatarUrl = avatar?.Url ?? "/images/default.png"
        };
    }

    public async Task<UserDto?> LoginAsync(LoginCommandDto dto)
    {
        var user = await _userRepository.GetAsync(u =>
            u.UserName == dto.UserName &&
            u.Password == dto.Password
        );

        if (user == null)
            return null;

        var avatar = await _imageUserRepository.GetAsync(i =>
            i.OwnerType == OwnerType.User &&
            i.OwnerId == user.Id
        );

        return new UserDto
        {
            ID = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            DateOfBirth = user.DateOfBirth,
            Status = user.Status,
            Role = user.Role,
            AvatarUrl = avatar?.Url ?? "/images/default.png"
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterCommandDto dto)
    {
        var existed = await _userRepository.GetAsync(u =>
            u.UserName == dto.UserName ||
            u.Email == dto.Email ||
            u.Phone == dto.Phone);

        if (existed != null)
            throw new InvalidOperationException("User already exists");

        var user = new User
        {
            UserName = dto.UserName,
            Password = dto.Password,
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email,
            Phone = dto.Phone,
            Gender = dto.Gender,
            Status = UserStatus.InConfirm,
            Role = UserAccountRole.Customer
        };

        string? avatarUrl = null;
        string? avatarPublicId = null;

        using var transaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (dto.AvatarUrl != null)
            {
                var image = await _cloudinaryService.UploadImageAsync(
                    dto.AvatarUrl,
                    $"Users/{dto.UserName.Replace(" ", "-")}"
                ) ?? throw new InvalidOperationException("Upload avatar failed");

                avatarUrl = image.Url;
                avatarPublicId = image.FileName;
            }

            if (!string.IsNullOrWhiteSpace(avatarUrl))
                user.SetAvatar(avatarUrl, avatarPublicId);
            
            if (dto.Addresses.Count != 0)
            {
                foreach (var addressDto in dto.Addresses)
                {
                    var address = new Address
                    {
                        AddressLine1 = addressDto.AddressLine1,
                        City = addressDto.City,
                        Country = addressDto.Country,
                        IsDefault = true
                    };
                    user.AddAddress(address);
                }
            }

            await _userRepository.InsertAsync(user);
            await _userRepository.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();

            if (!string.IsNullOrWhiteSpace(avatarPublicId))
                await _cloudinaryService.DeleteImageAsync(avatarPublicId);
            throw;
        }

        return new UserDto
        {
            ID = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone,
            DateOfBirth = user.DateOfBirth,
            Gender = user.Gender,
            Role = user.Role,
            AvatarUrl = avatarUrl
        };
    }

    public async Task UpdateRoleAsync(UserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.ID)
            ?? throw new InvalidOperationException("User not found");

        user.Role = dto.Role;
        await _userRepository.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(UserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(dto.ID)
            ?? throw new InvalidOperationException("User not found");

        user.Status = dto.Status;
        await _userRepository.SaveChangesAsync();
    }
}
