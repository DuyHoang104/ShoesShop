using Microsoft.AspNetCore.Mvc;
using ShoesShop.Domain.Modules.Categories.Services;
using ShoesShop.Domain.Modules.Products.Dtos.Commands;
using ShoesShop.Domain.Modules.Products.Services;
using ShoesShop.Web.Modules.Product.Dtos;

namespace ShoesShop.Web.Modules.Product.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        private readonly CloudinaryService _cloudinaryService;

        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment env, CloudinaryService cloudinaryService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(SearchModalDto searchDto)
        {
            var products = await _productService.GetAllCategoriesAsync();

            if (searchDto.CategoryId.HasValue)
                products = products.Where(p => p.Categories.Any(c => c.Id == searchDto.CategoryId.Value)).ToList();

            if (!string.IsNullOrEmpty(searchDto.Brand))
                products = products.Where(p => p.Brand.Equals(searchDto.Brand, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(searchDto.Color))
                products = products
                    .Where(p =>
                        !string.IsNullOrEmpty(p.Color) &&
                        p.Color.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Any(c => c.Trim().Equals(searchDto.Color, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

            if (searchDto.MinPrice.HasValue && searchDto.MaxPrice.HasValue)
                products = products.Where(p => p.Price >= searchDto.MinPrice && p.Price <= searchDto.MaxPrice).ToList();

            products = searchDto.SortBy switch
            {
                "name" => products.OrderBy(p => p.Name).ToList(),
                "price" => products.OrderBy(p => p.Price).ToList(),
                "brand" => products.OrderBy(p => p.Brand).ToList(),
                _ => products
            };

            if (searchDto.Sizes != null && searchDto.Sizes.Count != 0)
            {
                products = products
                    .Where(p =>
                        !string.IsNullOrEmpty(p.Sizes) &&
                        searchDto.Sizes.All(selectedSize =>
                            p.Sizes
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim())
                                .Contains(selectedSize)
                        )
                    ).ToList();
            }

            ViewBag.SelectedSizes = searchDto.Sizes ?? [];
            return View("~/Modules/Product/Views/Index.cshtml", products);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string? query)
        {
            var products = await _productService.SearchAsync(query);

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return View("~/Modules/Product/Views/Index.cshtml", products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var createModalDto = new CreateModalDto
            {
                Name = string.Empty,
                Brand = string.Empty
            };

            var categories = await _categoryService.GetAllAsync();
            createModalDto.Categories = categories.ToList();

            return View("~/Modules/Product/Views/Create.cshtml", createModalDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateModalDto dto)
        {
            if (!ModelState.IsValid)
            {
                dto.Categories = (await _categoryService.GetAllAsync()).ToList();
                return View("~/Modules/Product/Views/Create.cshtml", dto);
            }

            var uploadedImages = new List<(string Folder, string FileName, string Url)>();

            foreach (var file in dto.ImageFiles)
            {
                var result = await _cloudinaryService.UploadImageAsync(file, $"Products/{dto.Name.Replace(" ", "-")}");
                if (result != null)
                    uploadedImages.Add(result.Value);
            }

            var uploadedData = uploadedImages.Select(img => img.Url).ToList();

                
            var sizes = dto.Sizes != null && dto.Sizes.Any()
            ? string.Join(",", dto.Sizes)
            : string.Empty;

            var productDto = new CreateProductDto
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description ?? string.Empty,
                Quantity = dto.Quantity,
                SaleOff = dto.SaleOff,
                Status = dto.Status,
                Brand = dto.Brand,
                Color = dto.Color ?? string.Empty,
                Sizes = sizes,
                ImageUrl = uploadedData,
                Categories = dto.Categories
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name)) 
                    .ToList(),
            };

            await _productService.CreateAsync(productDto);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return Redirect("/404");
            }

            return View("~/Modules/Product/Views/Detail.cshtml", product);
        }
    }
}