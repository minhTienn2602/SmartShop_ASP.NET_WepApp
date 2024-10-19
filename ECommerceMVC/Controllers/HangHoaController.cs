using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context _context;
        public HangHoaController(Hshop2023Context context) {
            _context = context;
        }
        public IActionResult Index(int? loai)
        {
            var hangHoas=_context.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hangHoas=hangHoas.Where(p=>p.MaLoai==loai.Value);
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHH = p.MaHh,
                TenHH = p.TenHh,
                Hinh = p.Hinh ?? "",
                DonGia = p.DonGia ?? 0,
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai

            });
            return View(result);
        }
        public IActionResult Search(string? query)
        {
            var hangHoas =_context.HangHoas.AsQueryable();
            if(query != null)
            {
                hangHoas=hangHoas.Where(p=>p.TenHh.Contains(query));
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHH=p.MaHh,
                TenHH=p.TenHh,
                DonGia=p.DonGia ?? 0,
                Hinh=p.Hinh ?? "",
                MoTaNgan=p.MoTaDonVi ?? "",
                TenLoai=p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }
        public IActionResult Detail(int id)
        {
            var data = _context.HangHoas
                .Include(p=>p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);
            if(data == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }
            var result = new ChiTietHangHoaVM
            {
                MaHH=data.MaHh,
                TenHH=data.TenHh,
                DonGia=data.DonGia ?? 0,
                ChiTiet=data.MoTa ?? "",
                Hinh=data.Hinh ?? "",
                DiemDanhGia=5,//check sau
                MoTaNgan=data.MoTaDonVi ?? string.Empty,
                TenLoai=data.MaLoaiNavigation.TenLoai,
                SoLuongTon=10
            };
            return View(result);
        }
    }
}
