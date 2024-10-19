using AutoMapper;
using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;

namespace ECommerceMVC.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Define the mapping between RegisterVM and KhachHang
            CreateMap<RegisterVM, KhachHang>()
                .ForMember(dest => dest.GioiTinh,
                    opt => opt.MapFrom(src => src.GioiTinh == "Nam" ? true : false)); // Custom mapping for GioiTinh
        }
    }
}
