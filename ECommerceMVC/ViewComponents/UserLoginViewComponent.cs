using ECommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.ViewComponents
{
    public class UserLoginViewComponent:ViewComponent

    {
        private readonly Hshop2023Context _context;
        public UserLoginViewComponent(Hshop2023Context context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            bool isLoggedIn = User.Identity.IsAuthenticated;

            // Truyền giá trị isLoggedIn đến View
            return View(isLoggedIn);
        }
    }
}
