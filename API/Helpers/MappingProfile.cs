using AuthAPI.DTOs;
using AuthAPI.Models;
using AutoMapper;

namespace AuthAPI.Helpers;

/// <summary>
/// AutoMapper profile for entity-to-DTO conversions.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => Roles.User))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
