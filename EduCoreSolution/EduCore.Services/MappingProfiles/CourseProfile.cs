using AutoMapper;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Shared.DTOs.CourseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.MappingProfiles
{
    public class CourseProfile : Profile
    {
            public CourseProfile()
            {
                // Course → CourseSummaryDto (للقوايم)
                CreateMap<Course, CourseSummaryDto>()
                    .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                    .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher.Name));

                // Course → CourseDetailDto 
                CreateMap<Course, CourseDetailDto>()
                    .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                    .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher.Name))
                    .ForMember(d => d.TeacherAvatar, o => o.MapFrom(s => s.Teacher.AvatarUrl));

                // Section → SectionDto
                CreateMap<Section, SectionDto>();

                // Lesson → LessonDto
                CreateMap<Lesson, LessonDto>();

                // CreateCourseDto → Course
                CreateMap<CreateCourseDto, Course>();

                // UpdateCourseDto → Course
                CreateMap<UpdateCourseDto, Course>()
                    .ForAllMembers(o => o.Condition((src, dest, member) => member != null));
            }
        
    }
}
