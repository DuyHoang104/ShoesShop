namespace ShoesShop.Domain.Orders.Dtos.Commands;

public class RevenueDto
{
    public string Label { get; set; }    // Ví dụ: "01/2025" hoặc "2025-12-31"
    public decimal Revenue { get; set; }
}