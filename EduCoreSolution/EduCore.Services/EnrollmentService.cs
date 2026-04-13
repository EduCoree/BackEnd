using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.CourseModel;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.EnrollmentDTOs;
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

        public EnrollmentService(IUnitOfWork uow, IMapper mapper, PaymobService paymobService)
        {
            _uow = uow;
            _mapper = mapper;
            _paymobService = paymobService;
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

        public async Task<EnrollmentDto> RecordCashPaymentAsync(CashPaymentDto dto)
        {
            // ensure course avilable 
            var course = await _uow.CourseRepository.GetByIdAsync(dto.CourseId);

            if (course is null)
                throw new NotFoundException("course not found");

            // ensure student not enroll in course 
            if (await _uow.EnrollmentRepository.IsEnrolledAsync(dto.StudentId, dto.CourseId))
                throw new BadRequestException("The student is already enrolled in this course.");

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

            return _mapper.Map<EnrollmentDto>(enrollment);
        }

        public async Task HandlePaymobWebhookAsync(PaymobWebhookDto webhook)
        {
            var paymentIdStr = webhook.obj?.order?.merchant_order_id;

            if (!int.TryParse(paymentIdStr, out int paymentId))
                throw new BadRequestException("Payment ID does not exist");

            var payment = await _uow.PaymentRepository.GetByIdAsync(paymentId);

            if (payment is null)
                throw new NotFoundException("Payment not found");

            if (payment.Status == PaymentStatus.Completed)
                return; 

            // get couurse from db or ref
            var course = await GetCourseFromPayment(payment);

            // make Enrollment
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

            // payment with enrollment
            payment.EnrollmentId = enrollment.Id; 
            payment.Status = PaymentStatus.Completed;
            payment.PaidAt = DateTime.UtcNow;

            _uow.PaymentRepository.Update(payment);
            await _uow.SaveChangesAsync();
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
