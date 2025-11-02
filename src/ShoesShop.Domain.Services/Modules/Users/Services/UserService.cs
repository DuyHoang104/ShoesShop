using ShoesShop.Domain.Modules.Commons.Repositories;
using ShoesShop.Domain.Modules.Shares.Entities;
using ShoesShop.Domain.Modules.Users.Dtos;
using ShoesShop.Domain.Modules.Users.Dtos.Commands;
using ShoesShop.Domain.Modules.Users.Entities;
using ShoesShop.Domain.Modules.Users.Enums;
using ShoesShop.Domain.Modules.Users.Services;

namespace ShoesShop.Domain.Services.Modules.Users.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User, int> _userRepository;

    public UserService(IGenericRepository<User, int> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> CheckEmailAsync(CheckEmailCommandDto checkEmailCommandDto)
    {
        var email = await _userRepository.GetAsync(x => x.Email == checkEmailCommandDto.Email);
        if (email == null)
        {
            return false;
        }
        
        return true;
    }

    public async Task<UserDto> LoginAsync(LoginCommandDto loginCommandDto)
    {
        var user = await _userRepository.GetAsync(x => x.UserName == loginCommandDto.UserName && x.Password == loginCommandDto.Password) 
            ?? throw new InvalidOperationException("Invalid username or password.");
        return new UserDto
        {
            ID = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Phone = user.Phone ?? string.Empty,
            DateOfBirth = user.DateOfBirth,
            Status = user.Status,
            Role = user.Role
        };
    }
    
    public async Task<UserDto> RegisterAsync(RegisterCommandDto createUserDto)
    {
        var user = await _userRepository.GetAsync(x =>
            x.UserName == createUserDto.UserName
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
            bool isFirst = true;

            foreach (var addrDto in createUserDto.Addresses)
            {
                var address = new Address(
                    user,
                    addrDto.AddressLine1,
                    addrDto.City,
                    addrDto.Country,
                    isDefault: isFirst
                );

                user.AddAddress(address);
                isFirst = false;
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
}