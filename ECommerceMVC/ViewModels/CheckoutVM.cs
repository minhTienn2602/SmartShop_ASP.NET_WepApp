using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
    public class CheckoutVM
    {
        [Required]
        public string DiaChi {  get; set; }
        [Required]
        public string HoTen {  get; set; }
        [Required]
        public string DienThoai {  get; set; }  
        public string? GhiChu { get; set; }
    }
}
