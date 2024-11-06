using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
    public class ChangePasswordVM
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name ="Mật khẩu cũ")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Mật khẩu mới")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        [Display(Name = "Nhập lại")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
