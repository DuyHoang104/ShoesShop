using ShoesShop.Domain.Categories.Enums;
using ShoesShop.Domain.Commons.Enums;

namespace ShoesShop.Domain.Categories.Dtos;

public class CategoryDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public string Description { get; set; } 

    public CategoryStatus Status { get; set; }

    public DateTime LastActionTimeStamp { get; set; }

    public int CreateBy { get; set; }

    public DateTime CreateTimeStamp { get; set; }

    public int LastActionBy { get; set; }

    public LastAction LastAction { get; set; }
}