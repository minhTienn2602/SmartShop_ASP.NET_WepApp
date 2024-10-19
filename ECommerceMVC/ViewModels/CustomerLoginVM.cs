using Microsoft.CodeAnalysis.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace ECommerceMVC.ViewModels
{
    public class CustomerLoginVM
    {
        [Display(Name ="Tên đăng nhập")]
        [Required(ErrorMessage ="*")]
        [MaxLength(20,ErrorMessage ="Tối đa 20 kí tự")]
        public string Username {  get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
