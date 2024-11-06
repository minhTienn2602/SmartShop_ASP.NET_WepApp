using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using ECommerceMVC.Services;

namespace ECommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context _context;
        private readonly PaypalClient _paypalClient;

        public CartController(Hshop2023Context context, PaypalClient paypalClient)
        {
            _context = context;
            _paypalClient=paypalClient;
        }
        public IActionResult Index()
        {
            return View(Cart);
        }
        
        public List<CartItemVM> Cart => HttpContext.Session.Get<List<CartItemVM>>(MySetting.CART_KEY) ?? new List<CartItemVM>();
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            
            return RedirectToAction("Index");
        }
        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            //item null tức là mặt hàng này chưa có trong giỏ hàng
            if (item == null)
            {
                var hangHoa = _context.HangHoas.SingleOrDefault( p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItemVM
                {
                    MaHh = hangHoa.MaHh,
                    TenHh = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh=hangHoa.Hinh ?? string.Empty,
                    SoLuong=quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if(Cart.Count==0)
            {
                return Redirect("/");
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid) {
                var customerID = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                var hoaDon = new HoaDon
                {
                    MaKh = customerID,
                    HoTen=model.HoTen,
                    DiaChi = model.DiaChi,
                    DienThoai = model.DienThoai,
                    NgayDat=DateTime.Now,
                    CachThanhToan="COD",
                    CachVanChuyen="GRAB",
                    MaTrangThai=0,
                    GhiChu=model.GhiChu
                };
                _context.Database.BeginTransaction();
                try
                {
                    _context.Database.CommitTransaction();
                    _context.Add(hoaDon);
                    _context.SaveChanges();
                    var cthd = new List<ChiTietHd>();
                    foreach(var item in Cart)
                    {
                        cthd.Add(new ChiTietHd
                        {
                            MaHd=hoaDon.MaHd,
                            SoLuong=item.SoLuong,
                            DonGia=item.DonGia,
                            MaHh=item.MaHh,
                            GiamGia=0
                        });
                    }
                    _context.AddRange(cthd);
                    _context.SaveChanges();
                    HttpContext.Session.Set<List<CartItemVM>>(MySetting.CART_KEY, new List<CartItemVM>());
                    return View("Success");
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }
            }
            return View(Cart);
        }
        #region Paypal Payment
        //[Authorize]
        //[HttpPost]
        //public IActionResult CheckInfo(CheckoutVM model)
        //{
        //    if (ModelState.IsValid) {
        //        return RedirectToAction("CreatePaypalOrder");
        //    }
        //    return View(model);
        //}
        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            //Order's info send to Paypal 
            var tongTien=Cart.Sum(p=>p.ThanhTien).ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();
            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);
                return Ok(response);
            }
            catch(Exception ex) 
            {
                var error=new {ex.GetBaseException().Message};
                    return BadRequest(error);
            }

        }
        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder([FromBody] PaypalCheckoutVM model, CancellationToken cancelToken)
        {
            try
            {
                var resopnse = await _paypalClient.CaptureOrder(model.OrderID);
                if(resopnse.status == "COMPLETED")
                {
                    var customerID = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                    var hoaDon = new HoaDon
                    {
                        MaKh = customerID,
                        HoTen = model.HoTen,
                        DiaChi = model.DiaChi,
                        DienThoai = model.DienThoai,
                        NgayDat = DateTime.Now,
                        CachThanhToan = "PayPal",
                        CachVanChuyen = "GRAB",
                        MaTrangThai = 0,
                        GhiChu = model.GhiChu
                    };
                    _context.Database.BeginTransaction();
                    try
                    {
                        _context.Database.CommitTransaction();
                        _context.Add(hoaDon);
                        _context.SaveChanges();
                        var cthd = new List<ChiTietHd>();
                        foreach (var item in Cart)
                        {
                            cthd.Add(new ChiTietHd
                            {
                                MaHd = hoaDon.MaHd,
                                SoLuong = item.SoLuong,
                                DonGia = item.DonGia,
                                MaHh = item.MaHh,
                                GiamGia = 0
                            });
                        }
                        _context.AddRange(cthd);
                        _context.SaveChanges();
                        HttpContext.Session.Set<List<CartItemVM>>(MySetting.CART_KEY, new List<CartItemVM>());
                    }
                    catch
                    {
                        _context.Database.RollbackTransaction();
                    }
                    //Save order info in the database
                    return Ok(new { Message = "Đơn hàng đã được lưu thành công" });
                }
                // Trả về lỗi nếu thanh toán không thành công
                return BadRequest(new { Message = "Thanh toán PayPal không thành công" });
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }
        #endregion
        [Authorize]
        public IActionResult SuccessPayment()
        {
            return View("Success");
        }
        [Authorize]
        public IActionResult MyOrder()
        {
            string customerID = User.FindFirst(MySetting.CLAIM_CUSTOMERID)?.Value;
            var hoaDon = _context.HoaDons.Where(p => p.MaKh == customerID)
                                        .OrderByDescending(p => p.NgayDat)
                                        .Select(p => new HoaDonVM
                                        {
                                            MaHD = p.MaHd,
                                            NgayDat = p.NgayDat,
                                            NgayCan = p.NgayCan,
                                            NgayGiao = p.NgayGiao,
                                            HoTen = p.HoTen,
                                            DiaChi = p.DiaChi,
                                            CachThanhToan = p.CachThanhToan,
                                            CachVanChuyen = p.CachVanChuyen,
                                            MaTrangThai = p.MaTrangThai,
                                            GhiChu = p.GhiChu,
                                            DienThoai = p.DienThoai,
                                            TongSoLuong = _context.ChiTietHds
                .Where(ct => ct.MaHd == p.MaHd)
                .Sum(ct => ct.SoLuong), // Tính tổng số lượng
                                            TongGia = _context.ChiTietHds
                .Where(ct => ct.MaHd == p.MaHd)
                .Sum(ct => ct.DonGia * ct.SoLuong) // Tính tổng giá (có tính giảm giá)
                                        }).ToList();

            return View("MyOrder",hoaDon);
        }
    }
}
