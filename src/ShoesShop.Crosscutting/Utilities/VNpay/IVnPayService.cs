using Microsoft.AspNetCore.Http;

namespace ShoesShop.Crosscutting.Utilities.VNpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPayRequestModel model);
        VnPaymentResponseModel PaymentExcute(IQueryCollection collections);
    }
}