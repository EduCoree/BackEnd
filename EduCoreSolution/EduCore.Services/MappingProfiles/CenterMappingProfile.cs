using AutoMapper;
using EduCore.Domain.Entities.CenterModel;
using EduCore.Shared.DTOs.Centers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EduCore.Services.MappingProfiles
{
    public class CenterMappingProfile : Profile
    {
        public CenterMappingProfile()
        {
            CreateMap<Center, CenterDto>()
                .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom<CenterLogoUrlResolver>())
                .ForMember(dest => dest.SocialLinks, opt => opt.MapFrom(src => DeserializeSocialLinks(src.SocialLinks)));

            CreateMap<CreateCenterDto, Center>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.SocialLinks, opt => opt.MapFrom(src => SerializeSocialLinks(src.SocialLinks))); 

            CreateMap<UpdateCenterDto, Center>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.SocialLinks, opt => opt.MapFrom(src => SerializeSocialLinks(src.SocialLinks))); 
        }

       
        private static string? SerializeSocialLinks(SocialLinksDto? dto)
        {
            if (dto is null) return null;
            return JsonSerializer.Serialize(dto);
        }

      
        private static SocialLinksDto? DeserializeSocialLinks(string? json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                return JsonSerializer.Deserialize<SocialLinksDto>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}