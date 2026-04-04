using AutoMapper;
using EduCore.Domain.Entities.ProgressModel;
using EduCore.Shared.DTOs.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.MappingProfiles
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<CourseReview, ReviewDto>();
        }
    }
}
