using System.Net.Sockets;
using System.Net;
using WebBanGiayDep.Models;
using System.Web;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace WebBanGiayDep.Services
{
    public class VnPayService : IVnPayService
    {
        //private readonly IConfiguration _configuration;

        //public VnPayService( IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(ConfigurationManager.AppSettings["Vnpay:TmnCode"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = ConfigurationManager.AppSettings["PaymentCallBack:ReturnUrl"];

            pay.AddRequestData("vnp_Version", ConfigurationManager.AppSettings["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", ConfigurationManager.AppSettings["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", ConfigurationManager.AppSettings["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", ConfigurationManager.AppSettings["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", ConfigurationManager.AppSettings["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.OrderCode}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl =
                pay.CreateRequestUrl(ConfigurationManager.AppSettings["Vnpay:BaseUrl"], ConfigurationManager.AppSettings["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        public Task<PaymentResponseModel> PaymentExecute(HttpRequest collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, ConfigurationManager.AppSettings["Vnpay:HashSecret"]);

            // Assuming GetFullResponseData returns a PaymentResponseModel
            return Task.FromResult(response);
        }

        public string GetIpAddress(HttpContext context)
        {
            var pay = new VnPayLibrary();

            var ipAddress = pay.GetIpAddress(context);
          
            return ipAddress;
        }

    }
}
