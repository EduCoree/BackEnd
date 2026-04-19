using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.DTOs.Quiz.Student
{
    public class AvailableQuizzesDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public int PassScore { get; init; }
        public string CourseTitle { get; init; } = string.Empty;
    }
}
