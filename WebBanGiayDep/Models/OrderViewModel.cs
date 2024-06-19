using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebBanGiayDep.Models
{
    public class OrderViewModel
    {
        public int TypePaymentVN { get; set; }
        public int OrderCode { get; set; }
    }
}