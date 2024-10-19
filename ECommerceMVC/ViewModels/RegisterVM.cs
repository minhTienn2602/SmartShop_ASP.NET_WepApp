using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
    
    public class RegisterVM
    {
      
        //Apply Validation
        [Display(Name ="Tên đăng nhập")]
        [Required(ErrorMessage ="*")]
        [MaxLength(20,ErrorMessage ="Tối đa 20 kí tự")]
        public string MaKh { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage ="*")]
        [MaxLength(50,ErrorMessage = "Tối đa 50 kí tự")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }


        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)] 
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "   Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 kí tự")]

        public string DiaChi { get; set; }

        [Display(Name = "Điện thoại")]

        [RegularExpression(@"0[1-9]\d{8}", ErrorMessage ="Chua dung dinh dang dien thoai VN")]
        public string DienThoai { get; set; }

        [Display(Name = "Email")]

        [EmailAddress(ErrorMessage = "Chua dung dinh dang")]
        public string Email { get; set; }

        [Display(Name = "Ảnh đại diện ")]
        public string? Hinh { get; set; }

        
    }
}
