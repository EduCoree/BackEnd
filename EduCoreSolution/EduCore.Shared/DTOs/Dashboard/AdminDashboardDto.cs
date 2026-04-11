using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Dashboard
{
    public record AdminDashboardDto(
    int TotalStudents,
    int TotalTeachers,
    int TotalCourses,
    int ActiveCourses,
    int TotalEnrollments,
    decimal TotalRevenue,
    int NewEnrollmentsToday,
    int CertificatesIssued
    );

    public record TrendPointDto(
        DateOnly Date,
        decimal Value
    );
    public record TopCourseDto(
    int CourseId,
    string Title,
    string? CoverImage,
    string TeacherName,
    int EnrollmentCount
);

}
