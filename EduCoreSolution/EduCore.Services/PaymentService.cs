using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Services_Abstraction;
using EduCore.Shared.Common;
using EduCore.Shared.DTOs.EnrollmentDTOs;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentDto>> GetMyPaymentsAsync(string studentId)
        {
            var payments = await _uow.PaymentRepository.GetStudentPaymentsAsync(studentId);
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PagedResult<PaymentDto>> GetAllPaymentsAsync(PaginationParams pagination)
        {
            var allPayments = _uow.PaymentRepository.GetAllWithDetailsAsQueryable();
            var total = await allPayments.CountAsync();

            var pagedPayments = await allPayments
                .OrderByDescending(p => p.PaidAt ?? DateTime.UtcNow)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(pagedPayments);

            return new PagedResult<PaymentDto>
            {
                Items = paymentDtos,
                TotalCount = total,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
        }
    }
}
