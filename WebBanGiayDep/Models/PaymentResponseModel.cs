using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanGiayDep.Models
{
    public class PaymentResponseModel
    {
        public int ID { get; set; }
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public bool Success { get; set; }
        public bool checkSignature { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
        public string Amount { get; set; }
        public int PackegeID { get; set; }
        public string RspCode { get; set; }
        public string Message { get; set; }
    }
}