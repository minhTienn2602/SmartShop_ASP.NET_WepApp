﻿using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
    public class CheckoutVM
    {
       
        public string DiaChi {  get; set; }
        
        public string HoTen {  get; set; }
       
        public string DienThoai {  get; set; }  
        public string? GhiChu { get; set; }
    }
}
