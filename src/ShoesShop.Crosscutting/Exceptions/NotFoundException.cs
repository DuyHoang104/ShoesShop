namespace ShoesShop.Crosscutting.Utilities.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message) : base(message)
        {}
    }   
}