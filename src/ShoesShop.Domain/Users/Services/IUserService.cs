using ShoesShop.Domain.Users.Dtos;
using ShoesShop.Domain.Users.Dtos.Commands;

namespace ShoesShop.Domain.Users.Services;

public interface IUserService
{
    public Task<UserDto> RegisterAsync(RegisterCommandDto createUserDto);
    public Task<UserDto> LoginAsync(LoginCommandDto loginCommandDto);
    public Task<List<UserDto>> GetAllUsersAsync();
    public Task<UserDto> GetUserByIdAsync(int userId);
    public Task UpdateStatusAsync(UserDto user);
    public Task UpdateRoleAsync(UserDto user);
    public Task<GetAllInfoUsersQuery> GetAllInfoUsersAsync(int userId);
}