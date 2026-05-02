using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.EnrollmentDTOs;
using EduCore.Shared.DTOs.Notifications;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly PaymobService _paymobService;
        private readonly INotificationService _notificationService;
        


        // 👇 Earnings service — still needed, but no more DbContext dependency
        private readonly ITeacherEarningService _teacherEarningService;

        public EnrollmentService(IUnitOfWork uow, IMapper mapper, PaymobService paymobService,
             ITeacherEarningService teacherEarningService,INotificationService notificationService) 
        {
            _uow = uow;
            _mapper = mapper;
            _paymobService = paymobService;
           _notificationService = notificationService;
           
            _teacherEarningService = teacherEarningService;
        }

        public async Task<EnrollmentDto> EnrollFreeAsync(string studentId, int courseId)
        {
            //  get course and confirm it free ,publish,avilable
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.PricingType != CoursePricingType.Free)
                throw new BadRequestException("Course Not Free");

            if (course.Status != CourseStatus.Published)
                throw new BadRequestException("Course Not Avilable");

            // ensure with student not enroll in course
            if (await _uow.EnrollmentRepository.IsEnrolledAsync(studentId, courseId))
                throw new BadRequestException("You are already enrolled in this course");

            // make enroll to student
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                Type = EnrollmentType.Free,
                Status = EnrollmentStatus.Active,
                EnrolledAt = DateTime.UtcNow
            };

            await _uow.EnrollmentRepository.AddAsync(enrollment);
            await _uow.SaveChangesAsync();
            await _notificationService.SendNotificationAsync(studentId, "Enrolled Successfully", $"You have been enrolled in {course.Title}",NotificationType.Enrollment, courseId);
            var enrollmentWithCourse = await _uow.EnrollmentRepository
         .GetByIdAsync(enrollment.Id);

            return _mapper.Map<EnrollmentDto>(enrollmentWithCourse);
        }

        public async Task<CheckoutResponseDto> CreateCheckoutAsync(string studentId, int courseId)
        {
            // get course and ensure avilable,publish and free
            var course = await _uow.CourseRepository.GetByIdAsync(courseId);

            if (course is null)
                throw new NotFoundException("Course Not Found");

            if (course.PricingType == CoursePricingType.Free)
                throw new BadRequestException("The course is free — use Free Enrollment");

            if (course.Status != CourseStatus.Published)
                throw new BadRequestException("Course Not Avilable");

            //  ensuure with student not enroll in course
            if (await _uow.EnrollmentRepository.IsEnrolledAsync(studentId, courseId))
                throw new BadRequestException("You are already enrolled in this course");

            // make paument status = pending
            var payment = new Payment
            {
                EnrollmentId = null,        
                StudentId = studentId,
                Amount = course.DiscountedPrice ?? course.Price,
                Currency = "EGP",
                Method = PaymentMethod.Paymob,
                Status = PaymentStatus.Pending,
                Reference = $"course_{courseId}_{Guid.NewGuid()}"
            };

            await _uow.PaymentRepository.AddAsync(payment);
            await _uow.SaveChangesAsync();

            // checkout for paymob
            var studentEmail = await GetStudentEmailAsync(studentId);
            var checkoutUrl = await _paymobService.CreateCheckoutAsync(
                payment.Id,
                payment.Amount,
                payment.Currency,
                studentEmail
            );

            return new CheckoutResponseDto
            {
                PaymentId = payment.Id,
                CheckoutUrl = checkoutUrl
            };
        }

        private async Task<string> GetStudentEmailAsync(string studentId)
        {
            return "student@educore.com";
        }

        //public async Task<EnrollmentDto> RecordCashPaymentAsync(CashPaymentDto dto)
        //{
        //    // ensure course avilable 
        //    var course = await _uow.CourseRepository.GetByIdAsync(dto.CourseId);

        //    if (course is null)
        //        throw new NotFoundException("course not found");

        //    // ensure student not enroll in course 
        //    if (await _uow.EnrollmentRepository.IsEnrolledAsync(dto.StudentId, dto.CourseId))
        //        throw new BadRequestException("The student is already enrolled in this course.");

        //    // make payment + enrollment
        //    var enrollment = new Enrollment
        //    {
        //        StudentId = dto.StudentId,
        //        CourseId = dto.CourseId,
        //        Type = EnrollmentType.Purchase,
        //        Status = EnrollmentStatus.Active,
        //        EnrolledAt = DateTime.UtcNow
        //    };

        //    await _uow.EnrollmentRepository.AddAsync(enrollment);
        //    await _uow.SaveChangesAsync();

        //    // payment recored
        //    var payment = new Payment
        //    {
        //        EnrollmentId = enrollment.Id,
        //        StudentId = dto.StudentId,
        //        Amount = dto.Amount,
        //        Currency = dto.Currency,
        //        Method = PaymentMethod.CreditCard,
        //        Status = PaymentStatus.Completed, 
        //        PaidAt = DateTime.UtcNow
        //    };

        //    await _uow.PaymentRepository.AddAsync(payment);
        //    await _uow.SaveChangesAsync();

        //    return _mapper.Map<EnrollmentDto>(enrollment);
        //}

        //public async Task HandlePaymobWebhookAsync(PaymobWebhookDto webhook)
        //{

        //    var paymentIdStr = webhook.obj?.order?.merchant_order_id;
        //    if (!int.TryParse(paymentIdStr, out int paymentId))
        //    {
        //        throw new BadRequestException("Invalid payment ID");
        //    }
        //    var payment = await _uow.PaymentRepository.GetByIdAsync(paymentId);

        //    if (payment is null)
        //    {
        //        throw new NotFoundException("Payment not found");
        //    }

        //    if (payment.Status == PaymentStatus.Completed)
        //    {
        //        return;
        //    }
        //    var course = await GetCourseFromPayment(payment);

        //    if (course == null)
        //    {
        //        throw new NotFoundException("Course not found");
        //    }

        //    // Create enrollment
        //    var enrollment = new Enrollment
        //    {
        //        StudentId = payment.StudentId,
        //        CourseId = course.Id,
        //        Type = EnrollmentType.Purchase,
        //        Status = EnrollmentStatus.Active,
        //        EnrolledAt = DateTime.UtcNow
        //    };

        //    await _uow.EnrollmentRepository.AddAsync(enrollment);
        //    await _uow.SaveChangesAsync();

        //    // Update payment
        //    payment.EnrollmentId = enrollment.Id;
        //    payment.Status = PaymentStatus.Completed;
        //    payment.PaidAt = DateTime.UtcNow;

        //    _uow.PaymentRepository.Update(payment);
        //    await _uow.SaveChangesAsync();
        //}
        // ══════════════════════════════════════════════════════════════════
        //  🔧 MODIFIED — Cash payment now creates earning in a transaction
        //                Transaction is managed through IUnitOfWork (not DbContext)
        // ══════════════════════════════════════════════════════════════════
        public async Task<EnrollmentDto> RecordCashPaymentAsync(CashPaymentDto dto)
        {
            // ensure course avilable 
            var course = await _uow.CourseRepository.GetByIdAsync(dto.CourseId);

            if (course is null)
                throw new NotFoundException("course not found");

            // ensure student not enroll in course 
            if (await _uow.EnrollmentRepository.IsEnrolledAsync(dto.StudentId, dto.CourseId))
                throw new BadRequestException("The student is already enrolled in this course.");

            // 👇 Wrap enrollment + payment + earning in one transaction via IUnitOfWork
            await _uow.BeginTransactionAsync();

            try
            {
                // make payment + enrollment
                var enrollment = new Enrollment
                {
                    StudentId = dto.StudentId,
                    CourseId = dto.CourseId,
                    Type = EnrollmentType.Purchase,
                    Status = EnrollmentStatus.Active,
                    EnrolledAt = DateTime.UtcNow
                };

                await _uow.EnrollmentRepository.AddAsync(enrollment);
                await _uow.SaveChangesAsync();

                // payment recored
                var payment = new Payment
                {
                    EnrollmentId = enrollment.Id,
                    StudentId = dto.StudentId,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    Method = PaymentMethod.CreditCard,
                    Status = PaymentStatus.Completed,
                    PaidAt = DateTime.UtcNow
                };

           
                await _uow.PaymentRepository.AddAsync(payment);
                await _uow.SaveChangesAsync();
                await _notificationService.SendNotificationAsync(dto.StudentId, "Payment Confirmed", $"Your Cash Payment Has Been Recorded", NotificationType.Enrollment, dto.CourseId);

              
                await _teacherEarningService.CreateEarningForPaymentAsync(payment, enrollment);
                await _uow.SaveChangesAsync();

                await _uow.CommitTransactionAsync();

                return _mapper.Map<EnrollmentDto>(enrollment);
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw; // re-throw so existing exception middleware handles it
            }
        }

        // ══════════════════════════════════════════════════════════════════
        //  🔧 MODIFIED — Paymob webhook now creates earning in a transaction
        //                Transaction is managed through IUnitOfWork (not DbContext)
        // ══════════════════════════════════════════════════════════════════
        public async Task HandlePaymobWebhookAsync(PaymobWebhookDto webhook)
        {
            var paymentIdStr = webhook.obj?.order?.merchant_order_id;
            if (paymentIdStr?.Contains('_') == true)
                paymentIdStr = paymentIdStr.Split('_')[0];
            if (!int.TryParse(paymentIdStr, out int paymentId))
            {
                throw new BadRequestException($"Invalid payment ID: '{paymentIdStr}'");
            }
            var payment = await _uow.PaymentRepository.GetByIdAsync(paymentId);

            if (payment is null)
            {
                throw new NotFoundException("Payment not found");
            }

            if (payment.Status == PaymentStatus.Completed)
            {
                return;
            }
            var course = await GetCourseFromPayment(payment);

            if (course == null)
            {
                throw new NotFoundException("Course not found");
            }

            // 👇 Wrap enrollment + payment update + earning in one transaction
            await _uow.BeginTransactionAsync();

            try
            {
                // Create enrollment
                var enrollment = new Enrollment
                {
                    StudentId = payment.StudentId,
                    CourseId = course.Id,
                    Type = EnrollmentType.Purchase,
                    Status = EnrollmentStatus.Active,
                    EnrolledAt = DateTime.UtcNow
                };

                await _uow.EnrollmentRepository.AddAsync(enrollment);
                await _uow.SaveChangesAsync();

                // Update payment
                payment.EnrollmentId = enrollment.Id;
                payment.Status = PaymentStatus.Completed;
                payment.PaidAt = DateTime.UtcNow;

                _uow.PaymentRepository.Update(payment);
                await _uow.SaveChangesAsync();
                 await _notificationService.SendNotificationAsync(payment.StudentId, "Payment Successful", $"You have been enrolled in {course.Title}", NotificationType.Enrollment, course.Id);
            await _notificationService.SendNotificationAsync(course.TeacherId,"New Enrollment", $"A student enrolled in {course.Title}",NotificationType.Enrollment, course.Id);

                // 👇 Create teacher earning (80% / 20% split)
                await _teacherEarningService.CreateEarningForPaymentAsync(payment, enrollment);
                await _uow.SaveChangesAsync();

                await _uow.CommitTransactionAsync();
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }
        private async Task<Course> GetCourseFromPayment(Payment payment)
        {
            if (payment.Reference?.Contains("course_") == true)
            {
                var parts = payment.Reference.Split('_');
                if (parts.Length > 1 && int.TryParse(parts[1], out int courseId))
                {
                    return await _uow.CourseRepository.GetByIdAsync(courseId);
                }
            }

            throw new BadRequestException("CourseId not found in Payment");
        }

    }
}
