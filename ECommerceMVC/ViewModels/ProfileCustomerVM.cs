using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
    public class ProfileCustomerVM
    {
        //Apply Validation
        [Display(Name = "Tên đăng nhập")]
        public string MaKh { get; set; }

        


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

        [RegularExpression(@"0[1-9]\d{8}", ErrorMessage = "Chua dung dinh dang dien thoai VN")]
        public string DienThoai { get; set; }

        [Display(Name = "Email")]

        [EmailAddress(ErrorMessage = "Chua dung dinh dang")]
        public string Email { get; set; }

        [Display(Name = "Ảnh đại diện ")]
        public string? Hinh { get; set; }
    }
}
