using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Shared.Enums
{
        public enum CourseLevel { Beginner, Intermediate, Advanced }
        public enum CoursePricingType { Free, Paid, Subscription }
        public enum CourseStatus { Draft, Published, Archived }
    public enum EnrollmentType { Purchase, Free, Gift }
    public enum EnrollmentStatus { Active, Expired, Cancelled , Completed }
    // SET type — a lesson can combine multiple content types
    // e.g. "video,pdf" or "live" or "video,live"
    [Flags]
    public enum LessonType
    {
        None = 0,
        Video = 1,
        Pdf = 2,
        Live = 4,
    }

}
