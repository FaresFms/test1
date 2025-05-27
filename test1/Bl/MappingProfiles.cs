using AutoMapper;
using rent.Models;
using test1.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // تعيين من ApplicationUser إلى UserProfileDto
        CreateMap<ApplicationUser, UserModel>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.city))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FullName));
        // بدلاً من إنشاء DTO جديد للسيارات المميزة
        CreateMap<TbCar, PageCarModel>();
      
        // يمكنك إضافة تعيينات أخرى هنا

    }
}