using AutoMapper;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.ContentModel;
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
                    .ForMember(d => d.CategoryName,   o => o.MapFrom(s => s.Category.Name))
                    .ForMember(d => d.TeacherName,    o => o.MapFrom(s => s.Teacher.Name))
                    .ForMember(d => d.TotalStudents,  o => o.MapFrom(s => s.Enrollments != null ? s.Enrollments.Count : 0))
                    .ForMember(d => d.TotalSections,  o => o.MapFrom(s => s.Sections   != null ? s.Sections.Count   : 0))
                    .ForMember(d => d.TotalLessons,   o => o.MapFrom(s => s.Sections   != null
                        ? s.Sections.Sum(sec => sec.Lessons != null ? sec.Lessons.Count : 0)
                        : 0));

                // Course → CourseDetailDto 
                CreateMap<Course, CourseDetailDto>()
                    .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                    .ForMember(d => d.TeacherName, o => o.MapFrom(s => s.Teacher.Name))
                    .ForMember(d => d.TeacherAvatar, o => o.MapFrom(s => s.Teacher.AvatarUrl));

                // Section → SectionDto
                CreateMap<Section, SectionDto>();

                // Lesson → LessonDto (includes nested VideoLesson & PdfLesson)
                CreateMap<Lesson, LessonDto>();
                CreateMap<VideoLesson, VideoLessonDto>();
                CreateMap<PdfLesson, PdfLessonDto>();

                // CreateCourseDto → Course
                CreateMap<CreateCourseDto, Course>();

                // UpdateCourseDto → Course
                CreateMap<UpdateCourseDto, Course>()
                    .ForAllMembers(o => o.Condition((src, dest, member) => member != null));
            }
        
    }
}
