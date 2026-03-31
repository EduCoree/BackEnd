using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.CenterModel;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Centers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class CenterService : ICenterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CenterService(IUnitOfWork unitOfWork ,IMapper mapper )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<CenterDto> CreateCenterAsync(CreateCenterDto dto)
        {
            var repo = _unitOfWork.GetRepository<Center, int>();
            var center = _mapper.Map<Center>(dto);
            await repo.AddAsync(center);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CenterDto>(center);
        }

        public async Task<bool> DeleteCenterAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Center, int>();
            var center = await repo.GetByIdAsync(id);
            if (center is null) return false;

            repo.Remove(center);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CenterDto>> GetAllCentersAsync()
        {
            var repo = _unitOfWork.GetRepository<Center, int>();
            var centers = await repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CenterDto>>(centers);
        }

        public async Task<CenterDto?> GetCenterByIdAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Center, int>();
            var center = await repo.GetByIdAsync(id);
            //return _mapper.Map<CenterDto>(center);
            return center is null ? null : _mapper.Map<CenterDto>(center);
        }

        public async Task<CenterDto?> UpdateCenterAsync(int id, UpdateCenterDto dto)
        {
            var repo = _unitOfWork.GetRepository<Center, int>();
            var center = await repo.GetByIdAsync(id);
            if (center is null) return null;

            _mapper.Map(dto, center);
            repo.Update(center);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<CenterDto>(center);
        }
    }
}
