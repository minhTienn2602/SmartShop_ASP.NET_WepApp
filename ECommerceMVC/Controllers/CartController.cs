using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context _context;
        public CartController(Hshop2023Context context)
        {
            _context = context;
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
    }
}
