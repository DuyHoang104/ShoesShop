using ShoesShop.Domain.Modules.Admin.Admin.Dtos;

namespace ShoesShop.Domain.Modules.Admin.Admin.Services
{
    public interface IAdminService
    {
        public Task<AdminDto> GetAllInfomationAsync();
    }
}