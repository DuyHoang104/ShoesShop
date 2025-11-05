using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ShoesShop.Crosscutting.Utilities.VNpay
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(HttpContext context, VnPayRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnpayLibrary();

            // 🔹 Dữ liệu cơ bản
            vnpay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            vnpay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:Vnp_TmnCode"]);

            // 🔹 Số tiền (VNPay yêu cầu *100)
            var amount = Math.Round(model.Amount, 0) * 100;
            vnpay.AddRequestData("vnp_Amount", amount.ToString());

            // 🔹 Thông tin đơn hàng
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"] ?? "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"] ?? "vn");
            vnpay.AddRequestData("vnp_OrderInfo", $"Payment for the order {model.OrderId}");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:ReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            // 🔹 Tạo URL thanh toán
            var baseUrl = _configuration["Vnpay:BaseUrl"];
            var hashSecret = _configuration["Vnpay:HashSecret"];

            var paymentUrl = vnpay.CreateRequestUrl(baseUrl, hashSecret);
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExcute(IQueryCollection collections)
        {
            var vnpay = new VnpayLibrary();

            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    vnpay.AddResponseData(key, value.ToString());
            }

            var vnpOrderId = vnpay.GetResponseData("vnp_TxnRef");
            var vnpTransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnpSecureHash = collections.FirstOrDefault(x => x.Key == "vnp_SecureHash").Value;
            var vnpResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnpOrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            // 🔹 Validate chữ ký
            var isValidSignature = vnpay.ValidateSignature(vnpSecureHash, _configuration["Vnpay:HashSecret"]);
            if (!isValidSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    VnPayResponseCode = vnpResponseCode,
                    OrderDescription = "Invalid signature."
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VNPay",
                OrderDescription = vnpOrderInfo,
                OrderId = vnpOrderId,
                TransactionId = vnpTransactionId,
                Token = vnpSecureHash,
                VnPayResponseCode = vnpResponseCode
            };
        }
    }
}