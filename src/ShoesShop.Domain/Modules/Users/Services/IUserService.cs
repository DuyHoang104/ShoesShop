using ShoesShop.Domain.Modules.Users.Dtos;
using ShoesShop.Domain.Modules.Users.Dtos.Commands;

namespace ShoesShop.Domain.Modules.Users.Services;

public interface IUserService
{
    public Task<UserDto> RegisterAsync(RegisterCommandDto createUserDto);

    public Task<UserDto> LoginAsync(LoginCommandDto loginCommandDto);
}