using AutoMapper;
using EduCore.Domain.Entities.CenterModel;
using EduCore.Shared.DTOs.Centers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.MappingProfiles
{
    public class CenterMappingProfile : Profile
    {
        public CenterMappingProfile() {

            CreateMap<Center, CenterDto>();

            CreateMap<CreateCenterDto, Center>()
              .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UpdateCenterDto, Center>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
