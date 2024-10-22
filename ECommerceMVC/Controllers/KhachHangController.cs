using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Security.Claims;

namespace ECommerceMVC.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context _context;
        private readonly IMapper _mapper;

        public KhachHangController(Hshop2023Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();

        }
        [HttpPost]
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh)
        {

            model.Hinh = "temp";
            bool temp = ModelState.IsValid;
            if (temp){
                // Giả sử bạn có phương thức kiểm tra tên đăng nhập trong lớp UserRepository hoặc UserService
                bool isUsernameExists = _context.KhachHangs.Any(k => k.MaKh == model.MaKh);

                if (isUsernameExists)
                {
                    // Nếu tên đăng nhập đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("MaKh", "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.");
                    return View(model);  // Trả về view với dữ liệu hiện tại
                }
                else
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRandomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true;
                    khachHang.VaiTro = 0;
                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UpLoadHinh(Hinh, "KhachHang");
                    }
                    _context.Add(khachHang);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");

                }
            }

            return View(model);

        }
        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(CustomerLoginVM model,string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid) {
                var khachHang = _context.KhachHangs.SingleOrDefault(p => p.MaKh == model.Username);
                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "Thông tin đăng nhập không đúng!");
                }
                else
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản của quý khách đã bị khóa");

                    }
                    else
                    {
                        if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            ModelState.AddModelError("loi", "Thông tin đăng nhập không đúng!");
                        }
                        else
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email,khachHang.Email),
                                new Claim(ClaimTypes.Name,khachHang.HoTen),
                                new Claim("CustomerID",khachHang.MaKh),
                                new Claim(ClaimTypes.Role,"Customer")
                            };
                            var claimsIdentity=new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimsPrincipal);
                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
            }
            return View();
        }

        #endregion Login

       
        [Authorize]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            string MaKh = User.FindFirst("CustomerID")?.Value;
            var khachHang=_context.KhachHangs.SingleOrDefault(p=>p.MaKh==MaKh);
            if (khachHang != null)
            {
                var model = new ProfileCustomerVM
                {
                    MaKh = khachHang.MaKh,
                    HoTen = khachHang.HoTen,
                    NgaySinh = khachHang.NgaySinh,
                    DienThoai = khachHang.DienThoai,
                    Email = khachHang.Email,
                    DiaChi = khachHang.DiaChi,
                    GioiTinh = khachHang.GioiTinh? "Nam":"Nữ",
                    Hinh = khachHang.Hinh // Nếu cần thiết để hiển thị đường dẫn hình ảnh
                };
                return View(model);
            }

            return View(new RegisterVM());
        }
        [Authorize]
        [HttpPost]
        public IActionResult Profile(ProfileCustomerVM model, IFormFile Hinh)
        {
            model.Hinh = "temp";

            // Kiểm tra tính hợp lệ của dữ liệu nhập vào từ form
            if (ModelState.IsValid)
            {
                // Tìm kiếm khách hàng dựa trên MaKh
                var khachHang = _context.KhachHangs.FirstOrDefault(k => k.MaKh == model.MaKh);
                if (khachHang != null)
                {
                    // Cập nhật thông tin khách hàng
                    khachHang.DienThoai = model.DienThoai;
                    khachHang.Email = model.Email;
                    khachHang.NgaySinh = model.NgaySinh ?? khachHang.NgaySinh;
                    khachHang.GioiTinh = model.GioiTinh == "Nam" ? true : false; // Chuyển đổi giới tính sang kiểu bool
                    khachHang.DiaChi = model.DiaChi;

                    // Kiểm tra xem người dùng có tải lên hình ảnh mới không
                    if (Hinh != null)
                    {
                        // Gọi phương thức upload hình và lưu tên file vào cơ sở dữ liệu
                        khachHang.Hinh = MyUtil.UpLoadHinh(Hinh, "KhachHang");
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.Update(khachHang);
                    _context.SaveChanges();

                    // Chuyển hướng người dùng về trang thông tin cá nhân sau khi cập nhật thành công
                    return RedirectToAction("Profile");
                }
                else
                {
                    // Trường hợp không tìm thấy khách hàng
                    ModelState.AddModelError("", "Không tìm thấy thông tin khách hàng.");
                }
            }

            // Trả về view với dữ liệu hiện tại nếu có lỗi xảy ra
            return View(model);
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var khachHang = _context.KhachHangs.SingleOrDefault(p => p.MaKh == User.FindFirst("CustomerID").Value);
                string oldPass = model.OldPassword.ToMd5Hash(khachHang.RandomKey);
                if (oldPass != khachHang.MatKhau)
                {
                    ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng!");
                }
                else
                {
                    if(model.NewPassword!=model.ConfirmPassword)
                    {
                        ModelState.AddModelError("ConfirmPassword", "Mật khẩu nhập lại không đúng!");
                    }
                    else
                    {
                        khachHang.MatKhau = model.NewPassword.ToMd5Hash(khachHang.RandomKey);
                        _context.Update(khachHang);
                        _context.SaveChanges();
                    }
                }
            }

            return View(model);
        }

    }
}
