using ShoesShop.Domain.Categories.Enums;

namespace ShoesShop.Web.Categories.Dtos;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public CategoryStatus Status { get; set; }
}