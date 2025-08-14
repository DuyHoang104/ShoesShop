    using ShoesShop.Domain.Modules.Products.Entities;
    using ShoesShop.Domain.Modules.Users.Entities;
    using ShoesShop.Domain.Modules.Commons.Entities;

    namespace ShoesShop.Domain.Modules.Carts.Entities
    {
        public class CartItem : BaseEntity<int>
        {
            private int _quantity;
            public int Quantity
            {
                get => _quantity;
                set
                {
                    if (value <= 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Quantity), "Quantity must be greater than 0.");
                    }

                    _quantity = value;
                }
            }

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

            private int _userId;
            public int UserId
            {
                get => _userId;
            }

            private User _user = null!;
            public User User
            {
                get => _user;
                internal set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(User), "User cannot be null.");
                    }

                    _userId = value.Id;
                    _user = value;
                }
            }

            public CartItem(User user, Product product, int quantity)
            {
                User = user;
                Product = product;
                Quantity = quantity;
            }

            public CartItem() { }
        }
    }