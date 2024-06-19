using System.Threading.Tasks;
using System.Web;
using WebBanGiayDep.Models;

namespace WebBanGiayDep.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        Task<PaymentResponseModel> PaymentExecute(HttpRequest collections);
        string GetIpAddress(HttpContext context);
    }
}
