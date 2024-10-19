namespace ECommerceMVC.ViewModels
{
    //Để hiện thông tin trên icon cart trên nav
    public class CartNavVM
    {
        //Tổng số lượng hàng hóa trong giỏ hàng
        public int quantity {  get; set; }
        //Tổng tiền của giỏ hàng
        public double Total {  get; set; }
    }
}
