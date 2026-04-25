using AutoMapper;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Shared.DTOs.EnrollmentDTOs;

namespace EduCore.Services.MappingProfiles
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            // Enrollment → EnrollmentDto
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(d => d.CourseTitle,
                    o => o.MapFrom(s => s.Course.Title))
                .ForMember(d => d.CourseCover,
                    o => o.MapFrom(s => s.Course.CoverImage));

            // Payment → PaymentDto
            CreateMap<Payment, PaymentDto>()
                .ForMember(d => d.CourseTitle,
                    o => o.MapFrom(s => s.Enrollment.Course.Title));
            // CashPaymentRequest → CashPaymentRequestDto
            CreateMap<CashPaymentRequest, CashPaymentRequestDto>()
                .ForMember(d => d.StudentName,
                    o => o.MapFrom(s => s.Student.Name))
                .ForMember(d => d.CourseTitle,
                    o => o.MapFrom(s => s.Course.Title))
                .ForMember(d => d.Amount,
                    o => o.MapFrom(s => s.Course.Price))
                .ForMember(d => d.Status,
                    o => o.MapFrom(s => s.Status.ToString()));
        }
    }
}
