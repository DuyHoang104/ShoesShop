using ShoesShop.Domain.Modules.User.Categories.Enums;

namespace ShoesShop.Web.Modules.Admins.Categories.Dtos;
public class CategoryDtos
{
    public int Id { get; set;}
    public string Name { get; set;}
    public string Description { get; set;}
    public DateTime CreatedAt { get; set;} = DateTime.Now;
    public CategoryStatus Status { get; set;}
}