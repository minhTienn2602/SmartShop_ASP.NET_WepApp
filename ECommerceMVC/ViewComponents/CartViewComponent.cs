using ECommerceMVC.Data;
using ECommerceMVC.Helpers;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.ViewComponents
{
    public class CartViewComponent:ViewComponent
    {
        private readonly Hshop2023Context _context;
        public CartViewComponent(Hshop2023Context context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var cart=  HttpContext.Session.Get<List<CartItemVM>>(MySetting.CART_KEY) ?? new List<CartItemVM>();
            return View("CartNav",new CartNavVM
            {
                quantity= cart.Sum(p=>p.SoLuong),
                Total=cart.Sum(p=>p.ThanhTien)
            });
        }

    }
}
