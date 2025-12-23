namespace ShoesShop.Domain.Modules.Shares.Review.Entity
{
    public class Order_class
    {
        private int _employeeRating;
        public int EmployeeRating
        {
            get => _employeeRating;
            private set
            {
                if (value < 1 || value > 5)
                    throw new ArgumentOutOfRangeException(nameof(EmployeeRating));
                _employeeRating = value;
            }
        }

        private int _shipperRating;
        public int ShipperRating
        {
            get => _shipperRating;
            private set
            {
                if (value < 1 || value > 5)
                    throw new ArgumentOutOfRangeException(nameof(ShipperRating));
                _shipperRating = value;
            }
        }

        public int OrderId { get; private set; }

        protected Order_class() { }

        public Order_class(int employeeRating, int shipperRating, int orderId)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(orderId);

            EmployeeRating = employeeRating;
            ShipperRating = shipperRating;
            OrderId = orderId;
        }
    }
}