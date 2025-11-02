namespace ShoesShop.Web.Modules.Product.Dtos
{
    public class SearchModalDto
    {
        public int? CategoryId { get; set; }
        public string? Brand { get; set; }
        public string? Color { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string>? Sizes { get; set; }
        public string? SortBy { get; set; }
    }
}