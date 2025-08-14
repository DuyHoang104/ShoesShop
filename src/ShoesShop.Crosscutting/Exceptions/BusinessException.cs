namespace ShoesShop.Crosscutting.Utilities.Exceptions
{
    public class BusinessException : BaseException
    {
        public BusinessException(string message) : base(message)
        {}
    }
}