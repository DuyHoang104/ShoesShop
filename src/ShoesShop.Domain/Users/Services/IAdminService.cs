using ShoesShop.Domain.Users.Dtos;

namespace ShoesShop.Domain.Users.Services;

public interface IAdminService
{
    public Task<AdminDto> GetAllInfomationAsync();
}