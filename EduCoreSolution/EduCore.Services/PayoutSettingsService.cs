using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.PayoutDTOs;
using EduCore.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class PayoutSettingsService : IPayoutSettingsService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PayoutSettingsService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PayoutSettingsDto> GetSettingsAsync()
        {
            var settings = await _uow.PayoutSettingsRepository.GetSettingsAsync();
            return _mapper.Map<PayoutSettingsDto>(settings);
        }

        public async Task<PayoutSettingsDto> UpdateSettingsAsync(UpdatePayoutSettingsDto dto, string adminId)
        {
            // Cross-field validation: tiers must be in increasing order of thresholds
            if (dto.Tier2Threshold <= dto.Tier1Threshold)
                throw new BadRequestException("Tier2Threshold must be greater than Tier1Threshold.");

            if (dto.Tier3Threshold <= dto.Tier2Threshold)
                throw new BadRequestException("Tier3Threshold must be greater than Tier2Threshold.");

            // Bonuses should also be monotonically increasing (motivational)
            if (dto.Tier2Bonus < dto.Tier1Bonus)
                throw new BadRequestException("Tier2Bonus should be >= Tier1Bonus.");

            if (dto.Tier3Bonus < dto.Tier2Bonus)
                throw new BadRequestException("Tier3Bonus should be >= Tier2Bonus.");

            // Map DTO → entity (repo will apply in-place to the existing Id=1 row)
            var incoming = _mapper.Map<Domain.Entities.PayoutModel.PayoutSettings>(dto);

            await _uow.PayoutSettingsRepository.UpdateSettingsAsync(incoming, adminId);
            await _uow.SaveChangesAsync();

            // Return the updated (persisted) settings
            var updated = await _uow.PayoutSettingsRepository.GetSettingsAsync();
            return _mapper.Map<PayoutSettingsDto>(updated);
        }
    }
}
