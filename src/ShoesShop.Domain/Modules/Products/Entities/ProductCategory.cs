using ShoesShop.Domain.Modules.Categories.Entities;
using ShoesShop.Domain.Modules.Commons.Entities;

namespace ShoesShop.Domain.Modules.Products.Entities;
public class ProductCategory : BaseEntity<int>
{
    private int _productId;
    public int ProductId
    {
        get => _productId;
    }

    private Product _product = null!;
    public Product Product
    {
        get => _product;
        internal set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(Product), "Product cannot be null.");
            }
            
            _productId = value.Id;
            _product = value;
        }
    }

    private int _categoryId;
    public int CategoryId
    {
        get => _categoryId;
    }

    private Category _category = null!;
    public Category Category
    {
        get => _category;
        internal set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(Category), "Category cannot be null.");
            }
            
            _categoryId = value.Id;
            _category = value;
        }
    }

    public ProductCategory(Product product, Category category)
    {
        Product = product;
        Category = category;
    }

    public ProductCategory() { }
}