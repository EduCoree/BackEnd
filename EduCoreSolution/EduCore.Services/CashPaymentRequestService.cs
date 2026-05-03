using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.EnrollmentModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.EnrollmentDTOs;
using EduCore.Shared.Enums;
using EduCore.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Services
{
    public class CashPaymentRequestService : ICashPaymentRequestService
    {
        private readonly IUnitOfWork _uow;
        private readonly IEnrollmentService _enrollmentService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public CashPaymentRequestService(
            IUnitOfWork uow,
            IEnrollmentService enrollmentService,
            INotificationService notificationService,
            IMapper mapper)
        {
            _uow = uow;
            _enrollmentService = enrollmentService;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<CashPaymentRequestDto> CreateRequestAsync(string studentId, int courseId)   // notification
        {
            var repo = _uow.GetRepository<CashPaymentRequest, int>();

            var alreadyExists = await repo.AnyAsync(r =>
                r.StudentId == studentId &&
                r.CourseId == courseId &&
                r.Status == CashRequestStatus.Pending);

            if (alreadyExists)
                throw new ConflictException("You already have a pending cash request for this course.");

            var course = await _uow.CourseRepository.GetByIdAsync(courseId)
                ?? throw new NotFoundException("Course not found.");

            var request = new CashPaymentRequest
            {
                StudentId = studentId,
                CourseId = courseId,
                Status = CashRequestStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await repo.AddAsync(request);
            await _uow.SaveChangesAsync();
            await _notificationService.SendNotificationToAdminsAsync(
                title: "New Cash Payment Request",
                message: $"A student requested cash payment for \"{course.Title}\"",
                notificationType: NotificationType.CashPaymentRequest,
                entityId: request.Id
            );

            return new CashPaymentRequestDto
            {
                Id = request.Id,
                StudentId = studentId,
                CourseId = courseId,
                CourseTitle = course.Title,
                Amount = course.Price,
                Status = request.Status.ToString(),
                RequestedAt = request.RequestedAt
            };
        }

        //public async Task<IEnumerable<CashPaymentRequestDto>> GetAllRequestsAsync()
        //{
        //    var requests = await _uow.GetRepository<CashPaymentRequest, int>().GetAllAsync();
        //    return _mapper.Map<IEnumerable<CashPaymentRequestDto>>(requests);
        //}
        public async Task<IEnumerable<CashPaymentRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _uow.GetRepository<CashPaymentRequest, int>()
                .GetAllAsQueryable()
                .Include(r => r.Student)
                .Include(r => r.Course)
                .AsNoTracking()
                .ToListAsync();

            return requests.Select(request => new CashPaymentRequestDto
            {
                Id = request.Id,
                StudentId = request.StudentId,
                StudentName = request.Student?.Name ?? "Unknown",
                CourseId = request.CourseId,
                CourseTitle = request.Course?.Title ?? "Unknown",
                Amount = request.Course?.Price ?? 0,
                Status = request.Status.ToString(),
                RequestedAt = request.RequestedAt
            }).ToList();
        }
        public async Task<CashPaymentRequestDto> ConfirmRequestAsync(int requestId)
        {
            var repo = _uow.GetRepository<CashPaymentRequest, int>();
            var request = await repo.GetByIdAsync(requestId)
                ?? throw new NotFoundException("Request not found.");

            if (request.Status != CashRequestStatus.Pending)
                throw new BadRequestException("Request is not pending.");

            var course = await _uow.CourseRepository.GetByIdAsync(request.CourseId)
                ?? throw new NotFoundException("Course not found.");

            await _enrollmentService.RecordCashPaymentAsync(new CashPaymentDto
            {
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                Amount = course.Price,
                Currency = "EGP"
            });

            request.Status = CashRequestStatus.Confirmed;
            repo.Update(request);
            await _uow.SaveChangesAsync();

            return new CashPaymentRequestDto
            {
                Id = request.Id,
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                CourseTitle = course.Title,
                Amount = course.Price,
                Status = request.Status.ToString(),
                RequestedAt = request.RequestedAt
            };
        }
        public async Task<CashPaymentRequestDto> RejectRequestAsync(int requestId)
        {
            var repo = _uow.GetRepository<CashPaymentRequest, int>();
            var request = await repo.GetByIdAsync(requestId)
                ?? throw new NotFoundException("Request not found.");

            if (request.Status != CashRequestStatus.Pending)
                throw new BadRequestException("Request is not pending.");

            var course = await _uow.CourseRepository.GetByIdAsync(request.CourseId)
                ?? throw new NotFoundException("Course not found.");

            request.Status = CashRequestStatus.Rejected;
            repo.Update(request);
            await _uow.SaveChangesAsync();

            return new CashPaymentRequestDto
            {
                Id = request.Id,
                StudentId = request.StudentId,
                CourseId = request.CourseId,
                CourseTitle = course.Title,
                Amount = course.Price,
                Status = request.Status.ToString(),
                RequestedAt = request.RequestedAt
            };
        }
    }
}
