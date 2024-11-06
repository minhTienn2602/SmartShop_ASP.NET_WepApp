using ECommerceMVC.Data;
using ECommerceMVC.Models;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Hshop2023Context _context;
        public HomeController(ILogger<HomeController> logger,Hshop2023Context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? loai)
        {
            // Lấy hàng hóa và join với ChiTietHD để tính tổng số lượng bán được
            var hangHoas = _context.HangHoas
                .Join(
                    _context.ChiTietHds,
                    h => h.MaHh,
                    c => c.MaHh,
                    (h, c) => new { HangHoa = h, SoLuong = c.SoLuong }
                )
                .GroupBy(
                    hc => new { hc.HangHoa.MaHh, hc.HangHoa.TenHh, hc.HangHoa.Hinh, hc.HangHoa.DonGia, hc.HangHoa.MoTaDonVi, hc.HangHoa.MaLoaiNavigation.TenLoai, hc.HangHoa.MaLoai }
                )
                .Select(g => new
                {
                    MaHH = g.Key.MaHh,
                    TenHH = g.Key.TenHh,
                    Hinh = g.Key.Hinh ?? "",
                    DonGia = g.Key.DonGia ?? 0,
                    MoTaNgan = g.Key.MoTaDonVi ?? "",
                    TenLoai = g.Key.TenLoai,
                    MaLoai = g.Key.MaLoai,
                    Total = g.Sum(x => x.SoLuong)  // Tính tổng số lượng bán được
                });

            // Nếu có loại thì chỉ lấy hàng hóa thuộc loại đó
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            // Lấy top 8 hàng hóa có tổng số lượng bán nhiều nhất
            var result = hangHoas
                .OrderByDescending(p => p.Total)
                .Take(8)
                .Select(p => new HangHoaVM
                {
                    MaHH = p.MaHH,
                    TenHH = p.TenHH,
                    Hinh = p.Hinh,
                    DonGia = p.DonGia,
                    MoTaNgan = p.MoTaNgan,
                    TenLoai = p.TenLoai
                }).ToList();

            return View(result);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}
