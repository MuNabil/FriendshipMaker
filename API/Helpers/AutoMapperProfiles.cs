namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<ApplicationUser, MemberDto>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

        CreateMap<Photo, PhotoDto>();

        CreateMap<UpdateMemberDto, ApplicationUser>();

        CreateMap<RegisterDto, ApplicationUser>()
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.ToLower()));

        CreateMap<Message, MessageDto>()
          .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
          .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));

        // For Utc DateTime standard
        CreateMap<DateTime, DateTime>().ConvertUsing(date => DateTime.SpecifyKind(date, DateTimeKind.Utc));

    }
}