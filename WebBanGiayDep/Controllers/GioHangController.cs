using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebBanGiayDep.Models;

namespace WebBanGiayDep.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        dbShopGiayDataContext data = new dbShopGiayDataContext();
        public List<GioHang> LayGioHang()
        {
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang == null)
            {
                listGioHang = new List<GioHang>();
                Session["GioHang"] = listGioHang;
            }
            return listGioHang;
        }
        public int LayMaDonHang()
        {
            DONHANG dh = data.DONHANGs.FirstOrDefault(x => x.TinhTrangGiaoHang == false);
            return dh.MaDonHang;
        }
        public ActionResult ThemGioHang(int iMaGiay, string strURL)
        {
            List<GioHang> listGioHang = LayGioHang();
            GioHang sanpham = listGioHang.Find(n => n.iMaGiay == iMaGiay);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMaGiay);
                listGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang != null)
            {
                iTongSoLuong = listGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang != null)
            {
                iTongTien = listGioHang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }
        public ActionResult GioHang()
        {
            List<GioHang> listGioHang = LayGioHang();
            if (listGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(listGioHang);
        }

        public ActionResult GiohangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        //Xoa gio hang 
        public ActionResult XoaGioHang(int iMaSp)
        {
            //lay gio hang tu sesstion
            List<GioHang> listGioHang = LayGioHang();
            //Kiem tra giay da co trong sesstion 
            GioHang sanpham = listGioHang.SingleOrDefault(n => n.iMaGiay == iMaSp);
            //neu ton tai thi cho sua so luong
            if (sanpham != null)
            {
                listGioHang.RemoveAll(n => n.iMaGiay == iMaSp);
                return RedirectToAction("GioHang");
            }
            if (listGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }
        //cap nhap gio hang 
        public ActionResult CapnhatGiohang(int iMaSp, FormCollection f)
        {
            //Lay gio hang tu sesstion
            List<GioHang> listGioHang = LayGioHang();
            //kiem tra giay da co trong sesstion
            GioHang sanpham = listGioHang.SingleOrDefault(n => n.iMaGiay == iMaSp);
            //neu ton tai thi cho sua so luong
            if (sanpham != null)
            {
                sanpham.iSoLuong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        //xoa tat ca gio hang 
        public ActionResult XoaTatcaGiohang()
        {
            //Lay gio hang tu sesstion
            List<GioHang> listGioHang = LayGioHang();
            listGioHang.Clear();
            return RedirectToAction("Index", "Home");
        }
        //hien thi view dathang de cap nhap cac thong tin cho don hang
        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            var itemOrder = data.DONHANGs.FirstOrDefault(x => x.MaKH == kh.MaKH && x.TinhTrangGiaoHang == false);
            if(itemOrder != null)
            {
                List<GioHang> gh = LayGioHang();
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.TongTien = TongTien();
                ViewBag.MaDonHang = LayMaDonHang();
                return View(gh);
            }
            else
            {
                List<GioHang> lstGioHang = LayGioHang();
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.TongTien = TongTien();
                DONHANG dh = new DONHANG();
                List<GioHang> gh = LayGioHang();
                dh.MaKH = kh.MaKH;
                dh.NgayDat = DateTime.Now;
                var ngaygiao = String.Format("{0:MM/dd/yyyy}", DateTime.Now.AddDays(7));
                dh.NgayGiao = DateTime.Parse(ngaygiao);
                dh.TinhTrangGiaoHang = false;
                dh.TongTien = (decimal)TongTien();
                data.DONHANGs.InsertOnSubmit(dh);
                data.SubmitChanges();
                //Them chi tiet don hang
                foreach (var item in gh)
                {
                    CT_DONHANG ctdh = new CT_DONHANG();
                    ctdh.MaDonHang = dh.MaDonHang;
                    ctdh.MaGiay = item.iMaGiay;
                    ctdh.SoLuong = item.iSoLuong;
                    ctdh.GiaLucBan = (decimal)item.dGiaBan;
                    ctdh.ThanhTien = (decimal)item.dThanhTien;
                    data.CT_DONHANGs.InsertOnSubmit(ctdh);
                }
                data.SubmitChanges();
                ViewBag.MaDonHang = LayMaDonHang();
                return View(lstGioHang);

            }

            //Lya gio hang tu sesion


        }

        [HttpPost]
        public ActionResult DatHang(OrderViewModel model)
        {
            //them don hang
            //DONHANG dh = new DONHANG();
            //KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            //List<GioHang> gh = LayGioHang();
            //dh.MaKH = kh.MaKH;
            //dh.NgayDat = DateTime.Now;
            //var ngaygiao = String.Format("{0:MM/dd/yyyy}", DateTime.Now.AddDays(7));
            //dh.NgayGiao = DateTime.Parse(ngaygiao);
            //dh.TinhTrangGiaoHang = false;
            //dh.TongTien = (decimal)TongTien();
            //data.DONHANGs.InsertOnSubmit(dh);
            //data.SubmitChanges();
            ////Them chi tiet don hang
            //foreach (var item in gh)
            //{
            //    CT_DONHANG ctdh = new CT_DONHANG();
            //    ctdh.MaDonHang = dh.MaDonHang;
            //    ctdh.MaGiay = item.iMaGiay;
            //    ctdh.SoLuong = item.iSoLuong;
            //    ctdh.GiaLucBan = (decimal)item.dGiaBan;
            //    ctdh.ThanhTien = (decimal)item.dThanhTien;
            //    data.CT_DONHANGs.InsertOnSubmit(ctdh);
            //}
            //data.SubmitChanges();
            //Session["GioHang"] = null;
            model.TypePaymentVN = 2;
            var url = UrlPayment(model.TypePaymentVN, model.OrderCode);

            return Redirect(url);
        }

       




        public ActionResult Xacnhandonhang()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            ViewBag.ThanhToanThanhCong = TempData["ThanhToanThanhCong"];
            return View();
        }
        #region Thanh toán vnpay callback
        [HttpGet]
        public ActionResult PaymentCallBack()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["Vnpay:HashSecret"]; //Secret Key
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string orderCode = Convert.ToString(vnpay.GetResponseData("vnp_TxnRef"));
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];

                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        var itemOrder = data.DONHANGs.FirstOrDefault(x => x.MaDonHang == Convert.ToInt64(orderCode) && x.MaKH == kh.MaKH);

                        if (itemOrder != null)
                        {
                            itemOrder.TinhTrangGiaoHang = true;//đã thanh toán
                            data.SubmitChanges();
                        }
                        //Thanh toan thanh cong
                        TempData["SuccessMessage"] = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";
                    }
                    else
                    {


                        var itemOrder = data.DONHANGs.FirstOrDefault(x => x.MaDonHang == Convert.ToInt64(orderCode) && x.MaKH == kh.MaKH);
                        if (itemOrder != null)
                        {
                            var orderDetails = data.CT_DONHANGs.Where(x => x.MaDonHang == itemOrder.MaDonHang).ToList();
                            if (orderDetails.Count > 0)
                            {
                                foreach (var detail in orderDetails)
                                {
                                    data.CT_DONHANGs.DeleteOnSubmit(detail);
                                }
                            }
                            data.DONHANGs.DeleteOnSubmit(itemOrder);
                            data.SubmitChanges();

                        }
                        Session["GioHang"] = null;

                        TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: " + vnp_ResponseCode;
                    }

                    TempData["ThanhToanThanhCong"] = "Số tiền thanh toán (VND): " + vnp_Amount.ToString();

                }
            }

            return RedirectToAction("Xacnhandonhang", "GioHang");
        }
        #endregion  

        #region Thanh toán vnpay
        public string UrlPayment(int TypePaymentVN, int orderCode)
        {
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];

            var urlPayment = "";

            var order = data.DONHANGs.FirstOrDefault(x => x.MaDonHang == orderCode && x.MaKH == kh.MaKH);
            
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["PaymentCallBack:ReturnUrl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["Vnpay:BaseUrl"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["Vnpay:TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["Vnpay:HashSecret"]; //Secret Key

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            var Price = (long)order.TongTien * 100;
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Price.ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (TypePaymentVN == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (TypePaymentVN == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (TypePaymentVN == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", order.NgayDat?.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng :" + order.MaDonHang);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.MaDonHang.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            //log.InfoFormat("VNPAY URL: {0}", paymentUrl);
            return urlPayment;
        }
        #endregion    
    }
}