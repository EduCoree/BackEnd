using AutoMapper;
using EduCore.Domain.Entities.PayoutModel;
using EduCore.Shared.DTOs.PayoutDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services.MappingProfiles
{
    public class PayoutMappingProfile : Profile
    {
        public PayoutMappingProfile()
        {
            // TeacherEarning → TeacherEarningDto
            CreateMap<TeacherEarning, TeacherEarningDto>()
                .ForMember(d => d.CourseTitle,
                    o => o.MapFrom(s => s.Course != null ? s.Course.Title : string.Empty))
                .ForMember(d => d.InvoiceNumber,
                    o => o.MapFrom(s => s.Invoice != null ? s.Invoice.InvoiceNumber : null));

            // TeacherInvoice → TeacherInvoiceDto (list view)
            CreateMap<TeacherInvoice, TeacherInvoiceDto>()
                .ForMember(d => d.TeacherName,
                    o => o.MapFrom(s => s.Teacher != null ? s.Teacher.Name : null));

            // TeacherInvoice → TeacherInvoiceDetailDto (detail view with earnings)
            CreateMap<TeacherInvoice, TeacherInvoiceDetailDto>()
                .ForMember(d => d.TeacherName,
                    o => o.MapFrom(s => s.Teacher != null ? s.Teacher.Name : null))
                .ForMember(d => d.Earnings,
                    o => o.MapFrom(s => s.Earnings));

            // PayoutSettings → PayoutSettingsDto (read)
            CreateMap<PayoutSettings, PayoutSettingsDto>();

            // UpdatePayoutSettingsDto → PayoutSettings (write)
            // Only the editable fields — Id, UpdatedAt, UpdatedBy are set by service
            CreateMap<UpdatePayoutSettingsDto, PayoutSettings>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore());
        }
    }
}
