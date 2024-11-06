namespace ECommerceMVC.ViewModels
{
    public class HoaDonVM
    {
        public int MaHD {  get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime? NgayCan {  get; set; }
        public DateTime? NgayGiao { get; set; }
        public string HoTen {  get; set; }
        public string DiaChi {  get; set; } 
        public string CachThanhToan {  get; set; }
        public string CachVanChuyen {  get; set; }
        public int MaTrangThai {  get; set; }
        public float PhiVanChuyen { get; set; }

        public string GhiChu {  get; set; }
        public string DienThoai {  get; set; }
        public int TongSoLuong { get; set; }
        public double TongGia { get; set; }

    }
}
