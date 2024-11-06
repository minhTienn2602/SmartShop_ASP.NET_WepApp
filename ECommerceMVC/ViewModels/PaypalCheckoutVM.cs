namespace ECommerceMVC.ViewModels
{
    public class PaypalCheckoutVM
    {
        public string OrderID { get; set; }  // ID của đơn hàng PayPal
        public string HoTen { get; set; }    // Họ tên người mua
        public string DiaChi { get; set; }   // Địa chỉ giao hàng
        public string DienThoai { get; set; } // Số điện thoại người mua
        public string GhiChu {  get; set; } 
    }
}
